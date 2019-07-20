using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "gameData", menuName = "Game/Game Data", order = 0)]
public class GameData : M8.SingletonScriptableObject<GameData> {
    public const string sceneVarPlayerName = "playerName";

    public const string sceneVarIsMonitorAwake = "monitorAwake";
    public const string sceneVarIsNetworkDisconnect = "networkDisconnect";

    public enum VolatileType {
        SystemTime,
        ProcessInfo,
        NetworkInfo,
        UserInfo,
        CacheInfo,

        Count
    }

    public class CaptureInfo {
        public Texture2D texture;
        public Vector3 forward;
        public Quaternion rotation;
        public Vector3 postion;

        public void Apply(Camera cam, RenderTexture rt) {
            cam.targetTexture = rt;
            cam.Render();
            cam.targetTexture = null;

            if(!texture)
                texture = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);

            var prevRTActive = RenderTexture.active;
            RenderTexture.active = rt;

            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply();

            RenderTexture.active = prevRTActive;

            var t = cam.transform;
            forward = t.forward;
            rotation = t.rotation;
            postion = t.position;
        }
    }

    public struct DeviceAcquisition {
        public AcquisitionItemData item;
        public int count;
    }

    [Header("Capture")]
    public RenderTexture captureRenderTexture;
    public int captureCount = 4;

    [Header("Volatile Info")]
    public string[] modalVolatiles; //corresponds to VolatileType

    [Header("Lab Inspection Info")]
    public FlaggedItemData[] malwareChecks; //only check the key from SceneState

    public string playerName {
        get {
            if(string.IsNullOrEmpty(mCurPlayerName))
                InitPlayerName();

            return mCurPlayerName;
        }
    }

    public string playerInitial {
        get {
            if(string.IsNullOrEmpty(mCurPlayerInitial))
                InitPlayerName();

            return mCurPlayerInitial;
        }
    }

    public CaptureInfo[] captureInfos {
        get {
            if(mCaptureInfos == null) {
                mCaptureInfos = new CaptureInfo[captureCount];
                for(int i = 0; i < mCaptureInfos.Length; i++)
                    mCaptureInfos[i] = new CaptureInfo();
            }

            return mCaptureInfos;
        }
    }

    public Texture2D captureScreenTexture { get; private set; }
    public bool captureScreenIsMonitorAwake { get; private set; }

    public InteractiveMode currentInteractMode {
        get { return mCurrentInteractMode; }
        set {
            if(mCurrentInteractMode != value) {
                mCurrentInteractMode = value;

                interactModeChanged?.Invoke();
            }
        }
    }

    //on-site specific
    public bool isMonitorAwake { get { return M8.SceneState.instance.global.GetValue(sceneVarIsMonitorAwake) != 0; } }
    public bool isNetworkDisconnect { get { return M8.SceneState.instance.global.GetValue(sceneVarIsNetworkDisconnect) != 0; } }

    public List<VolatileType> volatileAcquisitions { get { return mVolatileAcquisitions; } }

    public List<DeviceAcquisition> deviceAcquisitions { get { return mAcquisitions; } }

    public List<SearchKeywordData> searchKeywords { get { return mSearchKeywords; } }

    public event System.Action interactModeChanged;

    private string mCurPlayerName;
    private string mCurPlayerInitial;
    private CaptureInfo[] mCaptureInfos;

    private InteractiveMode mCurrentInteractMode;

    private List<VolatileType> mVolatileAcquisitions = new List<VolatileType>((int)VolatileType.Count);
    private List<DeviceAcquisition> mAcquisitions = new List<DeviceAcquisition>();

    private List<SearchKeywordData> mSearchKeywords = new List<SearchKeywordData>();

    private const string malwareCheckFormat = "malware_checked_{0}";

    public bool IsMalwareChecked(string key) {
        FlaggedItemData itm = null;
        for(int i = 0; i < malwareChecks.Length; i++) {
            if(malwareChecks[i].key == key) {
                itm = malwareChecks[i];
                break;
            }
        }

        return itm && M8.SceneState.instance.global.GetValue(string.Format(malwareCheckFormat, key)) != 0;
    }

    public void SetMalwareChecked(string key, bool isChecked) {
        FlaggedItemData itm = null;
        for(int i = 0; i < malwareChecks.Length; i++) {
            if(malwareChecks[i].key == key) {
                itm = malwareChecks[i];
                break;
            }
        }

        if(itm) {
            M8.SceneState.instance.global.SetValue(string.Format(malwareCheckFormat, key), isChecked ? 1 : 0, false);
        }
    }

    public void AcquireDevice(AcquisitionItemData item) {
        int ind = -1;
        for(int i = 0; i < mAcquisitions.Count; i++) {
            var acquisition = mAcquisitions[i];
            if(acquisition.item == item) {
                ind = i;
                break;
            }
        }

        if(ind != -1) {
            var acquisition = mAcquisitions[ind];
            acquisition.count++;

            mAcquisitions[ind] = acquisition;
        }
        else {
            mAcquisitions.Add(new DeviceAcquisition { item=item, count=1 });
        }
    }

    public void VolatileOpenModal(VolatileType volatileType) {
        if(modalVolatiles == null)
            return;

        int ind = (int)volatileType;
        if(ind >= 0 && ind < modalVolatiles.Length && !string.IsNullOrEmpty(modalVolatiles[ind])) {

            M8.ModalManager.main.Open(modalVolatiles[ind]);
        }
    }

    public void VolatileAcquire(VolatileType volatileType) {
        if(!mVolatileAcquisitions.Contains(volatileType))
            mVolatileAcquisitions.Add(volatileType);
    }

    public void Capture(int index, Camera cam) {
        if(index >= 0 && index < captureCount) {
            var captureInfo = captureInfos[index];

            captureInfo.Apply(cam, captureRenderTexture);
        }
    }

    public void CaptureScreen(Camera cam) {
        cam.targetTexture = captureRenderTexture;
        cam.Render();
        cam.targetTexture = null;

        if(!captureScreenTexture)
            captureScreenTexture = new Texture2D(captureRenderTexture.width, captureRenderTexture.height, TextureFormat.RGB24, false);

        var prevRTActive = RenderTexture.active;
        RenderTexture.active = captureRenderTexture;

        captureScreenTexture.ReadPixels(new Rect(0, 0, captureScreenTexture.width, captureScreenTexture.height), 0, 0);
        captureScreenTexture.Apply();

        RenderTexture.active = prevRTActive;

        captureScreenIsMonitorAwake = isMonitorAwake;
    }

    public void SetPlayerName(string aPlayerName) {
        mCurPlayerName = aPlayerName;

        M8.SceneState.instance.global.SetValueString(sceneVarPlayerName, mCurPlayerName, true);

        mCurPlayerInitial = GenerateInitial(mCurPlayerName);
    }

    public string GenerateInitial(string aName) {
        if(!string.IsNullOrEmpty(aName)) {
            var words = aName.Split(' ');
            if(words.Length > 1) {
                var sb = new System.Text.StringBuilder(2);

                var firstWord = words[0];
                var lastWord = words[words.Length - 1];

                if(!string.IsNullOrEmpty(firstWord))
                    sb.Append(firstWord[0]);

                if(!string.IsNullOrEmpty(lastWord))
                    sb.Append(lastWord[0]);

                return sb.ToString().ToUpper();
            }
            else if(!string.IsNullOrEmpty(words[0]))
                return words[0].Substring(0, 1).ToUpper();
        }

        return "";
    }

    protected override void OnInstanceInit() {
        mCurPlayerName = "";
        mCurPlayerInitial = "";
        mCaptureInfos = null;

        mCurrentInteractMode = null;

        if(captureScreenTexture) {
            DestroyImmediate(captureScreenTexture);
            captureScreenTexture = null;
        }

        mVolatileAcquisitions = new List<VolatileType>((int)VolatileType.Count);
        mAcquisitions = new List<DeviceAcquisition>();
    }

    private void InitPlayerName() {
        mCurPlayerName = M8.SceneState.instance.global.GetValueString(sceneVarPlayerName);
        mCurPlayerInitial = GenerateInitial(mCurPlayerName);
    }

    private void ClearCaptures() {
        if(mCaptureInfos != null) {
            for(int i = 0; i < mCaptureInfos.Length; i++) {
                var inf = mCaptureInfos[i];
                if(inf.texture)
                    DestroyImmediate(inf.texture);
            }

            mCaptureInfos = null;
        }
    }
}

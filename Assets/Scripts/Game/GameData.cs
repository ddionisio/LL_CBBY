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

    public enum HelpState {
        None,

        PreInvestigateLogin,
        PreInvestigateBriefing,

        InvestigateCameraInstruction,

        InvestigateComputerPower,

        VolatileDataGather,

        DeviceGather,

        CloneDrive,

        DataInvestigate,
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
        public int index;
        public int subIndex;
        public string date;

        public string GetLabel() {
            return string.Format("{0}-{1}-{2:000}-{3:00}", GameData.instance.playerInitial, date, index + 1, subIndex + 1);
        }

        public string GetName() {
            return M8.Localize.Get(item.nameTextRef);
        }
    }

    public struct ActivityLogItem {
        public System.DateTime time;
        public string detailTextRef;

        public string GetTimeString() {
            return time.ToString("g", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }
    }

    [System.Serializable]
    public class VolatileOrderItem {
        public VolatileType[] volatiles;

        public bool Contains(VolatileType v) {
            return System.Array.IndexOf(volatiles, v) != -1;
        }
    }

    public struct VolatileItem {
        public VolatileType type;
        public int score;
    }

    [Header("Data")]
    public int caseNumber = 111007;
    [M8.Localize]
    public string departmentNameTextRef;
    [M8.TagSelector]
    public string tagActivityPopUp;

    [Header("Capture")]
    public RenderTexture captureRenderTexture;
    public int captureCount = 4;
    [M8.TagSelector]
    public string captureTagPOIs;
    public M8.RangeFloat captureAngleRange;
    public int captureScoreValue = 1000;

    [Header("PC Verify")]
    public int pcVerifyScoreValue = 500;

    [Header("Modals")]
    public string modalProgress = "progress";
    public string modalChainOfCustody = "chainOfCustody";

    [Header("Volatile Info")]
    [M8.Localize]
    public string volatileAcquireFormatRef;
    public string[] modalVolatiles; //corresponds to VolatileType
    public VolatileOrderItem[] volatileOrder; //most to least
    public int volatileScoreValue = 300;
    public int volatileScorePenaltyValue = 150;

    [Header("Device Acquisition")]
    public int deviceAcquisitionScoreValue = 100;
    public int deviceAcquisitionScorePenaltyValue = 100;

    [Header("Digital Investigation")]
    public FlaggedItemData[] digitalReportFlaggedItems;
    public int digitalReportScoreValue = 200;

    [Header("Signal Invoke")]
    public SignalActivityLogUpdate signalInvokeActivityLogUpdate;

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

    public List<VolatileItem> volatileAcquisitions { get { return mVolatileAcquisitions; } }

    public List<DeviceAcquisition> deviceAcquisitions { get { return mAcquisitions; } }

    public List<SearchKeywordData> searchKeywords { get { return mSearchKeywords; } }

    public List<ActivityLogItem> activityLogs { get { return mActivityLogs; } }

    //capture
    public float capturePercent { get; private set; }
    public int captureScore { get; private set; }

    //pc verify
    public int pcVerifyScore { get; private set; }

    //volatile
    public int volatileScore { get; private set; }

    //device acquisition
    public int deviceAcquisitionScore { get; private set; }

    //digital investigation
    public int digitalReportScore { get; private set; }

    public HelpState helpState { get; set; }

    public event System.Action interactModeChanged;

    private string mCurPlayerName;
    private string mCurPlayerInitial;
    private CaptureInfo[] mCaptureInfos;

    private InteractiveMode mCurrentInteractMode;

    private List<VolatileItem> mVolatileAcquisitions = new List<VolatileItem>((int)VolatileType.Count);

    private List<DeviceAcquisition> mAcquisitions = new List<DeviceAcquisition>();
    private int mAcquisitionCurIndex = 0;
    private int mAcquisitionCurSubIndex = 0;

    private List<SearchKeywordData> mSearchKeywords = new List<SearchKeywordData>();

    private List<ActivityLogItem> mActivityLogs = new List<ActivityLogItem>();

    private const string malwareCheckFormat = "malware_checked_{0}";

    public int GetFullScore() {
        int photo = captureScoreValue;
        int pcVerify = pcVerifyScoreValue * 3;
        int volatileAcq = mVolatileAcquisitions.Count * volatileScoreValue;

        int deviceAcq = 0;
        for(int i = 0; i < mAcquisitions.Count; i++) {
            var itm = mAcquisitions[i];

            if(itm.item.isValid)
                deviceAcq += deviceAcquisitionScoreValue;
        }

        int digitalReport = 0;
        for(int i = 0; i < digitalReportFlaggedItems.Length; i++) {
            var itm = digitalReportFlaggedItems[i];
            digitalReport += digitalReportScoreValue;
            if(itm.malwareData)
                digitalReport += digitalReportScoreValue;
        }

        return photo + pcVerify + volatileAcq + deviceAcq + digitalReport;
    }

    public void UpdateDigitalReportScore() {
        digitalReportScore = 0;

        for(int i = 0; i < digitalReportFlaggedItems.Length; i++) {
            var itm = digitalReportFlaggedItems[i];
            if(itm.isFlagged) {
                digitalReportScore += digitalReportScoreValue;

                if(itm.malwareData && IsMalwareChecked(itm.key))
                    digitalReportScore += digitalReportScoreValue;
            }
        }

        UpdateLoLScore();
    }

    public void UpdateDeviceAcquisitionScore() {
        deviceAcquisitionScore = 0;

        for(int i = 0; i < mAcquisitions.Count; i++) {
            var itm = mAcquisitions[i];

            if(itm.item.isValid)
                deviceAcquisitionScore += deviceAcquisitionScoreValue;
            else
                deviceAcquisitionScore -= deviceAcquisitionScorePenaltyValue;
        }

        if(deviceAcquisitionScore < 0)
            deviceAcquisitionScore = 0;

        UpdateLoLScore();
    }

    public void UpdateVolatileScore() {
        volatileScore = 0;

        for(int i = 0; i < mVolatileAcquisitions.Count; i++)
            volatileScore += mVolatileAcquisitions[i].score;

        UpdateLoLScore();
    }

    public void UpdatePCVerifyScore() {
        pcVerifyScore = 0;

        if(isNetworkDisconnect)
            pcVerifyScore += pcVerifyScoreValue;
        if(isMonitorAwake)
            pcVerifyScore += pcVerifyScoreValue;
        if(captureScreenTexture && captureScreenIsMonitorAwake)
            pcVerifyScore += pcVerifyScoreValue;

        UpdateLoLScore();
    }

    public void UpdateCaptureScore() {
        var capturePOIsGO = GameObject.FindGameObjectWithTag(captureTagPOIs);
                
        if(capturePOIsGO) {
            capturePercent = 0f;
            captureScore = 0;

            var cam = Camera.main;
            var camT = cam.transform;

            var capturePOIsRoot = capturePOIsGO.transform;

            int count = capturePOIsRoot.childCount;

            float totalAngle = 0f;
            float totalPercent = 0f;

            for(int i = 0; i < count; i++) {
                var t = capturePOIsRoot.GetChild(i);

                var dir = (t.position - camT.position).normalized;

                //get lowest angle from captures
                float angle = float.MaxValue;

                for(int j = 0; j < captureCount; j++) {
                    var captureItm = captureInfos[j];

                    var a = captureItm.texture ? Vector3.Angle(captureItm.forward, dir) : float.MaxValue;
                    if(a < angle)
                        angle = a;
                }

                totalAngle += angle;

                float percent;

                if(angle < captureAngleRange.min)
                    percent = 1f;
                else if(angle > captureAngleRange.max)
                    percent = 0f;
                else
                    percent = 1f - ((angle - captureAngleRange.min) / captureAngleRange.length);

                totalPercent += percent;
            }

            capturePercent = totalPercent / count;
            captureScore = Mathf.RoundToInt(capturePercent * captureScoreValue);

            UpdateLoLScore();
        }
    }

    private void UpdateLoLScore() {
        LoLManager.instance.curScore = captureScore + pcVerifyScore + volatileScore + deviceAcquisitionScore + digitalReportScore;
    }

    public ActivityLogPopUpWidget GetActivityLogPopUpWidget() {
        var go = GameObject.FindGameObjectWithTag(tagActivityPopUp);
        return go ? go.GetComponent<ActivityLogPopUpWidget>() : null;
    }

    public void AddActivityLog(string descTextRef) {
        var newLog = new ActivityLogItem { time = System.DateTime.Now, detailTextRef = descTextRef };

        mActivityLogs.Add(newLog);

        signalInvokeActivityLogUpdate.Invoke(newLog);
    }

    public string GetVolatileTypeText(VolatileType volatileType) {
        return M8.Localize.Get("volatile_data_" + volatileType.ToString());
    }

    public bool IsMalwareChecked(string key) {
        return M8.SceneState.instance.global.GetValue(string.Format(malwareCheckFormat, key)) != 0;
    }

    public void SetMalwareChecked(string key, bool isChecked) {
        M8.SceneState.instance.global.SetValue(string.Format(malwareCheckFormat, key), isChecked ? 1 : 0, false);
    }

    public void AcquireDevice(AcquisitionItemData item) {
        var dateDat = System.DateTime.Now;
        var dateStr = dateDat.ToString("dd-MM-yy", System.Globalization.DateTimeFormatInfo.InvariantInfo);

        mAcquisitions.Add(new DeviceAcquisition { item=item, index=mAcquisitionCurIndex, subIndex=mAcquisitionCurSubIndex, date=dateStr });

        mAcquisitionCurSubIndex++;
    }

    public void AcquireIncrementIndex() {
        mAcquisitionCurIndex++;
        mAcquisitionCurSubIndex = 0;
    }

    public DeviceAcquisition[] GetAcquisitions(AcquisitionItemData[] data) {
        List<DeviceAcquisition> ret = new List<DeviceAcquisition>();

        for(int i = 0; i < data.Length; i++) {
            for(int j = 0; j < mAcquisitions.Count; j++) {
                var acq = mAcquisitions[j];
                if(acq.item == data[i]) {
                    ret.Add(acq);
                    break;
                }
            }
        }

        return ret.ToArray();
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
        if(!VolatileContains(volatileType)) {
            //generate score
            int score = 0;

            int orderInd = -1;
            for(int i = 0; i < volatileOrder.Length; i++) {
                if(volatileOrder[i].Contains(volatileType)) {
                    orderInd = i;
                    break;
                }
            }

            if(orderInd != -1) {
                //get difference and determine score with penalty
                int ind = Mathf.Clamp(mVolatileAcquisitions.Count, 0, volatileOrder.Length - 1);
                int diff = Mathf.Abs(orderInd - ind);

                score = volatileScoreValue - (diff * volatileScorePenaltyValue);
                if(score < 0)
                    score = 0;
            }

            //add
            mVolatileAcquisitions.Add(new VolatileItem { type=volatileType, score=score });
        }
    }

    public bool VolatileContains(VolatileType type) {
        for(int i = 0; i < mVolatileAcquisitions.Count; i++) {
            if(mVolatileAcquisitions[i].type == type)
                return true;
        }

        return false;
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

        mVolatileAcquisitions = new List<VolatileItem>((int)VolatileType.Count);

        mAcquisitions = new List<DeviceAcquisition>();
        mAcquisitionCurIndex = 0;
        mAcquisitionCurSubIndex = 0;

        mSearchKeywords = new List<SearchKeywordData>();

        mActivityLogs = new List<ActivityLogItem>();
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

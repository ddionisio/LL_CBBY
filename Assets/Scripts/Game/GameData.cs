using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "gameData", menuName = "Game/Game Data", order = 0)]
public class GameData : M8.SingletonScriptableObject<GameData> {
    public const string sceneVarPlayerName = "playerName";

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

    [Header("Capture")]
    public RenderTexture captureRenderTexture;
    public int captureCount = 4;

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

    public InteractiveMode currentInteractMode { get; private set; }

    public event System.Action interactModeChanged;

    private string mCurPlayerName;
    private string mCurPlayerInitial;
    private CaptureInfo[] mCaptureInfos;

    public void SetCurrentInteractMode(InteractiveMode toMode) {
        currentInteractMode = toMode;

        interactModeChanged?.Invoke();
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

        RenderTexture.active = prevRTActive;
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

    private void InitPlayerName() {
        mCurPlayerName = M8.SceneState.instance.global.GetValueString(sceneVarPlayerName);
        mCurPlayerInitial = GenerateInitial(mCurPlayerName);
    }
}

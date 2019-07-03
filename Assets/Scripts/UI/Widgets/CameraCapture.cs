using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour {
    [Header("Display")]
    public RawImage captureImage;

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeCapture;

    private int mCurCaptureIndex;

    private Camera mCamera;

    public void Capture() {
        if(!mCamera)
            mCamera = Camera.main;

        GameData.instance.Capture(mCurCaptureIndex, mCamera);
        ApplyCapture(mCurCaptureIndex);

        if(animator && !string.IsNullOrEmpty(takeCapture))
            animator.Play(takeCapture);

        mCurCaptureIndex++;
        if(mCurCaptureIndex == GameData.instance.captureTextures.Length)
            mCurCaptureIndex = 0;
    }

    void OnEnable() {
        //get available capture
        var captureInfos = GameData.instance.captureInfos;

        var availableInd = -1;

        for(int i = 0; i < captureInfos.Length; i++) {
            var inf = captureInfos[i];
            if(!inf.isCaptured) {
                availableInd = i;
                break;
            }
        }

        if(availableInd == -1)
            mCurCaptureIndex = 0;
        else
            mCurCaptureIndex = availableInd;

        //determine which capture to put on image
        var capturedInd = -1;

        for(int i = 0; i < captureInfos.Length; i++) {
            var inf = captureInfos[i];
            if(!inf.isCaptured) {
                capturedInd = i;
                break;
            }
        }

        ApplyCapture(capturedInd);
    }

    private void ApplyCapture(int index) {
        if(index != -1) {
            captureImage.gameObject.SetActive(true);
            captureImage.texture = GameData.instance.captureTextures[index];
        }
        else {
            captureImage.gameObject.SetActive(false);
        }
    }
}

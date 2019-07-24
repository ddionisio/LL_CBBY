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

    private Coroutine mCaptureRout;

    public void Capture() {
        if(mCaptureRout != null)
            return;

        if(!mCamera)
            mCamera = Camera.main;

        mCaptureRout = StartCoroutine(DoCapture());
    }

    void OnEnable() {
        //get available capture
        var captureInfos = GameData.instance.captureInfos;

        var availableInd = -1;

        for(int i = 0; i < captureInfos.Length; i++) {
            var inf = captureInfos[i];
            if(!inf.texture) {
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
            if(inf.texture) {
                capturedInd = i;
                break;
            }
        }

        ApplyCapture(capturedInd);
    }

    void OnDisable() {
        mCaptureRout = null;
    }

    IEnumerator DoCapture() {
        GameData.instance.Capture(mCurCaptureIndex, mCamera);

        if(animator && !string.IsNullOrEmpty(takeCapture))
            yield return animator.PlayWait(takeCapture);

        ApplyCapture(mCurCaptureIndex);

        mCurCaptureIndex++;
        if(mCurCaptureIndex == GameData.instance.captureCount)
            mCurCaptureIndex = 0;

        mCaptureRout = null;
    }

    private void ApplyCapture(int index) {
        if(index != -1) {
            captureImage.gameObject.SetActive(true);
            captureImage.texture = GameData.instance.captureInfos[index].texture;
        }
        else {
            captureImage.gameObject.SetActive(false);
        }
    }
}

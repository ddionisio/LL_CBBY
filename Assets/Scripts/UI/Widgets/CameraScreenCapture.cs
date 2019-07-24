using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScreenCapture : MonoBehaviour {
    [Header("Display")]
    public RawImage captureImage;

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeCapture;

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
        ApplyCapture();
    }

    void OnDisable() {
        mCaptureRout = null;
    }

    IEnumerator DoCapture() {
        GameData.instance.CaptureScreen(mCamera);

        if(animator && !string.IsNullOrEmpty(takeCapture))
            yield return animator.PlayWait(takeCapture);

        ApplyCapture();

        mCaptureRout = null;
    }

    private void ApplyCapture() {
        var captureImg = GameData.instance.captureScreenTexture;
        if(captureImg) {
            captureImage.gameObject.SetActive(true);
            captureImage.texture = captureImg;
        }
        else
            captureImage.gameObject.SetActive(false);
    }
}

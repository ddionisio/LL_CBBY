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

    public void Capture() {
        if(!mCamera)
            mCamera = Camera.main;

        GameData.instance.CaptureScreen(mCamera);

        ApplyCapture();
    }

    void OnEnable() {
        ApplyCapture();
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

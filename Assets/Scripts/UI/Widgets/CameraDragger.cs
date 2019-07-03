using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [Header("Config")]
    public float angleYawLimit = 90f; //euler Y
    public float anglePitchLimit = 40f; //euler X
    public float dragYawScale = 1f;
    public float dragPitchScale = 1f;
    public float cameraDirDelay = 0.3f;

    [Header("Display")]
    public GameObject dragDisplayGO;

    private Camera mCamera;

    private float mCurYaw;
    private float mCurPitch;
    private Vector3 mCurForward;

    private bool mIsDragging;

    private Coroutine mCamRout;

    void OnApplicationFocus(bool focus) {
        if(!focus)
            DragEnd();
    }

    void OnEnable() {
        if(dragDisplayGO) dragDisplayGO.SetActive(false);
    }

    void OnDisable() {
        DragEnd();

        if(mCamRout != null) {
            StopCoroutine(mCamRout);
            mCamRout = null;
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
        if(!mCamera)
            mCamera = Camera.main;

        var camRot = mCamera.transform.localEulerAngles;
        mCurYaw = camRot.y > angleYawLimit ? camRot.y - 360f : camRot.y;
        mCurPitch = camRot.x > anglePitchLimit ? camRot.x - 360f : camRot.x;
        mCurForward = mCamera.transform.forward;

        if(dragDisplayGO) {
            dragDisplayGO.SetActive(true);
            dragDisplayGO.transform.position = eventData.position;
        }
                
        mIsDragging = true;

        if(mCamRout == null)
            mCamRout = StartCoroutine(DoCameraDir());
    }

    void IDragHandler.OnDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        //update display
        if(dragDisplayGO)
            dragDisplayGO.transform.position = eventData.position;
                
        //update camera forward destination
        var pitchDelta = eventData.delta.y * dragPitchScale;
        var yawDelta = eventData.delta.x * dragYawScale;

        mCurPitch = Mathf.Clamp(mCurPitch + pitchDelta, -anglePitchLimit, anglePitchLimit);
        mCurYaw = Mathf.Clamp(mCurYaw + yawDelta, -angleYawLimit, angleYawLimit);
                
        mCurForward = Quaternion.Euler(mCurPitch, mCurYaw, 0f) * Vector3.forward;

        //Debug.Log(string.Format("yaw: {0} pitch: {1} forward: {2}", mCurYaw, mCurPitch, mCurForward));
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
        if(!mIsDragging)
            return;

        DragEnd();
    }

    void DragEnd() {
        if(dragDisplayGO)
            dragDisplayGO.SetActive(false);

        mIsDragging = false;
    }

    IEnumerator DoCameraDir() {
        var curVel = Vector3.zero;

        var camTrans = mCamera.transform;

        while(mIsDragging || camTrans.forward != mCurForward) {
            var forward = Vector3.SmoothDamp(camTrans.forward, mCurForward, ref curVel, cameraDirDelay);

            camTrans.forward = forward;

            yield return null;
        }

        mCamRout = null;
    }
}

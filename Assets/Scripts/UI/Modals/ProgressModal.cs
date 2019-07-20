using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressModal : M8.ModalController, M8.IModalPush, M8.IModalActive {
    public const string parmDelay = "delay";
    public const string parmTitleString = "title";
    public const string parmCallback = "cb";

    public float startDelay = 0.3f;
    public float endDelay = 2f;

    [Header("Display")]
    public Text titleText;
    public Image imageFill;
    public Text percentText;

    private System.Action mCallback;

    private float mDelay;

    private Coroutine mRout;

    void M8.IModalActive.SetActive(bool aActive) {
        if(aActive) {
            if(mDelay > 0f)
                mRout = StartCoroutine(DoProgress());
            else
                Close();
        }
        else
            StopRout();
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {
        titleText.text = "";
        mCallback = null;
        mDelay = 0f;

        if(parms != null) {
            if(parms.ContainsKey(parmDelay))
                mDelay = parms.GetValue<float>(parmDelay);

            if(parms.ContainsKey(parmTitleString))
                titleText.text = parms.GetValue<string>(parmTitleString);

            if(parms.ContainsKey(parmCallback))
                mCallback = parms.GetValue<System.Action>(parmCallback);
        }

        SetProgress(0f);
    }

    void OnDisable() {
        mCallback = null;
        StopRout();
    }

    IEnumerator DoProgress() {
        yield return new WaitForSeconds(startDelay);

        float curTime = 0f;
        while(curTime < mDelay) {
            yield return null;

            curTime += Time.deltaTime;

            float t = Mathf.Clamp01(curTime / mDelay);

            SetProgress(t);
        }

        yield return new WaitForSeconds(endDelay);

        mRout = null;

        var cb = mCallback;

        Close();

        if(cb != null)
            cb();
    }

    private void SetProgress(float t) {
        //t = [0, 1]
        imageFill.fillAmount = t;
        percentText.text = Mathf.FloorToInt(t * 100f).ToString() + "%";
    }

    private void StopRout() {
        if(mRout != null) {
            StopCoroutine(mRout);
            mRout = null;
        }
    }
}

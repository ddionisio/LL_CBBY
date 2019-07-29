using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : M8.SingletonBehaviour<HUD> {
    [Header("Display")]
    public bool isVisibleOnEnable;
    public GameObject rootGO;

    [Header("Next")]
    public GameObject nextButtonGO;
    public GameObject nextIndicatorGO;
    public AnimatorEnterExit nextEnterExit;

    [Header("Previous")]
    public GameObject prevButtonGO;
    public AnimatorEnterExit prevEnterExit;

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeEnter;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeExit;

    public bool isVisible { get { return rootGO.activeSelf; } }
    public bool isBusy { get { return mRout != null; } }

    private Coroutine mRout;
    private bool mIsNextShown;
    private bool mIsPrevShown;
        
    public void Show() {
        Stop();
        mRout = StartCoroutine(DoShow());
    }

    public void Hide() {
        Stop();
        mRout = StartCoroutine(DoHide());
    }
        
    public void Stop() {
        if(mRout != null) {
            StopCoroutine(mRout);
            mRout = null;
        }
    }

    public void NextSetShow(bool show, bool showIndicator) {
        if(mIsNextShown != show) {
            mIsNextShown = show;

            if(show)
                StartCoroutine(DoShow(nextButtonGO, showIndicator ? nextIndicatorGO : null, nextEnterExit));
            else
                StartCoroutine(DoHide(nextButtonGO, nextEnterExit));
        }
    }

    public void PrevSetShow(bool show) {
        if(mIsPrevShown != show) {
            mIsPrevShown = show;

            if(show)
                StartCoroutine(DoShow(prevButtonGO, null, prevEnterExit));
            else
                StartCoroutine(DoHide(prevButtonGO, prevEnterExit));
        }
    }

    protected override void OnInstanceInit() {
    }

    void OnEnable() {
        rootGO.SetActive(isVisibleOnEnable);

        nextButtonGO.SetActive(false);
        prevButtonGO.SetActive(false);
        nextIndicatorGO.SetActive(false);

        mIsNextShown = false;
        mIsPrevShown = false;
    }

    IEnumerator DoShow(GameObject go, GameObject postActiveGO, AnimatorEnterExit animatorEnterExit) {
        go.SetActive(true);

        if(animatorEnterExit) {
            while(animatorEnterExit.isPlaying)
                yield return null;

            yield return animatorEnterExit.PlayEnterWait();

            if(postActiveGO)
                postActiveGO.SetActive(true);
        }
    }

    IEnumerator DoHide(GameObject go, AnimatorEnterExit animatorEnterExit) {
        if(animatorEnterExit) {
            while(animatorEnterExit.isPlaying)
                yield return null;

            yield return animatorEnterExit.PlayExitWait();
        }

        go.SetActive(false);
    }

    IEnumerator DoShow() {
        nextButtonGO.SetActive(false);
        nextIndicatorGO.SetActive(false);

        prevButtonGO.SetActive(false);

        rootGO.SetActive(true);

        if(animator && !string.IsNullOrEmpty(takeEnter))
            yield return animator.PlayWait(takeEnter);

        //show prev and/or next
        if(mIsNextShown)
            StartCoroutine(DoShow(nextButtonGO, null, nextEnterExit));

        if(mIsPrevShown)
            StartCoroutine(DoShow(prevButtonGO, null, prevEnterExit));

        mRout = null;
    }

    IEnumerator DoHide() {
        if(animator && !string.IsNullOrEmpty(takeExit))
            yield return animator.PlayWait(takeExit);

        rootGO.SetActive(false);

        mRout = null;
    }
}

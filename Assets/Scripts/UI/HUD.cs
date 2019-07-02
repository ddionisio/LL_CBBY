using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : M8.SingletonBehaviour<HUD> {
    [Header("Display")]
    public bool isVisibleOnEnable;
    public GameObject rootGO;
    public Button nextButton;

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeEnter;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeExit;

    [Header("Signal Invoke")]
    public M8.Signal signalInvokeNext;

    public bool isVisible { get { return rootGO.activeSelf; } }
    public bool isBusy { get { return mRout != null; } }

    private Coroutine mRout;
        
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

    public void SetNextInteractive(bool interactive) {
        nextButton.interactable = interactive;
    }

    protected override void OnInstanceInit() {
        nextButton.onClick.AddListener(OnNext);
    }

    void OnEnable() {
        rootGO.SetActive(isVisibleOnEnable);

        nextButton.interactable = false;
    }

    void OnNext() {
        signalInvokeNext.Invoke();
    }

    IEnumerator DoShow() {
        rootGO.SetActive(true);

        if(animator && !string.IsNullOrEmpty(takeEnter))
            yield return animator.PlayWait(takeEnter);

        mRout = null;
    }

    IEnumerator DoHide() {
        if(animator && !string.IsNullOrEmpty(takeExit))
            yield return animator.PlayWait(takeExit);

        rootGO.SetActive(false);

        mRout = null;
    }
}

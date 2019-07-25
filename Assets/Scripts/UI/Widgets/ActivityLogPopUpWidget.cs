using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivityLogPopUpWidget : MonoBehaviour {
    [Header("Data")]
    public float delay = 3f;

    [Header("UI")]
    public Text timeText;
    public Text detailText;

    [Header("Animation")]
    public M8.Animator.Animate animator;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeEnter;
    [M8.Animator.TakeSelector(animatorField = "animator")]
    public string takeExit;

    [Header("Signal Listen")]
    public SignalActivityLogUpdate signalListenActivityLogUpdate;

    public bool isRunning { get { return mRout != null; } }

    private Queue<GameData.ActivityLogItem> mLogQueue = new Queue<GameData.ActivityLogItem>();

    private Coroutine mRout;

    public void Close() {
        if(mRout != null) {
            StopCoroutine(mRout);
            mRout = null;
        }

        mLogQueue.Clear();

        if(animator && !string.IsNullOrEmpty(takeExit))
            animator.Play(takeExit);
    }

    void OnDisable() {
        if(mRout != null) {
            StopCoroutine(mRout);
            mRout = null;
        }

        mLogQueue.Clear();

        signalListenActivityLogUpdate.callback -= OnActivityLogUpdate;
    }

    void OnEnable() {
        signalListenActivityLogUpdate.callback += OnActivityLogUpdate;

        if(animator && !string.IsNullOrEmpty(takeEnter))
            animator.ResetTake(takeEnter);
    }

    private void OnActivityLogUpdate(GameData.ActivityLogItem itm) {
        mLogQueue.Enqueue(itm);

        if(mRout == null)
            mRout = StartCoroutine(DoPopup());
    }

    IEnumerator DoPopup() {
        var wait = new WaitForSeconds(delay);

        while(mLogQueue.Count > 0) {
            var log = mLogQueue.Dequeue();

            timeText.text = log.GetTimeString();
            detailText.text = M8.Localize.Get(log.detailTextRef);

            if(animator && !string.IsNullOrEmpty(takeEnter))
                yield return animator.PlayWait(takeEnter);

            yield return wait;

            if(animator && !string.IsNullOrEmpty(takeExit))
                yield return animator.PlayWait(takeExit);
        }

        mRout = null;
    }
}

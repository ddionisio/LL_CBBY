using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemTimeModal : M8.ModalController, M8.IModalPush {
    [Header("UI")]
    public Text daysText;
    public Text hoursText;
    public Text minutesText;
    public Text secondsText;
    public Text millisecondsText;
    public Text ticksText;

    private DateTime mDateTime;
    private bool mIsSystemTimeInit;

    void M8.IModalPush.Push(M8.GenericParams parms) {
        if(!mIsSystemTimeInit) {
            mIsSystemTimeInit = true;

            mDateTime = DateTime.Now;

            daysText.text = mDateTime.Day.ToString();
            hoursText.text = mDateTime.Hour.ToString();
            minutesText.text = mDateTime.Minute.ToString();
            secondsText.text = mDateTime.Second.ToString();
            millisecondsText.text = mDateTime.Millisecond.ToString();
            ticksText.text = mDateTime.Ticks.ToString();
        }
    }
}

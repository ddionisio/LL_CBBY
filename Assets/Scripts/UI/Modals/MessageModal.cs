using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageModal : M8.ModalController, M8.IModalPush {
    public const string modal = "message";

    public const string parmTitle = "title";
    public const string parmText = "text";

    [Header("UI")]
    public Text titleText;
    public Text bodyText;

    public static void Open(string title, string text) {
        var parms = new M8.GenericParams();
        parms[parmTitle] = title;
        parms[parmText] = text;

        M8.ModalManager.main.Open(modal, parms);
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {
        titleText.text = "";
        bodyText.text = "";

        if(parms != null) {
            if(parms.ContainsKey(parmTitle))
                titleText.text = parms.GetValue<string>(parmTitle);

            if(parms.ContainsKey(parmText))
                bodyText.text = parms.GetValue<string>(parmText);
        }
    }
}

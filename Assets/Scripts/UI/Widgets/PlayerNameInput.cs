using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour {
    [Header("UI")]
    public InputField nameInput;
    public Text initialText;
    public Button confirmButton;

    [Header("Signal Invoke")]
    public M8.Signal signalInvokeNext;

    private string mCurName;
    private string mCurInitial;

    void OnEnable() {
        //initialize strings

        //determine if confirmButton is enabled
    }

    void Awake() {
        
    }

    void OnNameChanged(string nameVal) {

    }

    void OnNameSubmit(string nameVal) {

    }

    void OnConfirmClick() {

    }
}

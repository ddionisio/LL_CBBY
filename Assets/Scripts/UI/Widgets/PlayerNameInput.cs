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

    void OnEnable() {
        //initialize strings
        nameInput.text = GameData.instance.playerName;
        initialText.text = GameData.instance.playerInitial;
        nameInput.interactable = true;

        nameInput.ActivateInputField();
    }

    void Awake() {
        nameInput.onValueChanged.AddListener(OnNameChanged);
        nameInput.onEndEdit.AddListener(OnNameSubmit);

        confirmButton.onClick.AddListener(OnConfirmClick);
    }

    void OnNameChanged(string nameVal) {
        initialText.text = GameData.instance.GenerateInitial(nameVal);

        //determine if confirmButton is enabled
        confirmButton.interactable = !string.IsNullOrEmpty(nameInput.text);
    }

    void OnNameSubmit(string nameVal) {
        if(!string.IsNullOrEmpty(nameVal)) {
            Proceed(nameVal);
        }
    }

    void OnConfirmClick() {
        if(!string.IsNullOrEmpty(initialText.text)) {
            Proceed(initialText.text);
        }
    }

    private void Proceed(string aName) {
        GameData.instance.SetPlayerName(aName);

        confirmButton.interactable = false;

        signalInvokeNext.Invoke();
    }
}

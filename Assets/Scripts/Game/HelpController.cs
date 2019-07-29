using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpController : MonoBehaviour {
    [Header("Pre-Investigation")]
    [M8.Localize]
    public string preInvestigateLogin;
    [M8.Localize]
    public string preInvestigateBriefing;

    [Header("Investigation On-site")]
    [M8.TagSelector]
    public string tagCameraDrag;

    [M8.Localize]
    public string computerPowerCheck;

    [Header("Signal Listen")]
    public M8.Signal signalListenExecute;

    void OnDisable() {
        signalListenExecute.callback -= OnSignalExecute;
    }

    void OnEnable() {
        signalListenExecute.callback += OnSignalExecute;
    }

    void OnSignalExecute() {
        switch(GameData.instance.helpState) {
            case GameData.HelpState.PreInvestigateLogin:
                ModalDialog.Open(null, preInvestigateLogin, OnDialogNextClose);
                break;

            case GameData.HelpState.PreInvestigateBriefing:
                ModalDialog.Open(null, preInvestigateBriefing, OnDialogNextClose);
                break;

            case GameData.HelpState.InvestigateCameraInstruction:
                var cameraDragInstructGO = GameObject.FindGameObjectWithTag(tagCameraDrag);
                if(cameraDragInstructGO) {
                    var camDragger = cameraDragInstructGO.GetComponent<CameraDragger>();
                    if(camDragger) {
                        if(camDragger.hideOnDragGO)
                            camDragger.hideOnDragGO.SetActive(true);
                        if(camDragger.clickInstructGO) {
                            camDragger.clickInstructGO.SetActive(false);
                            camDragger.clickInstructGO.SetActive(true);
                        }
                    }
                }
                break;

            case GameData.HelpState.InvestigateComputerPower:
                ModalDialog.Open(null, computerPowerCheck, OnDialogNextClose);
                break;
        }
    }

    void OnDialogNextClose() {
        ModalDialog.CloseGeneric();
    }
}

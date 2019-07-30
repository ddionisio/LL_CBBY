using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DigitalInvestigationReportProxy : MonoBehaviour {
    public string modal = "digitalInvestigationResult";
    public bool proceedDisable = true;

    private M8.GenericParams mParms = new M8.GenericParams();

    public void Invoke() {
        mParms[DigitalInvestigationReportModal.parmDisableProceed] = proceedDisable;

        M8.ModalManager.main.Open(modal, mParms);
    }
}

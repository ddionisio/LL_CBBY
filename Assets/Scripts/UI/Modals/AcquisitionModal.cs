using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquisitionModal : M8.ModalController, M8.IModalPush {
    public const string parmItems = "items"; //AcquisitionItemData[]

    [Header("Signal Invoke")]
    public SignalBoolean signalInvokeConfirm;

    public void Confirm(bool isConfirm) {
        if(signalInvokeConfirm)
            signalInvokeConfirm.Invoke(isConfirm);

        Close();
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {

    }
}

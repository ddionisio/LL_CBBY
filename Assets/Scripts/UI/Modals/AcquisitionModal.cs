using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcquisitionModal : M8.ModalController, M8.IModalPush {
    public const string parmItems = "items"; //AcquisitionItemData[]

    [Header("Data")]
    [M8.Localize]
    public string itemsHeaderTextRef;

    [Header("UI")]
    public Text itemsText;

    [Header("Signal Invoke")]
    public SignalBoolean signalInvokeConfirm;

    private AcquisitionItemData[] mItems;

    public void Confirm(bool isConfirm) {
        if(signalInvokeConfirm)
            signalInvokeConfirm.Invoke(isConfirm);

        Close();
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {
        mItems = null;

        if(parms != null) {
            if(parms.ContainsKey(parmItems))
                mItems = parms.GetValue<AcquisitionItemData[]>(parmItems);
        }

        if(mItems != null && mItems.Length > 0) {
            itemsText.gameObject.SetActive(true);

            var sb = new System.Text.StringBuilder();

            sb.Append(M8.Localize.Get(itemsHeaderTextRef)).Append(' ');

            for(int i = 0; i < mItems.Length; i++) {
                var itm = mItems[i];

                sb.Append(M8.Localize.Get(itm.nameTextRef));

                if(i < mItems.Length - 1)
                    sb.Append(", ");
            }

            itemsText.text = sb.ToString();
        }
        else {
            itemsText.gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainOfCustodyProxy : MonoBehaviour
{
    [M8.Localize]
    public string releasedBy;
    public bool releasedByIsPlayer;

    [M8.Localize]
    public string receivedBy;
    public bool receivedByIsPlayer;

    [M8.Localize]
    public string purpose;

    public AcquisitionItemData[] items;
    public bool isAllItems;

    private M8.GenericParams mParms = new M8.GenericParams();

    public void Invoke() {
        mParms[ChainOfCustodyModal.parmDateApply] = false;
        mParms[ChainOfCustodyModal.parmReleasedByString] = releasedByIsPlayer ? GameData.instance.playerName : M8.Localize.Get(releasedBy);
        mParms[ChainOfCustodyModal.parmReceivedByString] = receivedByIsPlayer ? GameData.instance.playerName : M8.Localize.Get(receivedBy);
        mParms[ChainOfCustodyModal.parmPurposeString] = M8.Localize.Get(purpose);

        if(isAllItems) {
            mParms[ChainOfCustodyModal.parmItems] = GameData.instance.deviceAcquisitions.ToArray();
        }
        else if(items != null && items.Length > 0) {
            var acqs = GameData.instance.GetAcquisitions(items);
            mParms[ChainOfCustodyModal.parmItems] = acqs;
        }

        M8.ModalManager.main.Open(GameData.instance.modalChainOfCustody, mParms);
    }
}

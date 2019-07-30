using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceAcquisitionResultModal : M8.ModalController, M8.IModalPush {
    [Header("UI")]
    public ChainOfCustodyItemWidget itemTemplate;

    public Text pointsText;

    public Transform contentRoot;
    public ScrollRect scroller;

    private List<ChainOfCustodyItemWidget> mItemsActive = new List<ChainOfCustodyItemWidget>();
    private List<ChainOfCustodyItemWidget> mItemsCache = new List<ChainOfCustodyItemWidget>();

    void M8.IModalPush.Push(M8.GenericParams parms) {

        GameData.instance.UpdateDeviceAcquisitionScore();

        var devices = GameData.instance.deviceAcquisitions.ToArray();

        ClearItems();

        for(int i = 0; i < devices.Length; i++) {
            AllocateItem(devices[i]);
        }

        pointsText.text = GameData.instance.deviceAcquisitionScore.ToString();

        scroller.normalizedPosition = new Vector2(0f, 1f);
    }

    void Awake() {
        itemTemplate.gameObject.SetActive(false);
    }

    private void ClearItems() {
        for(int i = 0; i < mItemsActive.Count; i++) {
            mItemsActive[i].gameObject.SetActive(false);
            mItemsCache.Add(mItemsActive[i]);
        }

        mItemsActive.Clear();
    }

    private void AllocateItem(GameData.DeviceAcquisition device) {
        ChainOfCustodyItemWidget item;

        if(mItemsCache.Count > 0) {
            int ind = mItemsCache.Count - 1;
            item = mItemsCache[ind];
            mItemsCache.RemoveAt(ind);
        }
        else {
            item = Instantiate(itemTemplate);
        }

        item.transform.SetParent(contentRoot, false);
        item.transform.SetAsLastSibling();

        item.idText.text = device.GetLabel();
        item.descText.text = device.GetName();
        item.valueText.text = (device.item.isValid ? GameData.instance.deviceAcquisitionScoreValue : -GameData.instance.deviceAcquisitionScorePenaltyValue).ToString();

        item.gameObject.SetActive(true);

        mItemsActive.Add(item);
    }
}

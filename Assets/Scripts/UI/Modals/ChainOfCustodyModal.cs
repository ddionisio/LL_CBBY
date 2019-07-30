using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChainOfCustodyModal : M8.ModalController, M8.IModalPush {
    public const string parmDateApply = "dateApply"; //bool, true=update date/time
    public const string parmReleasedByString = "releasedBy";
    public const string parmReceivedByString = "receivedBy";
    public const string parmPurposeString = "purpose";
    public const string parmItems = "items"; //DeviceAcquisition[]

    [Header("Data")]
    [M8.Localize]
    public string dateFormatRef;
    [M8.Localize]
    public string caseFormatRef;
    [M8.Localize]
    public string departmentFormatRef;

    [Header("UI")]
    public ChainOfCustodyItemWidget itemTemplate;

    public Text dateTimeText;
    public Text caseNumText;
    public Text departmentText;
    public Text releasedByText;
    public Text receivedByText;
    public Text purposeText;

    public Transform contentRoot;
    public ScrollRect scroller;

    private List<ChainOfCustodyItemWidget> mItemsActive = new List<ChainOfCustodyItemWidget>();
    private List<ChainOfCustodyItemWidget> mItemsCache = new List<ChainOfCustodyItemWidget>();

    void M8.IModalPush.Push(M8.GenericParams parms) {
        bool isDateApply = false;
        string releasedBy = null;
        string receivedBy = null;
        string purpose = null;
        GameData.DeviceAcquisition[] devices = null;

        if(parms != null) {
            if(parms.ContainsKey(parmDateApply))
                isDateApply = parms.GetValue<bool>(parmDateApply);

            if(parms.ContainsKey(parmReleasedByString))
                releasedBy = parms.GetValue<string>(parmReleasedByString);

            if(parms.ContainsKey(parmReceivedByString))
                receivedBy = parms.GetValue<string>(parmReceivedByString);

            if(parms.ContainsKey(parmPurposeString))
                purpose = parms.GetValue<string>(parmPurposeString);

            if(parms.ContainsKey(parmItems))
                devices = parms.GetValue<GameData.DeviceAcquisition[]>(parmItems);
        }

        if(isDateApply) {
            var dateDat = System.DateTime.Now;
            var dateString = dateDat.ToString("g", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            dateTimeText.text = string.Format(M8.Localize.Get(dateFormatRef), dateString);
        }

        caseNumText.text = string.Format(M8.Localize.Get(caseFormatRef), GameData.instance.caseNumber.ToString());

        departmentText.text = string.Format(M8.Localize.Get(departmentFormatRef), M8.Localize.Get(GameData.instance.departmentNameTextRef));

        if(!string.IsNullOrEmpty(releasedBy))
            releasedByText.text = releasedBy;

        if(!string.IsNullOrEmpty(receivedBy))
            receivedByText.text = receivedBy;

        if(!string.IsNullOrEmpty(purpose))
            purposeText.text = purpose;

        if(devices != null) {
            ClearItems();

            for(int i = 0; i < devices.Length; i++) {
                AllocateItem(devices[i]);
            }
        }

        scroller.normalizedPosition = new Vector2(0f, 1f);
    }

    void Awake() {
        itemTemplate.gameObject.SetActive(false);

        dateTimeText.text = "";
        releasedByText.text = "";
        receivedByText.text = "";
        purposeText.text = "";
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

        item.gameObject.SetActive(true);

        mItemsActive.Add(item);
    }
}

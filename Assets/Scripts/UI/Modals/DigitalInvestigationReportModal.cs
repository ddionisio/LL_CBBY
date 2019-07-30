using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigitalInvestigationReportModal : M8.ModalController, M8.IModalPush {
    public const string parmDisableProceed = "disableProceed";

    [Header("Data")]
    public string lockedItemTitle = "?????";
    public string lockedItemDesc = "??????????";
    public int flaggedReqCount = 3;

    [Header("Confirm")]
    public string confirmModal = "confirm";
    [M8.Localize]
    public string confirmTitleRef;
    [M8.Localize]
    public string confirmDescRef;

    [Header("Require Message")]
    [M8.Localize]
    public string reqTitleRef;
    [M8.Localize]
    public string reqDescRef;

    [Header("UI")]
    public ChainOfCustodyItemWidget itemTemplate;

    public Text pointsText;

    public Transform contentRoot;
    public ScrollRect scroller;

    public GameObject proceedButtonGO;

    [Header("Signal Invoke")]
    public M8.Signal signalInvokeProceed;

    private ChainOfCustodyItemWidget[] mItems;

    private M8.GenericParams mConfirmParms = new M8.GenericParams();

    public void Proceed() {
        //check flagged count
        var reportItems = GameData.instance.digitalReportFlaggedItems;

        int flaggedCount = 0;

        for(int i = 0; i < reportItems.Length; i++) {
            if(reportItems[i].isFlagged)
                flaggedCount++;
        }

        if(flaggedCount >= flaggedReqCount) {
            //show confirm
            mConfirmParms[ModalConfirm.parmTitleTextRef] = confirmTitleRef;
            mConfirmParms[ModalConfirm.parmDescTextRef] = confirmDescRef;
            mConfirmParms[ModalConfirm.parmCallback] = (System.Action<bool>)OnConfirmProceed;

            M8.ModalManager.main.Open(confirmModal, mConfirmParms);
        }
        else {
            //show message
            MessageModal.Open(M8.Localize.Get(reqTitleRef), M8.Localize.Get(reqDescRef));
        }
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {
        var reportItems = GameData.instance.digitalReportFlaggedItems;

        bool isProceedDisable = false;

        if(parms != null) {
            if(parms.ContainsKey(parmDisableProceed))
                isProceedDisable = parms.GetValue<bool>(parmDisableProceed);
        }

        //setup items
        if(mItems == null) {
            int itemCount = reportItems.Length;
            int malwareCount = 0;

            for(int i = 0; i < itemCount; i++) {
                if(reportItems[i].malwareData)
                    malwareCount++;
            }

            mItems = new ChainOfCustodyItemWidget[reportItems.Length + malwareCount];

            for(int i = 0; i < mItems.Length; i++) {
                var item = Instantiate(itemTemplate);
                item.transform.SetParent(contentRoot, false);
                item.transform.SetAsLastSibling();

                item.gameObject.SetActive(true);

                mItems[i] = item;
            }
        }

        //fill item information
        int curItemWidgetInd = 0;

        for(int i = 0; i < reportItems.Length; i++) {
            var widgetItm = mItems[curItemWidgetInd];
            curItemWidgetInd++;

            var reportItm = reportItems[i];

            var isFlagged = reportItm.isFlagged;

            //show item
            if(isFlagged) {
                widgetItm.idText.text = reportItm.key;
                widgetItm.descText.text = !string.IsNullOrEmpty(reportItm.reportTextRef) ? M8.Localize.Get(reportItm.reportTextRef) : "(EMPTY)";
            }
            else {
                widgetItm.idText.text = lockedItemTitle;
                widgetItm.descText.text = lockedItemDesc;
            }

            //show malware
            if(reportItm.malwareData) {
                widgetItm = mItems[curItemWidgetInd];
                curItemWidgetInd++;

                if(GameData.instance.IsMalwareChecked(reportItm.key)) {
                    widgetItm.idText.text = !string.IsNullOrEmpty(reportItm.malwareData.titleRef) ? M8.Localize.Get(reportItm.malwareData.titleRef) : "(EMPTY)";
                    widgetItm.descText.text = !string.IsNullOrEmpty(reportItm.malwareData.detailRef) ? M8.Localize.Get(reportItm.malwareData.detailRef) : "(EMPTY)";
                }
                else {
                    widgetItm.idText.text = lockedItemTitle;
                    widgetItm.descText.text = lockedItemDesc;
                }
            }
        }

        GameData.instance.UpdateDigitalReportScore();

        //score
        pointsText.text = GameData.instance.digitalReportScore.ToString();

        scroller.normalizedPosition = new Vector2(0f, 1f);

        proceedButtonGO.SetActive(!isProceedDisable);
    }

    void Awake() {
        itemTemplate.gameObject.SetActive(false);
    }

    void OnConfirmProceed(bool isConfirm) {
        if(isConfirm) {
            Close();

            if(signalInvokeProceed)
                signalInvokeProceed.Invoke();
        }
    }
}

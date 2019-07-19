using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkLogModal : M8.ModalController, M8.IModalPush, M8.IModalPop {
    public const string parmIsInspection = "isInspect";

    [System.Serializable]
    public struct IPProp {        
        public string localIP;
        public M8.RangeInt localPortRange;
        public string foreignIP; //if empty, use random
        public M8.RangeInt foreignPortRange;
        [M8.Localize]
        public string stateTextRef; //if empty, use random
        public string protocol;
        public int count;
    }

    [System.Serializable]
    public struct ForeignIPProp {
        public string ipPrepend;
        public M8.RangeInt endRange;

        public string ipText { get { return ipPrepend + endRange.random.ToString(); } }
    }

    [Header("IP Flag Data")]
    public FlaggedItemData ipFlaggedItem;
    public int ipFlaggedPort = 55516;
    [M8.Localize]
    public string ipFlaggedStateTextRef;
    public int ipFlaggedIPPorpIndex = 1; //which index of propRanges will the flagged ip be added after

    [Header("PropData")]
    public IPProp[] propRanges;
    public ForeignIPProp[] propForeignIPs;
    [M8.Localize]
    public string[] propStateTextRefs;

    public M8.RangeInt pidRange;

    [Header("UI")]
    public ItemSelectFlagWidget itemTemplate;

    public Selectable inspectSelectable;

    public Transform contentRoot;
    public ScrollRect scroller;

    public GameObject flagGO;
    public GameObject unflagGO;

    private int mItemFlaggableInd;

    private List<ItemSelectFlagWidget> mItems = new List<ItemSelectFlagWidget>();
    private int mCurIndex = -1;

    private bool mIsInspection;

    public void Flag() {
        if(!mIsInspection)
            return;

        if(mCurIndex == mItemFlaggableInd) {
            ipFlaggedItem.isFlagged = !ipFlaggedItem.isFlagged;
            UpdateSelected();
        }
    }

    void M8.IModalPop.Pop() {
        if(mCurIndex != -1) {
            if(mItems[mCurIndex])
                mItems[mCurIndex].isSelected = false;

            mCurIndex = -1;
        }
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {
        mIsInspection = false;

        if(parms != null) {
            if(parms.ContainsKey(parmIsInspection))
                mIsInspection = parms.GetValue<bool>(parmIsInspection);
        }

        scroller.normalizedPosition = new Vector2(0f, 1f);

        mItems[mItemFlaggableInd].isFlagged = ipFlaggedItem.isFlagged;

        if(mIsInspection) {
            mCurIndex = 0;
            UpdateSelected();

            inspectSelectable.gameObject.SetActive(true);
        }
        else
            inspectSelectable.gameObject.SetActive(false);
    }

    void Awake() {
        itemTemplate.gameObject.SetActive(false);

        int curItemInd = 0;

        //fill in props
        for(int i = 0; i < propRanges.Length; i++) {
            var prop = propRanges[i];

            var proto = prop.protocol;
            var localip = prop.localIP;
            
            for(int j = 0; j < prop.count; j++) {
                var localport = prop.localPortRange.random;

                var foreignip = string.IsNullOrEmpty(prop.foreignIP) ? propForeignIPs[Random.Range(0, propForeignIPs.Length)].ipText : prop.foreignIP;
                var foreignport = prop.foreignPortRange.random;
                var state = M8.Localize.Get(string.IsNullOrEmpty(prop.stateTextRef) ? propStateTextRefs[Random.Range(0, propStateTextRefs.Length)] : prop.stateTextRef);
                var pid = pidRange.random;

                GenerateItem(ref curItemInd, proto, localip, localport, foreignip, foreignport, state, pid, false);
            }

            //insert flagged item
            if(i == ipFlaggedIPPorpIndex) {
                mItemFlaggableInd = curItemInd;

                var localport = prop.localPortRange.random;
                var pid = pidRange.random;

                GenerateItem(ref curItemInd, proto, localip, localport, ipFlaggedItem.key, ipFlaggedPort, M8.Localize.Get(ipFlaggedStateTextRef), pid, true);
            }
        }
    }

    void OnItemClick(int index) {
        if(!mIsInspection)
            return;

        if(mCurIndex != index) {
            if(mCurIndex != -1)
                mItems[mCurIndex].isSelected = false;

            mCurIndex = index;

            UpdateSelected();
        }
    }

    private void GenerateItem(ref int index, string proto, string localIP, int localPort, string foreignIP, int foreignPort, string state, int pid, bool isFlaggable) {
        var itm = Instantiate(itemTemplate);

        itm.transform.SetParent(contentRoot, false);

        itm.Setup(index);
        itm.isFlagged = false;
        itm.isSelected = false;
        itm.clickCallback += OnItemClick;

        var netstatItm = itm.GetComponent<NetStatItemWidget>();

        netstatItm.highlightGO.SetActive(isFlaggable);

        netstatItm.protoText.text = proto;
        netstatItm.localAddrText.text = string.Format("{0}:{1}", localIP, localPort);
        netstatItm.foreignAddrText.text = string.Format("{0}:{1}", foreignIP, foreignPort);
        netstatItm.stateText.text = state;
        netstatItm.pidText.text = pid.ToString();

        itm.gameObject.SetActive(true);

        mItems.Add(itm);

        index++;
    }

    private void UpdateSelected() {
        if(mCurIndex != -1) {
            mItems[mCurIndex].isSelected = true;

            if(mCurIndex == mItemFlaggableInd) {                
                inspectSelectable.interactable = true;

                var isFlagged = ipFlaggedItem.isFlagged;

                mItems[mCurIndex].isFlagged = isFlagged;

                flagGO.SetActive(!isFlagged);
                unflagGO.SetActive(isFlagged);
            }
            else {
                inspectSelectable.interactable = false;

                mItems[mCurIndex].isFlagged = false;

                flagGO.SetActive(true);
                unflagGO.SetActive(false);
            }
        }
    }
}

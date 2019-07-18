using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FileInspectModal : M8.ModalController, M8.IModalPush {
    public const string parmSearchKeywordData = "keyDat";
    public const string parmSearchKeywordResultIndex = "keyResultInd";

    /// <summary>
    /// Which result from the keyword will this file have a flaggable item
    /// </summary>
    [System.Serializable]
    public class FlagData {
        public const int dataLineCharCount = 16;
        public const int dataLineCount = 16;

        public SearchKeywordData sourceKeywordData;
        public int sourceKeywordResultIndex;

        public FlaggedItemData flagData; //flag item reference to be found within data

        public int flagDataIndex { get; private set; } //initialized upon grabbing data

        public string[] data {
            get {
                if(mData == null) {
                    if(flagData)
                        flagDataIndex = Random.Range(dataLineCount / 4, dataLineCount - dataLineCount / 8 + 1);
                    else
                        flagDataIndex = -1;

                    mData = new string[dataLineCount];

                    for(int i = 0; i < dataLineCount; i++) {
                        if(i == flagDataIndex) {
                            string _dat;

                            if(flagData.keyword)
                                _dat = flagData.keyword.key;
                            else
                                _dat = flagData.key;

                            if(_dat.Length < dataLineCharCount) {
                                int sInd = Random.Range(0, dataLineCharCount - _dat.Length);

                                var str = _dat;
                                str.PadLeft(sInd, ' ');
                                str.PadRight(dataLineCharCount - _dat.Length - sInd, ' ');

                                mData[i] = str;
                            }
                            else {
                                mData[i] = _dat.Substring(0, dataLineCharCount); //clip
                            }
                        }
                        else {
                            //random junk
                            var junkCount = Random.Range(dataLineCount / 4, dataLineCount - 4);

                            var sInd = Random.Range(0, dataLineCharCount - junkCount);

                            var str = new System.Text.StringBuilder(dataLineCharCount);
                            str.Append(' ', dataLineCharCount);

                            for(int j = sInd; j < dataLineCharCount; j++)
                                str[j] = (char)Random.Range(1, 250);

                            mData[i] = str.ToString();
                        }
                    }
                }

                return mData;
            }
        }

        private string[] mData;
    }

    public struct ListItemData {
        public ItemSelectFlagWidget itemSelect;
        public StringPairWidget stringPair;

        public ListItemData(ItemSelectFlagWidget aItemSelect) {
            itemSelect = aItemSelect;
            stringPair = itemSelect.GetComponent<StringPairWidget>();
        }
    }

    [Header("Data")]
    public FlagData[] items;

    [Header("UI")]
    public ItemSelectFlagWidget listItemTemplate;
    public Transform listRoot;

    public Text titleText;
    [M8.Localize]
    public string titleTextRef;

    public ScrollRect scroller;

    public Selectable flagger;
        
    public GameObject flagGO;
    public GameObject unflagGO;

    private List<ListItemData> mListItemActive = new List<ListItemData>();
    private List<ListItemData> mListItemCache = new List<ListItemData>();

    private SearchKeywordData mSearchKeywordData;
    private int mKeywordResultIndex;

    private int mFlagDataIndex;

    private int mCurIndex;

    public void Flag() {
        if(mFlagDataIndex != -1) {
            var flagItm = items[mFlagDataIndex];

            if(flagItm.flagData && mCurIndex == flagItm.flagDataIndex)
                flagItm.flagData.isFlagged = !flagItm.flagData.isFlagged;

            UpdateSelectFlag();
        }
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {
        ClearItems();

        mFlagDataIndex = -1;

        mCurIndex = 0;

        mSearchKeywordData = null;
        mKeywordResultIndex = 0;

        if(parms != null) {
            if(parms.ContainsKey(parmSearchKeywordData))
                mSearchKeywordData = parms.GetValue<SearchKeywordData>(parmSearchKeywordData);

            if(parms.ContainsKey(parmSearchKeywordResultIndex))
                mKeywordResultIndex = parms.GetValue<int>(parmSearchKeywordResultIndex);
        }

        //grab which item to use
        FlagData itm = null;

        if(mSearchKeywordData != null) {
            for(int i = 0; i < items.Length; i++) {
                var _itm = items[i];
                if(_itm.sourceKeywordData == mSearchKeywordData && _itm.sourceKeywordResultIndex == mKeywordResultIndex) {
                    itm = _itm;
                    mFlagDataIndex = i;
                    break;
                }
            }
        }

        if(itm != null) {
            //fill list
            var dats = itm.data;

            UpdateSelectFlag();
        }

        scroller.normalizedPosition = Vector2.zero;
    }

    void Awake() {
        listItemTemplate.gameObject.SetActive(false);
    }

    void OnItemClick(int index) {
        if(mCurIndex != index) {

            UpdateSelectFlag();
        }
    }

    private void UpdateSelectFlag() {
        if(mFlagDataIndex != -1) {
            flagger.gameObject.SetActive(true);

            var flagItm = items[mFlagDataIndex];
            if(flagItm.flagData && mCurIndex == flagItm.flagDataIndex) {
                flagger.interactable = true;

                var isFlagged = flagItm.flagData.isFlagged;

                flagGO.SetActive(!isFlagged);
                unflagGO.SetActive(isFlagged);
            }
            else {
                flagger.interactable = false;

                flagGO.SetActive(true);
                unflagGO.SetActive(false);
            }
        }
        else { //fail-safe if not items
            flagger.gameObject.SetActive(false);
        }
    }

    private void ClearItems() {
        for(int i = 0; i < mListItemActive.Count; i++) {
            var itm = mListItemActive[i];

            itm.itemSelect.gameObject.SetActive(false);

            mListItemCache.Add(itm);
        }

        mListItemActive.Clear();
    }

    private void AllocateItem(FlagData flagData, int index) {
        ListItemData itm;

        if(mListItemCache.Count > 0) {
            itm = mListItemCache[mListItemCache.Count - 1];
            mListItemCache.RemoveAt(mListItemCache.Count - 1);
        }
        else {
            var newItemSelect = Instantiate(listItemTemplate);
            itm = new ListItemData(newItemSelect);
        }

        itm.itemSelect.transform.SetParent(listRoot, false);
        itm.itemSelect.transform.SetSiblingIndex(index);

        itm.itemSelect.Setup(index);

        bool isFlaggable = flagData.flagDataIndex == index && flagData.flagData;

        itm.itemSelect.isFlagged = isFlaggable && flagData.flagData.isFlagged;
        itm.itemSelect.isSelected = index == mCurIndex;

        var stringPairWidget = itm.stringPair;

        stringPairWidget.counterText.text = index.ToString("X2");

        //fill body
        var dat = flagData.data[index];

        var sbHex = new System.Text.StringBuilder();
        var sbChars = new System.Text.StringBuilder();

        //fill hex/char values
        for(int i = 0; i < dat.Length; i++) {
            var c = dat[i];

            if(c == ' ') //treat space as null
                sbHex.Append("00");
            else
                sbHex.Append(((int)c).ToString("X2"));

            sbHex.Append(' ');

            sbChars.Append(c);
            sbChars.Append(' ');
        }

        stringPairWidget.stringTextLeft.text = sbHex.ToString();
        stringPairWidget.stringTextRight.text = sbChars.ToString();

        stringPairWidget.highlightGO.SetActive(isFlaggable);

        itm.itemSelect.gameObject.SetActive(true);

        mListItemActive.Add(itm);
    }
}

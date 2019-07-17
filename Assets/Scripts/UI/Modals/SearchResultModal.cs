using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchResultModal : M8.ModalController, M8.IModalPush, M8.IModalPop {
    public const string parmSearchKeywordData = "data";
    public const string parmProceedCallback = "proceedCB";

    public delegate void ProceedCallback(int index);

    [Header("Data")]
    public ItemSelectFlagWidget itemTemplate;

    [Header("UI")]
    public Text titleText;
    [M8.Localize]
    public string titleTextRef;
    public Transform contentRoot;
    public ScrollRect scroller;
    public GameObject flagGO;
    public GameObject unflagGO;

    private List<ItemSelectFlagWidget> mItemActive = new List<ItemSelectFlagWidget>();
    private List<ItemSelectFlagWidget> mItemCache = new List<ItemSelectFlagWidget>();

    private SearchKeywordData mSearchKeywordData;
    private int mCurIndex;

    private ProceedCallback mProceedCallback;

    public void Flag() {
        var results = mSearchKeywordData.results;
        results[mCurIndex].isFlagged = !results[mCurIndex].isFlagged;

        UpdateSelectedItemFlag();

        //callback
    }

    public void Proceed() {
        var cb = mProceedCallback;
        if(cb != null)
            cb(mCurIndex);
    }

    void M8.IModalPop.Pop() {
        mProceedCallback = null;
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {
        ClearItems();

        mCurIndex = 0;
        mProceedCallback = null;

        if(parms != null) {
            if(parms.ContainsKey(parmSearchKeywordData))
                mSearchKeywordData = parms.GetValue<SearchKeywordData>(parmSearchKeywordData);

            if(parms.ContainsKey(parmProceedCallback))
                mProceedCallback = parms.GetValue<ProceedCallback>(parmProceedCallback);
        }

        if(mSearchKeywordData) {
            titleText.text = string.Format(M8.Localize.Get(titleTextRef), mSearchKeywordData.key);

            //populate list
            var results = mSearchKeywordData.results;
            for(int i = 0; i < results.Length; i++) {
                var result = results[i];

                AllocateItem(i, result);
            }

            UpdateSelectedItemFlag();
        }
                
        //init scroller
        scroller.normalizedPosition = Vector2.zero;
    }

    void Awake() {
        itemTemplate.gameObject.SetActive(false);
    }

    void OnItemClick(int index) {
        if(mCurIndex != index) {
            mItemActive[mCurIndex].isSelected = false;

            mCurIndex = index;

            mItemActive[mCurIndex].isSelected = true;

            UpdateSelectedItemFlag();
        }
    }

    private void ClearItems() {
        for(int i = 0; i < mItemActive.Count; i++) {
            var itm = mItemActive[i];
            if(itm) {
                itm.gameObject.SetActive(false);
                mItemCache.Add(itm);
            }
        }

        mItemActive.Clear();
    }

    private ItemSelectFlagWidget AllocateItem(int index, SearchKeywordData.ResultData dat) {
        ItemSelectFlagWidget ret = null;

        if(mItemCache.Count > 0) {
            var cacheInd = mItemCache.Count - 1;

            ret = mItemCache[cacheInd];

            mItemCache.RemoveAt(cacheInd);
        }
        else {
            ret = Instantiate(itemTemplate);
            ret.clickCallback += OnItemClick;
        }

        if(ret) {
            ret.gameObject.SetActive(true);
            ret.transform.SetParent(contentRoot, false);
            ret.transform.SetSiblingIndex(index);

            ret.Setup(index);
            ret.isSelected = index == mCurIndex;
            ret.isFlagged = dat.isFlagged;

            mItemActive.Add(ret);
        }

        return ret;
    }

    private void UpdateSelectedItemFlag() {
        var results = mSearchKeywordData.results;

        var isFlagged = results[mCurIndex].isFlagged;

        mItemActive[mCurIndex].isFlagged = isFlagged;

        flagGO.SetActive(!isFlagged);
        unflagGO.SetActive(isFlagged);
    }
}

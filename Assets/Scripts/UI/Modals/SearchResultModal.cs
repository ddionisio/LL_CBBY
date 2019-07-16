using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchResultModal : M8.ModalController, M8.IModalPush {
    public const string parmSearchKeywordData = "data";

    [Header("Data")]
    public ItemSelectFlagWidget itemTemplate;

    [Header("UI")]
    public Text titleText;
    [M8.Localize]
    public string titleTextRef;
    public Transform contentRoot;
    public ScrollRect scroller;

    private List<ItemSelectFlagWidget> mItemActive = new List<ItemSelectFlagWidget>();
    private List<ItemSelectFlagWidget> mItemCache = new List<ItemSelectFlagWidget>();

    private SearchKeywordData mSearchKeywordData;
    private int mCurIndex;

    public void Flag() {

    }

    public void Proceed() {

    }

    void M8.IModalPush.Push(M8.GenericParams parms) {
        ClearItems();

        mCurIndex = 0;

        if(parms != null) {
            if(parms.ContainsKey(parmSearchKeywordData))
                mSearchKeywordData = parms.GetValue<SearchKeywordData>(parmSearchKeywordData);
        }

        if(mSearchKeywordData) {
            titleText.text = string.Format(M8.Localize.Get(titleTextRef), mSearchKeywordData.key);

            //populate list
            var results = mSearchKeywordData.results;
            for(int i = 0; i < results.Length; i++) {
                var result = results[i];

                AllocateItem(i, result);
            }
        }
                
        //init scroller
        scroller.normalizedPosition = Vector2.zero;
    }

    void Awake() {
        itemTemplate.gameObject.SetActive(false);
    }

    void OnItemClick(int index) {
        mItemActive[mCurIndex].isSelected = false;

        mCurIndex = index;

        mItemActive[mCurIndex].isSelected = true;
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
}

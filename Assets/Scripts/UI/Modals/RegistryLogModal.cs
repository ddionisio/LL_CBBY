using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistryLogModal : M8.ModalController, M8.IModalPush, M8.IModalPop {
    [Header("Data")]
    public FlaggedItemData flaggedData;

    [Header("UI")]
    public Transform contentRoot;
    public ScrollRect scroller;

    public Selectable flagger;

    public GameObject flagGO;
    public GameObject unflagGO;

    private int mFlaggableInd;
    private int mCurInd = -1;

    private ItemSelectFlagWidget[] mItems;

    public void Flag() {
        if(mCurInd == mFlaggableInd) {
            flaggedData.isFlagged = !flaggedData.isFlagged;

            mItems[mFlaggableInd].isFlagged = flaggedData.isFlagged;

            UpdateSelectedItemFlag();
        }
    }

    void M8.IModalPop.Pop() {
        if(mItems != null && mCurInd != -1) {
            mItems[mCurInd].isSelected = false;
        }
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {

        scroller.normalizedPosition = new Vector2(0f, 1f);

        mCurInd = 0;
        mItems[mCurInd].isSelected = true;

        mItems[mFlaggableInd].isFlagged = flaggedData.isFlagged;

        UpdateSelectedItemFlag();
    }

    void Awake() {
        mItems = contentRoot.GetComponentsInChildren<ItemSelectFlagWidget>();

        //initialize items
        for(int i = 0; i < mItems.Length; i++) {
            var itm = mItems[i];

            itm.Setup(i);
            itm.isSelected = false;

            if(itm.isFlagged)
                mFlaggableInd = i;

            itm.isFlagged = false;

            itm.clickCallback += OnItemClick;
        }
    }

    void OnItemClick(int index) {
        if(mCurInd != index) {
            mItems[mCurInd].isSelected = false;

            mCurInd = index;

            mItems[mCurInd].isSelected = true;

            UpdateSelectedItemFlag();
        }
    }

    private void UpdateSelectedItemFlag() {
        if(mCurInd == mFlaggableInd) {
            flagger.interactable = true;

            var isFlagged = flaggedData.isFlagged;

            flagGO.SetActive(!isFlagged);
            unflagGO.SetActive(isFlagged);
        }
        else {
            flagger.interactable = false;

            flagGO.SetActive(true);
            unflagGO.SetActive(false);
        }
    }
}
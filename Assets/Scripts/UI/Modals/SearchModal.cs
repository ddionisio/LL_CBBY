using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SearchModal : M8.ModalController, M8.IModalPush, M8.IModalPop {
    public const string parmTitle = "title";
    public const string parmCallback = "cb";

    public delegate void CommitCallback(SearchKeywordData keyword);

    [Header("Data")]
    [M8.Localize]
    public string titleFormatRef;

    [Header("UI")]
    public Text titleText;
    public Dropdown dropdown;
        
    private List<SearchKeywordData> mSearchKeywords;
    private CommitCallback mCommitCallback;

    public void Commit() {
        var ind = dropdown.value;

        var cb = mCommitCallback;

        Close();

        if(mSearchKeywords.Count > 0) {
            if(cb != null)
                cb(mSearchKeywords[ind]);
        }
    }

    void M8.IModalPop.Pop() {
        mCommitCallback = null;
    }

    void M8.IModalPush.Push(M8.GenericParams parms) {
        //set defaults
        titleText.text = "";

        mCommitCallback = null;

        if(parms != null) {
            if(parms.ContainsKey(parmTitle))
                titleText.text = string.Format(M8.Localize.Get(titleFormatRef), parms.GetValue<string>(parmTitle));

            if(parms.ContainsKey(parmCallback))
                mCommitCallback = parms.GetValue<CommitCallback>(parmCallback);
        }

        //fill up search items from GameData.searchKeywords
        mSearchKeywords = new List<SearchKeywordData>(GameData.instance.searchKeywords);
        //mSearchKeywords.Sort();

        var options = new List<string>(mSearchKeywords.Count);
        for(int i = 0; i < mSearchKeywords.Count; i++)
            options.Add(mSearchKeywords[i].key);

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        //
    }
}

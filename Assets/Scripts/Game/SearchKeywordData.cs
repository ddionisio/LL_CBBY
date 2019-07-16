using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "searchKeywordData", menuName = "Game/Search Keyword Data")]
public class SearchKeywordData : ScriptableObject, IComparer, IComparer<SearchKeywordData> {
    [System.Serializable]
    public class ResultData {
        public string text;
        public FlaggedItemData flagData;

        public bool isFlagged {
            get { return flagData ? flagData.isFlagged : mIsFlagged; }
            set {
                if(flagData)
                    flagData.isFlagged = value;
                else
                    mIsFlagged = value;
            }
        }

        private bool mIsFlagged;
    }

    public string key;
    public SearchType resultType;
    public ResultData[] results;

    public int Compare(SearchKeywordData x, SearchKeywordData y) {
        return x.key.CompareTo(y);
    }

    int IComparer.Compare(object x, object y) {
        return Compare((SearchKeywordData)x, (SearchKeywordData)y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "searchKeywordData", menuName = "Game/Search Keyword Data")]
public class SearchKeywordData : ScriptableObject, IComparer, IComparer<SearchKeywordData> {
    [System.Serializable]
    public class ResultData {
        public string text;
        public FlaggedItemData flagData;
        public SearchType[] searchTypes;

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

        public bool IsSearchMatch(SearchType searchType) {
            for(int i = 0; i < searchTypes.Length; i++) {
                if(searchTypes[i] == searchType)
                    return true;
            }

            return false;
        }
    }

    public string key;    
    public ResultData[] results;

    public bool CheckResultSearch(SearchType s) {
        for(int i = 0; i < results.Length; i++) {
            if(results[i].IsSearchMatch(s))
                return true;
        }

        return false;
    }

    public int Compare(SearchKeywordData x, SearchKeywordData y) {
        return x.key.CompareTo(y);
    }

    int IComparer.Compare(object x, object y) {
        return Compare((SearchKeywordData)x, (SearchKeywordData)y);
    }
}

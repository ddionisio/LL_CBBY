using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "searchKeywordData", menuName = "Game/Search Keyword Data")]
public class SearchKeywordData : ScriptableObject {    
    public string key;
    public SearchType resultType;
    public string[] results;
}

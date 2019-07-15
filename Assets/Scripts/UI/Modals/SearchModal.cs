using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchModal : M8.ModalController, M8.IModalPush {
    public const string parmSearchType = "searchType";

    private SearchType mSearchType;

    void M8.IModalPush.Push(M8.GenericParams parms) {
        //fill up search items from GameData.searchKeywords
    }
}

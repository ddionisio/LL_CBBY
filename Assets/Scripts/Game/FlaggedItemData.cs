using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "flaggedItemData", menuName = "Game/Flagged Item Data")]
public class FlaggedItemData : ScriptableObject {
    public string key; //use this if keyword is null
    public SearchKeywordData keyword; //when flagged, keyword is added to list

    public bool isFlagged { get; set; }
}

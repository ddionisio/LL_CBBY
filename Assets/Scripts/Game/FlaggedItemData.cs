using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "flaggedItemData", menuName = "Game/Flagged Item Data")]
public class FlaggedItemData : ScriptableObject {
    
    public bool isFlagged { get; set; }
}

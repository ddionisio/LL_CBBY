using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "acquisitionItem", menuName = "Game/Acquisition Item Data")]
public class AcquisitionItemData : ScriptableObject {
    public bool isValid; //if true, this is a valid device acquisition
}

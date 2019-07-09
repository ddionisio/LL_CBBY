using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolatileSuiteItemWidget : MonoBehaviour {
    [Header("Data")]
    public GameData.VolatileType volatileType;

    [Header("Display")]
    public GameObject checkedGO;

    public void Click() {
        GameData.instance.VolatileAcquire(volatileType);

        if(checkedGO)
            checkedGO.SetActive(true);

        GameData.instance.VolatileOpenModal(volatileType);
    }

    void OnEnable() {
        if(checkedGO)
            checkedGO.SetActive(GameData.instance.volatileAcquisitions.Contains(volatileType));
    }
}

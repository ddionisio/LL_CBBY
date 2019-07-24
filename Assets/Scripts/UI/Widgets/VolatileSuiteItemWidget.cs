using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolatileSuiteItemWidget : MonoBehaviour {
    public const float progressDelay = 2f;

    [Header("Data")]
    public GameData.VolatileType volatileType;

    [Header("Display")]
    public GameObject checkedGO;

    public void Click() {
        if(checkedGO.activeSelf) {
            GameData.instance.VolatileOpenModal(volatileType);
        }
        else {
            GameData.instance.VolatileAcquire(volatileType);

            checkedGO.SetActive(true);

            StartCoroutine(DoProgress());
        }
    }

    void OnEnable() {
        checkedGO.SetActive(GameData.instance.volatileAcquisitions.Contains(volatileType));
    }

    IEnumerator DoProgress() {
        var title = string.Format(M8.Localize.Get(GameData.instance.volatileAcquireFormatRef), GameData.instance.GetVolatileTypeText(volatileType));

        var parms = new M8.GenericParams();
        parms[ProgressModal.parmTitleString] = title;
        parms[ProgressModal.parmDelay] = progressDelay;

        M8.ModalManager.main.Open(GameData.instance.modalProgress, parms);

        while(M8.ModalManager.main.isBusy || M8.ModalManager.main.IsInStack(GameData.instance.modalProgress))
            yield return null;

        GameData.instance.VolatileOpenModal(volatileType);
    }
}

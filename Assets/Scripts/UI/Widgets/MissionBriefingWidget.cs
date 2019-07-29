using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionBriefingWidget : MonoBehaviour {
    [Header("Data")]
    [M8.Localize]
    public string otherAgentTextRef;
    [M8.Localize]
    public string contentTextRef;

    [Header("UI")]
    public Text contentText;
    public ScrollRect scroller;

    void OnEnable() {
        var caseNumberString = GameData.instance.caseNumber.ToString();
        var departmentString = M8.Localize.Get(GameData.instance.departmentNameTextRef);

        var dateDat = System.DateTime.Now;
        var dateString = dateDat.ToString("g", System.Globalization.DateTimeFormatInfo.InvariantInfo);

        var playerNameString = GameData.instance.playerName;

        var otherAgentNameString = M8.Localize.Get(otherAgentTextRef);

        contentText.text = string.Format(M8.Localize.Get(contentTextRef), caseNumberString, departmentString, dateString, playerNameString, otherAgentNameString);

        scroller.normalizedPosition = new Vector2(0f, 1f);
    }
}

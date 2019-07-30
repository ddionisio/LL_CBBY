using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalScoreWidget : MonoBehaviour {
    [M8.Localize]
    public string formatRef;

    public Text text;

    void OnEnable() {
        var score = LoLManager.instance.curScore;
        var totalScore = GameData.instance.GetFullScore();

        text.text = string.Format(M8.Localize.Get(formatRef), score, totalScore);
    }
}

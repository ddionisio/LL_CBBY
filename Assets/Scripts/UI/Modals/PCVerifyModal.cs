using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PCVerifyModal : M8.ModalController, M8.IModalPush {
    [System.Serializable]
    public class PointsItem {
        public Text text;
        public GameObject correctGO;
        public GameObject wrongGO;

        public void Apply(int score) {
            var isCorrect = score > 0;

            text.text = score.ToString();

            correctGO.SetActive(isCorrect);
            wrongGO.SetActive(!isCorrect);
        }
    }

    [Header("UI")]
    public PointsItem[] points;

    public Text pointsText;

    void M8.IModalPush.Push(M8.GenericParams parms) {

        GameData.instance.UpdatePCVerifyScore();

        var pts = GameData.instance.pcVerifyScoreValue;

        points[0].Apply(GameData.instance.isNetworkDisconnect ? pts : 0);
        points[1].Apply(GameData.instance.isMonitorAwake ? pts : 0);
        points[2].Apply(GameData.instance.captureScreenTexture && GameData.instance.captureScreenIsMonitorAwake ? pts : 0);
                
        pointsText.text = GameData.instance.pcVerifyScore.ToString();
    }
}

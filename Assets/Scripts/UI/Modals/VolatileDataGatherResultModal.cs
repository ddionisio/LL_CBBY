using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolatileDataGatherResultModal : M8.ModalController, M8.IModalPush {
    [Header("UI")]
    public Text recommendedOrderText;
    public Text playerOrderText;
    public Text playerOrderScoreText;
    public Text scoreText;

    void M8.IModalPush.Push(M8.GenericParams parms) {

        GameData.instance.UpdateVolatileScore();

        //setup recommend order
        var orderTextSB = new System.Text.StringBuilder();

        for(int i = 0; i < GameData.instance.volatileOrder.Length; i++) {
            var orders = GameData.instance.volatileOrder[i];

            orderTextSB.Append((i + 1).ToString()).Append(" - ");

            for(int j = 0; j < orders.volatiles.Length; j++) {
                orderTextSB.Append(GameData.instance.GetVolatileTypeText(orders.volatiles[j]));
                if(j < orders.volatiles.Length - 1)
                    orderTextSB.Append(", ");
            }

            orderTextSB.Append("\n\n");
        }

        recommendedOrderText.text = orderTextSB.ToString();

        //setup player order/score
        var playerOrderTextSB = new System.Text.StringBuilder();
        var playerOrderScoresTextSB = new System.Text.StringBuilder();

        for(int i = 0; i < GameData.instance.volatileAcquisitions.Count; i++) {
            var volatileAcq = GameData.instance.volatileAcquisitions[i];

            playerOrderTextSB.Append((i + 1).ToString()).Append(" - ");
            playerOrderTextSB.Append(GameData.instance.GetVolatileTypeText(volatileAcq.type));
            playerOrderTextSB.Append("\n\n");

            playerOrderScoresTextSB.Append(volatileAcq.score);
            playerOrderScoresTextSB.Append("\n\n");
        }

        playerOrderText.text = playerOrderTextSB.ToString();

        playerOrderScoreText.text = playerOrderScoresTextSB.ToString();

        scoreText.text = GameData.instance.volatileScore.ToString();
    }
}

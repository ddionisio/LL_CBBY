using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivityItemWidget : MonoBehaviour {
    public Text timeText;
    public Text detailText;

    public void Apply(GameData.ActivityLogItem activityLogItem) {
        timeText.text = activityLogItem.GetTimeString();
        detailText.text = M8.Localize.Get(activityLogItem.detailTextRef);
    }
}

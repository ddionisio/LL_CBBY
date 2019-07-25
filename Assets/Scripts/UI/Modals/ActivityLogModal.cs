using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivityLogModal : M8.ModalController, M8.IModalPush {
    [Header("UI")]
    public ActivityItemWidget itemTemplate;

    public Transform contentRoot;
    public ScrollRect scroller;

    private List<ActivityItemWidget> mItemWidgets = new List<ActivityItemWidget>();

    void M8.IModalPush.Push(M8.GenericParams parms) {

        //check if we need to fill in new items
        var activityLogs = GameData.instance.activityLogs;

        //NOTE: assume we only add more activity logs 
        if(mItemWidgets.Count < activityLogs.Count) {
            int count = activityLogs.Count - mItemWidgets.Count;
            for(int i = 0; i < count; i++) {
                int logInd = mItemWidgets.Count;
                var activityLog = activityLogs[logInd];

                //add new item at the top
                var newItemWidget = Instantiate(itemTemplate);

                newItemWidget.transform.SetParent(contentRoot, false);
                newItemWidget.transform.SetAsFirstSibling();

                newItemWidget.Apply(activityLog);

                newItemWidget.gameObject.SetActive(true);

                mItemWidgets.Add(newItemWidget);
            }
        }

        scroller.normalizedPosition = new Vector2(0f, 1f);
    }

    void Awake() {
        itemTemplate.gameObject.SetActive(false);
    }
}

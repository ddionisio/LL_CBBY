using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "flaggedItemData", menuName = "Game/Flagged Item Data")]
public class FlaggedItemData : ScriptableObject {
    public string key; //use this if keyword is null
    [M8.Localize]
    public string reportTextRef; //use during reporting
    public SearchKeywordData keyword; //when flagged, keyword is added to list

    public MalwareData malwareData;

    public bool isFlagged {
        get { return M8.SceneState.instance.global.GetValue(sceneVarKey) != 0; }
        set {
            if(isFlagged != value) {
                M8.SceneState.instance.global.SetValue(sceneVarKey, value ? 1 : 0, false);

                if(keyword) {
                    var searchKeywords = GameData.instance.searchKeywords;

                    if(value) {
                        if(!searchKeywords.Contains(keyword))
                            searchKeywords.Add(keyword);
                    }
                    else
                        searchKeywords.Remove(keyword);
                }
            }
        }
    }

    private string sceneVarKey {
        get {
            if(string.IsNullOrEmpty(mSceneVarKey))
                mSceneVarKey = "flagged_" + key;

            return mSceneVarKey;
        }
    }

    private string mSceneVarKey;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "gameData", menuName = "Game/Game Data", order = 0)]
public class GameData : M8.SingletonScriptableObject<GameData> {
    public const string sceneVarPlayerName = "playerName";

    public string playerName {
        get {
            if(string.IsNullOrEmpty(mCurPlayerName))
                InitPlayerName();

            return mCurPlayerName;
        }
    }

    public string playerInitial {
        get {
            if(string.IsNullOrEmpty(mCurPlayerInitial))
                InitPlayerName();

            return mCurPlayerInitial;
        }
    }

    private string mCurPlayerName;
    private string mCurPlayerInitial;

    public void SetPlayerName(string aPlayerName) {
        mCurPlayerName = aPlayerName;

        M8.SceneState.instance.global.SetValueString(sceneVarPlayerName, mCurPlayerName, true);

        mCurPlayerInitial = GenerateInitial(mCurPlayerName);
    }

    public string GenerateInitial(string aName) {
        if(!string.IsNullOrEmpty(aName)) {
            var words = aName.Split(' ');
            if(words.Length > 1) {
                var sb = new System.Text.StringBuilder(2);

                var firstWord = words[0];
                var lastWord = words[words.Length - 1];

                if(!string.IsNullOrEmpty(firstWord))
                    sb.Append(firstWord[0]);

                if(!string.IsNullOrEmpty(lastWord))
                    sb.Append(lastWord[0]);

                return sb.ToString().ToUpper();
            }
            else if(!string.IsNullOrEmpty(words[0]))
                return words[0].Substring(0, 1).ToUpper();
        }

        return "";
    }

    private void InitPlayerName() {
        mCurPlayerName = M8.SceneState.instance.global.GetValueString(sceneVarPlayerName);
        mCurPlayerInitial = GenerateInitial(mCurPlayerName);
    }
}

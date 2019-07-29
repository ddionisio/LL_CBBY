using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class HelpSetState : FsmStateAction {
        public GameData.HelpState helpState;

        public override void Reset() {
            helpState = GameData.HelpState.None;
        }

        public override void OnEnter() {
            GameData.instance.helpState = helpState;
            Finish();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class SetInteractiveMode : FsmStateAction {
        public InteractiveMode mode;

        public override void OnEnter() {
            GameData.instance.currentInteractMode = mode;

            Finish();
        }
    }
}
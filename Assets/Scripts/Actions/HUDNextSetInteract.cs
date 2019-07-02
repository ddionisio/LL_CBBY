using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class HUDNextSetInteract : FsmStateAction {
        public FsmBool isInteractive;

        public override void Reset() {
            isInteractive = false;
        }

        public override void OnEnter() {
            HUD.instance.SetNextInteractive(isInteractive.Value);
            Finish();
        }
    }
}
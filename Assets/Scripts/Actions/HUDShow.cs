using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class HUDShow : FsmStateAction {
        public override void OnEnter() {
            if(!HUD.instance.isVisible)
                HUD.instance.Show();
            else
                Finish();
        }

        public override void OnUpdate() {
            if(!HUD.instance.isBusy)
                Finish();
        }
    }
}
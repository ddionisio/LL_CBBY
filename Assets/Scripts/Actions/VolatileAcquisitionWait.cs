using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    [Tooltip("Check if all volatiles are acquired.")]
    public class VolatileAcquisitionWait : FsmStateAction {
        public override void OnEnter() {
            
        }

        public override void OnUpdate() {
            var volatileList = GameData.instance.volatileAcquisitions;
            if(volatileList.Count >= (int)GameData.VolatileType.Count)
                Finish();
        }
    }
}
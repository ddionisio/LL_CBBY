using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class GetCameraCaptureMax : FsmStateAction {
        [UIHint(UIHint.Variable)]
        public FsmInt output;

        public override void Reset() {
            output = null;
        }

        public override void OnEnter() {
            output.Value = GameData.instance.captureCount;
        }
    }
}
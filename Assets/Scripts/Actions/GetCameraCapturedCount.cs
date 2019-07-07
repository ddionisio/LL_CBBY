using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class GetCameraCapturedCount : FsmStateAction {
        [UIHint(UIHint.Variable)]
        public FsmInt output;

        public override void Reset() {
            output = null;
        }

        public override void OnEnter() {
            int count = 0;

            var captureInfs = GameData.instance.captureInfos;
            for(int i = 0; i < captureInfs.Length; i++) {
                var inf = captureInfs[i];
                if(inf.texture)
                    count++;
            }

            output.Value = count;

            Finish();
        }
    }
}
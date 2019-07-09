using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class InteractiveObjectHideDisplay : ComponentAction<InteractiveObject> {
        [RequiredField]
        [CheckForComponent(typeof(InteractiveObject))]
        public FsmOwnerDefault gameObject;

        public override void OnEnter() {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if(UpdateCache(go))
                cachedComponent.HideHighlightDisplays();

            Finish();
        }
    }
}
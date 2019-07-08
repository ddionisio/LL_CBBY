using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class InteractiveObjectSetLock : ComponentAction<InteractiveObject> {
        [RequiredField]
        [CheckForComponent(typeof(InteractiveObject))]
        public FsmOwnerDefault gameObject;

        public FsmBool isLock;

        public override void Reset() {
            gameObject = null;
            isLock = false;
        }

        public override void OnEnter() {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if(UpdateCache(go))
                cachedComponent.isLocked = isLock.Value;

            Finish();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class AcquireDevice : ComponentAction<AcquireController> {
        [RequiredField]
        [CheckForComponent(typeof(AcquireController))]
        public FsmOwnerDefault gameObject;

        [RequiredField]
        [ObjectType(typeof(AcquireController.Type))]
        public FsmEnum acquisitionType;

        public override void OnEnter() {
            var go = Fsm.GetOwnerDefaultTarget(gameObject);
            if(UpdateCache(go)) {
                cachedComponent.Acquire((AcquireController.Type)acquisitionType.Value);
            }

            Finish();
        }
    }
}
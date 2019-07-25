using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M8;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class ActivityLogAdd : FsmStateAction {
        public M8.FsmLocalize detail;

        public FsmBool isWaitClose;

        private ActivityLogPopUpWidget mActivityLogPopUpWidget;

        public override void Reset() {
            detail = null;
            isWaitClose = false;
        }

        public override void OnEnter() {
            if(!isWaitClose.Value)
                Finish();
        }

        public override void OnUpdate() {
            if(!mActivityLogPopUpWidget)
                mActivityLogPopUpWidget = GameData.instance.GetActivityLogPopUpWidget();

            if(!(mActivityLogPopUpWidget && mActivityLogPopUpWidget.isRunning))
                Finish();
        }
    }
}
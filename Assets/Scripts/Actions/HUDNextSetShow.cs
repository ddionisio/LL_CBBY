﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class HUDNextSetShow : FsmStateAction {
        public FsmBool isShow;
        public FsmBool isShowIndicator;

        public override void Reset() {
            isShow = false;
            isShowIndicator = false;
        }

        public override void OnEnter() {
            HUD.instance.NextSetShow(isShow.Value, isShowIndicator.Value);
            Finish();
        }
    }
}
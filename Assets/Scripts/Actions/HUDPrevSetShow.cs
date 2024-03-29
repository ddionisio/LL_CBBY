﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class HUDPrevSetShow : FsmStateAction {
        public FsmBool isShow;

        public override void Reset() {
            isShow = false;
        }

        public override void OnEnter() {
            HUD.instance.PrevSetShow(isShow.Value);
            Finish();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class CameraChangePosition : FsmStateAction {
        public FsmGameObject target;
        public FsmFloat delay;
        public FsmBool applyOrientation;
        public DG.Tweening.Ease easeType;

        private Camera mCamera;
        private DG.Tweening.EaseFunction mEaseFunc;
        private float mCurTime;

        private Vector3 mStartPos;
        private Quaternion mStartRot;

        public override void Reset() {
            target = null;
            delay = 0.3f;
            applyOrientation = true;
            easeType = DG.Tweening.Ease.InOutSine;
        }

        public override void OnEnter() {
            if(!mCamera)
                mCamera = Camera.main;

            if(target.Value) {
                mEaseFunc = DG.Tweening.Core.Easing.EaseManager.ToEaseFunction(easeType);
                mCurTime = 0f;

                mStartPos = mCamera.transform.position;
                mStartRot = mCamera.transform.rotation;
            }
            else
                Finish();
        }

        public override void OnUpdate() {
            var targetTrans = target.Value ? target.Value.transform : null;
            var _delay = delay.Value;

            if(targetTrans && mCurTime < _delay) {
                mCurTime += Time.deltaTime;

                var t = mEaseFunc(mCurTime, _delay, 0f, 0f);

                var camTrans = mCamera.transform;

                camTrans.position = Vector3.Lerp(mStartPos, targetTrans.position, t);

                if(applyOrientation.Value)
                    camTrans.rotation = Quaternion.Lerp(mStartRot, targetTrans.rotation, t);
            }
            else
                Finish();
        }
    }
}
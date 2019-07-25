using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using M8;

namespace HutongGames.PlayMaker.Actions.Game {
    [ActionCategory("Game")]
    public class ChainOfCustodyUpdate : FsmStateAction {
        public M8.FsmLocalize releasedBy;
        public FsmBool releasedByIsPlayer;

        public M8.FsmLocalize receivedBy;
        public FsmBool receivedByIsPlayer;

        public M8.FsmLocalize purpose;

        public AcquisitionItemData[] items;
        public FsmBool isAllItems;

        public FsmBool isWaitModalClose;

        private GenericParams mParms = new GenericParams();

        public override void Reset() {
            releasedBy = null;
            releasedByIsPlayer = false;

            receivedBy = null;
            receivedByIsPlayer = false;

            purpose = null;

            items = new AcquisitionItemData[0];
            isAllItems = false;

            isWaitModalClose = false;
        }

        public override void OnEnter() {
            mParms[ChainOfCustodyModal.parmDateApply] = true;
            mParms[ChainOfCustodyModal.parmReleasedByString] = releasedByIsPlayer.Value ? GameData.instance.playerName : releasedBy.GetString();
            mParms[ChainOfCustodyModal.parmReceivedByString] = receivedByIsPlayer.Value ? GameData.instance.playerName : receivedBy.GetString();
            mParms[ChainOfCustodyModal.parmPurposeString] = purpose.GetString();

            if(isAllItems.Value) {
                mParms[ChainOfCustodyModal.parmItems] = GameData.instance.deviceAcquisitions.ToArray();
            }
            else if(items != null && items.Length > 0) {
                var acqs = GameData.instance.GetAcquisitions(items);
                mParms[ChainOfCustodyModal.parmItems] = acqs;
            }

            ModalManager.main.Open(GameData.instance.modalChainOfCustody, mParms);

            if(!isWaitModalClose.Value)
                Finish();
        }

        public override void OnUpdate() {
            var modalMgr = ModalManager.main;
            if(!modalMgr.isBusy && !modalMgr.IsInStack(GameData.instance.modalChainOfCustody))
                Finish();
        }
    }
}
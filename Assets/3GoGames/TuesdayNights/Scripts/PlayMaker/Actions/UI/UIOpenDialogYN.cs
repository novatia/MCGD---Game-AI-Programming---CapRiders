using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Open a modal dialog.")]
    public class UIOpenDialogYN : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Dialog message.")]
        public FsmString messageText;
        [Tooltip("Send this event if user clicks YES.")]
        public FsmEvent yesEvent;
        [Tooltip("Send this event if user clicks NO.")]
        public FsmEvent noEvent;

        public override void OnEnter()
        {
            if (ModalPanelHandle.handleMain != null)
            {
                ModalPanelDetails modalPanelDetails = new ModalPanelDetails
                {
                    message = messageText.Value
                };

                modalPanelDetails.button1Details = new EventButtonDetails
                {
                    buttonTitle = "Yes",
                    action = OnClickYes
                };

                modalPanelDetails.button2Details = new EventButtonDetails
                {
                    buttonTitle = "No",
                    action = OnClickNo
                };

                ModalPanelHandle.handleMain.OpenPanel(modalPanelDetails);
            }

            Finish();
        }

        void OnClickYes()
        {
            if (yesEvent != null)
            {
                Fsm.Event(yesEvent);
            }
        }

        void OnClickNo()
        {
            if (noEvent != null)
            {
                Fsm.Event(noEvent);
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("3Go - UI")]
    [Tooltip("Open a modal dialog.")]
    public class UIOpenDialog : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Dialog message.")]
        public FsmString messageText;
        [Tooltip("Send this event if user clicks OK.")]
        public FsmEvent okEvent;

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
                    buttonTitle = "OK",
                    action = OnClickOk
                };

                ModalPanelHandle.handleMain.OpenPanel(modalPanelDetails);
            }

            Finish();
        }

        void OnClickOk()
        {
            if (okEvent != null)
            {
                Fsm.Event(okEvent);
            }
        }
    }
}
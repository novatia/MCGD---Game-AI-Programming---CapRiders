using System;
using System.Globalization;

using TuesdayNights;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("TuesdayNights")]
    [Tooltip("Is current date valid?")]
    public class tnIsDateValid : FsmStateAction
    {
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [Tooltip("Date to compare.")]
        public FsmString date;

        [Tooltip("Event to send if the date is valid.")]
        public FsmEvent isTrue;

        [Tooltip("Event to send if the date is invalid.")]
        public FsmEvent isFalse;

        [Tooltip("Event to send if the given string is not a well-formatted date.")]
        public FsmEvent parseFailed;

        public override void Reset()
        {
            date = null;

            isTrue = null;
            isFalse = null;
        }

        public override void OnEnter()
        {
            CultureInfo culture;
            DateTimeStyles styles;

            culture = CultureInfo.CreateSpecificCulture("en-US");
            styles = DateTimeStyles.None;

            DateTime timeLimit;
            if (DateTime.TryParse(date.Value, culture, styles, out timeLimit))
            {
                DateTime now = DateTime.Now;
                
                if (now < timeLimit)
                {
                    Fsm.Event(isTrue);
                }
                else
                {
                    Fsm.Event(isFalse);
                }
            }
            else
            {
                Fsm.Event(parseFailed);
            }

            Finish();
        }
    }
}
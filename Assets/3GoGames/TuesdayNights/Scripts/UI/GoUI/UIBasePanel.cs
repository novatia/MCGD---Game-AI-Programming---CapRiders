using UnityEngine;

using System;

namespace GoUI
{
    public abstract class UIBasePanel : MonoBehaviour
    {
        public abstract bool isOpen { get; }
        public abstract bool isClose { get; }
        public abstract bool isViewTotallyOpen { get; }
        public abstract bool isViewTotallyClose { get; }
        public abstract bool isViewOpening { get; }
        public abstract bool isViewClosing { get; }

        public abstract void Open(Action i_Callback = null);
        public abstract void Close(Action i_Callback = null);
    }
}
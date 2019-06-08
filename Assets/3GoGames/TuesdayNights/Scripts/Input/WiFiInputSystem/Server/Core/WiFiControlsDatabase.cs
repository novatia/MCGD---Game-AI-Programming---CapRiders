using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

using WiFiInput.Common;

namespace WiFiInput.Server
{
    [Serializable]
    public class WiFiControlDescriptor
    {
        [SerializeField]
        private string m_ControlName = "";
        [SerializeField]
        private ControlType m_Type = ControlType.Button;

        public string controlName
        {
            get { return m_ControlName; }
        }

        public ControlType type
        {
            get { return m_Type; }
        }
    }

    public class WiFiControlsDatabase : BaseScriptableObject
    {
        [SerializeField]
        private List<WiFiControlDescriptor> m_Controls = new List<WiFiControlDescriptor>();

        public int count
        {
            get { return m_Controls.Count; }
        }

        public WiFiControlDescriptor GetControl(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_Controls.Count)
            {
                return null;
            }

            return m_Controls[i_Index];
        }
    }
}

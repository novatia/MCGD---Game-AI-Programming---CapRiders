using UnityEngine;

using System.Collections.Generic;

using FullInspector;

namespace WiFiInput.Server
{
    public class WiFiActionsDatabase : BaseScriptableObject
    {
        [SerializeField]
        private Dictionary<string, string> m_ActionsMap = new Dictionary<string, string>();

        public int count
        {
            get { return m_ActionsMap.Count; }
        }

        public Dictionary<string, string>.KeyCollection keys
        {
            get { return m_ActionsMap.Keys; }
        }

        public bool GetButton(string i_ActionId, out string o_ControlName)
        {
            return m_ActionsMap.TryGetValue(i_ActionId, out o_ControlName);
        }
    }
}

using UnityEngine;

using System;
using System.Collections.Generic;

using FullInspector;

namespace WiFiInput.Server
{
    [Serializable]
    public class WiFiPlayerDescriptor
    {
        [SerializeField]
        private string m_Name = "";

        public string name
        {
            get { return m_Name; }
        }
    }

    public class WiFiPlayersDatabase : BaseScriptableObject
    {
        [SerializeField]
        private List<WiFiPlayerDescriptor> m_Players = new List<WiFiPlayerDescriptor>();

        public int count
        {
            get { return m_Players.Count; }
        }

        public WiFiPlayerDescriptor GetPlayer(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_Players.Count)
            {
                return null;
            }

            return m_Players[i_Index];
        }
    }
}
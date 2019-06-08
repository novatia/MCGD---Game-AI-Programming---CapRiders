using UnityEngine;

using System.Collections.Generic;

using FullInspector;

public class tnCarambolaHolesManager : BaseBehavior
{
    private class TeleportCache
    {
        private List<Collider2D> m_Colliders = null;
        private List<bool> m_AlreadyTeleportedFlags = null;

        public int count
        {
            get
            {
                return m_Colliders.Count;
            }
        }

        // LOGIC

        public void Clear()
        {
            m_Colliders.Clear();
            m_AlreadyTeleportedFlags.Clear();
        }

        public Collider2D GetCollider(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_Colliders.Count)
            {
                return null;
            }

            return m_Colliders[i_Index];
        }

        public bool IsAlreadyTeleported(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_AlreadyTeleportedFlags.Count)
            {
                return false;
            }

            return m_AlreadyTeleportedFlags[i_Index];
        }

        public bool IsAlreadyTeleported(Collider2D i_Collider)
        {
            for (int index = 0; index < m_Colliders.Count; ++index)
            {
                Collider2D collider = m_Colliders[index];
                if (collider == i_Collider)
                {
                    return IsAlreadyTeleported(index);
                }
            }

            return false;
        }

        public void SetTeleportedFlag(int i_Index, bool i_Flag)
        {
            if (i_Index < 0 || i_Index >= m_AlreadyTeleportedFlags.Count)
                return;

            m_AlreadyTeleportedFlags[i_Index] = i_Flag;
        }

        public void SetTeleportedFlag(Collider2D i_Collider, bool i_Flag)
        {
            for (int index = 0; index < m_Colliders.Count; ++index)
            {
                Collider2D collider = m_Colliders[index];
                if (collider == i_Collider)
                {
                    SetTeleportedFlag(index, i_Flag);
                    break;
                }
            } 
        }

        public void Push(Collider2D i_Collider)
        {
            if (i_Collider == null)
                return;

            m_Colliders.Add(i_Collider);
            m_AlreadyTeleportedFlags.Add(false);
        }

        public bool Contains(Collider2D i_Collider)
        {
            return m_Colliders.Contains(i_Collider);
        }

        public void RemoveAt(int i_Index)
        {
            if (i_Index < 0 || i_Index >= m_Colliders.Count)
                return;

            m_Colliders.RemoveAt(i_Index);
            m_AlreadyTeleportedFlags.RemoveAt(i_Index);
        }

        // CTOR

        public TeleportCache()
        {
            m_Colliders = new List<Collider2D>();
            m_AlreadyTeleportedFlags = new List<bool>();
        }
    }

    [SerializeField]
    private List<tnCarambolaHole> m_Holes = new List<tnCarambolaHole>();
    [SerializeField]
    private bool m_DrawGizmos = false;
    
    private TeleportCache m_Cache = new TeleportCache();

    public int holesCount
    {
        get
        {
            return m_Holes.Count;
        }
    }

    // MonoBehaviour's interface

    void OnDrawGizmos()
    {
        if (!m_DrawGizmos)
            return;

        Gizmos.color = Color.blue;

        for (int holesIndex = 0; holesIndex < m_Holes.Count; ++holesIndex)
        {
            tnCarambolaHole hole = m_Holes[holesIndex];
            if (hole != null)
            {
                Gizmos.DrawSphere(hole.transform.position, hole.threshold);
            }
        }
    }

    protected override void Awake()
    {
        for (int holeIndex = 0; holeIndex < m_Holes.Count; ++holeIndex)
        {
            tnCarambolaHole hole = m_Holes[holeIndex];
            if (hole != null)
            {
                hole.SetManager(this);
            }
        }
    }

    void OnEnable()
    {
        for (int holeIndex = 0; holeIndex < m_Holes.Count; ++holeIndex)
        {
            tnCarambolaHole hole = m_Holes[holeIndex];
            if (hole != null)
            {
                hole.onPreHoleOccuredEvent += OnPreHoleOccuredEvent;
                hole.onPostHoleOccuredEvent += OnPostHoleOccuredEvent;
            }
        }

        Messenger.AddListener("FieldReset", OnFieldReset);
        Messenger.AddListener("KickOff", OnKickOff);
    }

    void OnDisable()
    {
        Messenger.RemoveListener("FieldReset", OnFieldReset);
        Messenger.RemoveListener("KickOff", OnKickOff);

        for (int holeIndex = 0; holeIndex < m_Holes.Count; ++holeIndex)
        {
            tnCarambolaHole hole = m_Holes[holeIndex];
            if (hole != null)
            {
                hole.onPreHoleOccuredEvent -= OnPreHoleOccuredEvent;
                hole.onPostHoleOccuredEvent -= OnPostHoleOccuredEvent;
            }
        }
    }

    void Update()
    {
        CheckCollisions();
    }

    // LOGIC
    
    public bool IsValidCollision(Collider2D i_Other)
    {
        if (i_Other == null)
        {
            return false;
        }

        return !m_Cache.Contains(i_Other);
    }

    // INTERNALS

    private void CheckCollisions()
    {
        for (int index = 0; index < m_Cache.count; ++index)
        {
            bool toRemove = true;

            if (!m_Cache.IsAlreadyTeleported(index))
                continue;

            Collider2D collider = m_Cache.GetCollider(index);

            if (collider != null)
            {
                for (int holeIndex = 0; holeIndex < m_Holes.Count; ++holeIndex)
                {
                    tnCarambolaHole hole = m_Holes[holeIndex];

                    if (hole == null)
                        continue;

                    if (hole.IsCollidingWith(collider))
                    {
                        toRemove = false;
                        break;
                    }
                }
            }

            if (toRemove)
            {
                m_Cache.RemoveAt(index);
                index = -1;
            }
        }
    }

    // EVENTS

    private void OnPreHoleOccuredEvent(Collider2D i_Entity)
    {
        if (i_Entity == null)
            return;

        m_Cache.Push(i_Entity);
    }

    private void OnPostHoleOccuredEvent(Collider2D i_Entity)
    {
        m_Cache.SetTeleportedFlag(i_Entity, true);

        CheckCollisions();
    }

    private void OnKickOff()
    {
        for (int holeIndex = 0; holeIndex < m_Holes.Count; ++holeIndex)
        {
            tnCarambolaHole hole = m_Holes[holeIndex];

            if (hole == null)
                continue;

            hole.enabled = true; // Hole clear itself in OnEnable;
        }

        m_Cache.Clear();
    }

    private void OnFieldReset()
    {
        for (int holeIndex = 0; holeIndex < m_Holes.Count; ++holeIndex)
        {
            tnCarambolaHole hole = m_Holes[holeIndex];

            if (hole == null)
                continue;

            hole.enabled = false; // Hole clear itself in OnDisable;
        }

        m_Cache.Clear();
    }
}

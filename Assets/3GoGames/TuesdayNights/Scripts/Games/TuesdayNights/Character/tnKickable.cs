using UnityEngine;

using System.Collections.Generic;

using BaseMatchEvents;

using TuesdayNights;

using TrueSync;

public delegate void KickedCallback();

public struct tnTouchCache
{
    // Fields

    private GameObject m_Source;
    private FP m_Timestamp;

    // ACCESSORS

    public GameObject source
    {
        get { return m_Source; }
    }

    public FP timestamp
    {
        get { return m_Timestamp; }
    }

    // CTOR

    public tnTouchCache(GameObject i_Source, FP i_Timestamp)
    {
        m_Source = i_Source;
        m_Timestamp = i_Timestamp;
    }
}

[RequireComponent(typeof(TSRigidBody2D))]
public class tnKickable : TrueSyncBehaviour
{
    // Serializable fields

    [Header("Params")]

    [SerializeField]
    private bool m_PreventMultipleKick = true;
    [SerializeField]
    private bool m_TrackTouches = false;

    // Fields

    [AddTracking]
    private bool m_Kicked = false;

    private DictionaryList<int, List<tnTouchCache>> m_PendingTouches = new DictionaryList<int, List<tnTouchCache>>();

    // COMPONENTS

    private TSRigidBody2D m_Rigidbody2d = null;
    private tnRespawn m_Respawn = null;

    private Deque<tnTouch> m_Touches = new Deque<tnTouch>();
    private static int s_MaxTouches = 64;

    private event KickedCallback m_KickedEvent;
    public event KickedCallback kickedEvent
    {
        add
        {
            m_KickedEvent += value;
        }

        remove
        {
            m_KickedEvent -= value;
        }
    }

    // GETTERS

    public FP mass
    {
        get
        {
            return m_Rigidbody2d.mass;
        }
    }

    public TSVector2 currentVelocity
    {
        get
        {
            return m_Rigidbody2d.velocity;
        }
    }

    public int touchesCount
    {
        get
        {
            return m_Touches.Count;
        }
    }

    public tnTouch GetTouch(int i_Index)
    {
        return m_Touches[i_Index];
    }

    // MonoBehaviour's interface

    void Awake()
    {
        m_Rigidbody2d = GetComponent<TSRigidBody2D>();
        m_Respawn = GetComponent<tnRespawn>();

        // Set sort order.

        sortOrder = BehaviourSortOrder.s_SortOrder_Kickable;
    }

    void OnEnable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent += OnRespawnOccured;
        }

        InternalStop();
    }

    void OnDisable()
    {
        if (m_Respawn != null)
        {
            m_Respawn.respawnOccurredEvent -= OnRespawnOccured;
        }
    }

    // TrueSyncBehaviour's interface

    public override void OnPreSyncedUpdate()
    {
        base.OnPreSyncedUpdate();

        int currentTick = TrueSyncManager.ticksMain;

        // Clear cache for this tick.

        List<tnTouchCache> touches = m_PendingTouches.GetValue(currentTick);
        if (touches != null)
        {
            touches.Clear();
        }

        // Reset kicked.

        m_Kicked = false;
    }

    public override void OnSyncedUpdate()
    {
        base.OnSyncedUpdate();

        int currentTick = TrueSyncManager.ticksMain;
        int rollbackWindow = Mathf.Max(1, TrueSyncManager.rollbackWindowMain);

        // Convalidate touches.

        for (int index = 0; index < m_PendingTouches.count; ++index)
        {
            int tick = m_PendingTouches.GetKey(index);
            if (currentTick == tick + rollbackWindow)
            {
                List<tnTouchCache> touches = m_PendingTouches[tick];
                if (touches != null)
                {
                    for (int touchIndex = 0; touchIndex < touches.Count; ++touchIndex)
                    {
                        tnTouchCache touch = touches[touchIndex];
                        ForcedAddTocuh(touch.source, touch.timestamp);
                    }
                }
            }
        }

        // Remove old ticks.

        for (int index = 0; index < m_PendingTouches.count; ++index)
        {
            int tick = m_PendingTouches.GetKey(index);
            if (currentTick == tick + rollbackWindow)
            {
                m_PendingTouches.Remove(tick);
                index = -1;
            }
        }
    }

    public override void OnSyncedCollisionEnter(TSCollision2D i_Collision)
    {
        if (!m_TrackTouches)
            return;

        GameObject otherGo = i_Collision.gameObject;

        if (otherGo.CompareTag(Tags.s_Character))
        {
            InternalAddTouch(otherGo, TrueSyncManager.timeMain /* Timestamp */);
        }
    }

    // LOGIC

    public void Kick(TSVector2 i_Direction, FP i_Force, GameObject i_Source)
    {
        TSVector2 direction = i_Direction.normalized;
        Kick(direction * i_Force, i_Source);
    }

    public void Kick(TSVector2 i_Force, GameObject i_Source)
    {
        if (m_PreventMultipleKick && m_Kicked)
            return;

        m_Rigidbody2d.AddForce(i_Force);

        if (m_TrackTouches)
        {
            InternalAddTouch(i_Source, TrueSyncManager.timeMain /* Timestamp */);
        }

        m_Kicked = true;

        if (m_KickedEvent != null)
        {
            m_KickedEvent();
        }
    }

    // EVENTS

    private void OnRespawnOccured()
    {
        InternalStop();
    }

    // INTERNALS

    private void InternalAddTouch(GameObject i_Source, FP i_Timestamp)
    {
        int currentTick = TrueSyncManager.ticksMain;

        // Build touche cache.

        tnTouchCache touch = new tnTouchCache(i_Source, i_Timestamp);

        // Add it to list.

        List<tnTouchCache> touches;
        bool alreadyPresent = m_PendingTouches.TryGetValue(currentTick, out touches);
        if (touches == null)
        {
            touches = new List<tnTouchCache>();
        }

        if (!alreadyPresent)
        {
            m_PendingTouches.Add(currentTick, touches);
        }

        touches.Add(touch);
    }

    private void ForcedAddTocuh(GameObject i_Source, FP i_Timestamp)
    {
        if (i_Source == null)
            return;

        tnCharacterInfo characterInfo = i_Source.GetComponent<tnCharacterInfo>();

        if (characterInfo == null)
            return;

        if (m_Touches.Count > 0)
        {
            tnTouch lastTouch = m_Touches[0];

            if (lastTouch.characterId == characterInfo.characterId)
            {
                return;
            }
        }

        // If queue is full, remove the oldest touch.

        while (m_Touches.Count >= s_MaxTouches)
        {
            m_Touches.RemoveBack();
        }

        // Add new ball touch.

        tnTouch touch = new tnTouch();

        touch.SetCharacterId(characterInfo.characterId);
        touch.SetTeamId(characterInfo.teamId);

        tnCharacterInput characterInput = i_Source.GetComponent<tnCharacterInput>();
        if (characterInput != null)
        {
            touch.SetIsHuman(characterInput.isHumanPlayer);
            touch.SetIsLocal(characterInput.isLocalPlayer);
        }
        else
        {
            touch.SetIsHuman(false);
            touch.SetIsLocal(false);
        }

        touch.SetTimestamp(i_Timestamp);

        m_Touches.AddFront(touch);
    }

    private void InternalStop()
    {
        m_Kicked = false;

        m_Touches.Clear();
    }
}

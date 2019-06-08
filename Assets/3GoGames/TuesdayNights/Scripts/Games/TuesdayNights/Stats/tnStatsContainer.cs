using UnityEngine;

using System;
using System.Collections.Generic;

public class tnStatsContainer : MonoBehaviour
{
    [SerializeField]
    private tnStatsDatabase m_StatsDatabase = null;

    private Dictionary<int, tnAttribute> m_Attributes = new Dictionary<int, tnAttribute>();

    private Dictionary<int, List<tnStatChangedCallback>> m_CallbacksCache = new Dictionary<int, List<tnStatChangedCallback>>();
    private Dictionary<int, List<tnAttributeModifier>> m_ModifiersCache = new Dictionary<int, List<tnAttributeModifier>>();

    private class TagRef
    {
        private int m_HashCode;
        private int m_Counter;

        public int hashCode
        {
            get
            {
                return m_HashCode;
            }
        }

        public void AddRef()
        {
            ++m_Counter;
        }

        public void RemoveRef()
        {
            --m_Counter;

            if (m_Counter < 0)
            {
                m_Counter = 0;
            }
        }

        public bool IsValid()
        {
            return (m_Counter > 0);
        }

        public TagRef(int i_HashCode)
        {
            m_HashCode = i_HashCode;
            m_Counter = 0;
        }

        public TagRef(string i_Tag)
        {
            m_HashCode = StringUtils.GetHashCode(i_Tag);
            m_Counter = 0;
        }
    };

    private List<TagRef> m_Tags = new List<TagRef>();

    // MonoBehaviour's interface

    void Awake()
    {
        Internal_CreateAttributes();
    }

    // INTERNALS

    private void Internal_CreateAttributes()
    {
        // Clear attributes.

        m_Attributes.Clear();

        if (m_StatsDatabase == null)
            return;

        // Create new attributes.

        for (int statIndex = 0; statIndex < m_StatsDatabase.statsCount; ++statIndex)
        {
            tnStatEntry stat = m_StatsDatabase.GetStat(statIndex);

            string id = stat.attributeId;
            if (id != "")
            {
                tnAttribute attribute = new tnAttribute(stat.baseValue);
                int hashCode = StringUtils.GetHashCode(id);
                m_Attributes.Add(hashCode, attribute);
            }
        }

        // Apply cache to new attributes.

        foreach (int attributeId in m_Attributes.Keys)
        {
            // Callbacks.

            {
                List<tnStatChangedCallback> cachedCallbacks = null;
                if (m_CallbacksCache.TryGetValue(attributeId, out cachedCallbacks))
                {
                    if (cachedCallbacks != null)
                    {
                        for (int index = 0; index < cachedCallbacks.Count; ++index)
                        {
                            tnStatChangedCallback callback = cachedCallbacks[index];
                            if (callback != null)
                            {
                                RegisterHandler(attributeId, callback);
                            }
                        }

                        cachedCallbacks.Clear();
                    }

                    m_CallbacksCache.Remove(attributeId);
                }
            }

            // Modifiers.

            {
                List<tnAttributeModifier> cachedModifiers = null;
                if (m_ModifiersCache.TryGetValue(attributeId, out cachedModifiers))
                {
                    if (cachedModifiers != null)
                    {
                        for (int index = 0; index < cachedModifiers.Count; ++index)
                        {
                            tnAttributeModifier modifier = cachedModifiers[index];
                            if (modifier != null)
                            {
                                AddModifier(modifier);
                            }
                        }

                        cachedModifiers.Clear();
                    }

                    m_ModifiersCache.Remove(attributeId);
                }
            }
        }
    }

    private void Internal_DestroyAttributes()
    {
        // Save cache.

        foreach (int attributeId in m_Attributes.Keys)
        {
            tnAttribute attribute = m_Attributes[attributeId];

            if (attribute == null)
                continue;

            // Callbacks.

            {
                if (attribute.statChangedEvent != null)
                {
                    Delegate[] callbacks = attribute.statChangedEvent.GetInvocationList();
                    if (callbacks != null)
                    {
                        if (callbacks.Length > 0)
                        {
                            List<tnStatChangedCallback> cache = new List<tnStatChangedCallback>();

                            for (int index = 0; index < callbacks.Length; ++index)
                            {
                                tnStatChangedCallback callback = (tnStatChangedCallback)callbacks[index];

                                if (callback == null)
                                    continue;

                                cache.Add(callback);
                            }

                            m_CallbacksCache.Add(attributeId, cache);
                        }
                    }
                }
            }

            // Modifiers.

            {
                if (attribute.modifiersCount > 0)
                {
                    List<tnAttributeModifier> cache = new List<tnAttributeModifier>();

                    for (int index = 0; index < attribute.modifiersCount; ++index)
                    {
                        tnAttributeModifier modifier = attribute.GetModifier(index);

                        if (modifier == null)
                            continue;

                        cache.Add(modifier);
                    }

                    m_ModifiersCache.Add(attributeId, cache);
                }
            }
        }

        // Clear attributes.

        m_Attributes.Clear();
    }

    // BUSINESS LOGIC

    public void SetStatsDatabase(tnStatsDatabase i_StatsDatabase)
    {
        Internal_DestroyAttributes();

        m_StatsDatabase = i_StatsDatabase;

        Internal_CreateAttributes();
    }

    public tnAttribute GetAttribute(int i_HashCode)
    {
        tnAttribute attribute = null;
        if (m_Attributes.TryGetValue(i_HashCode, out attribute))
        {
            return attribute;
        }

        return null; // Attribute not found.
    }

    public tnAttribute GetAttribute(string i_Id)
    {
        return GetAttribute(StringUtils.GetHashCode(i_Id));
    }

    public bool HasTag(int i_HashCode)
    {
        for (int tagRefIndex = 0; tagRefIndex < m_Tags.Count; ++tagRefIndex)
        {
            TagRef tagRef = m_Tags[tagRefIndex];

            if (tagRef.hashCode == i_HashCode)
            {
                return tagRef.IsValid();
            }
        }

        return false;
    }

    public bool HasTag(string i_Id)
    {
        return HasTag(StringUtils.GetHashCode(i_Id));
    }

    public void AddModifier(tnAttributeModifier i_Modifier)
    {
        if (i_Modifier == null)
            return;

        int attributeId = i_Modifier.attributeId;

        tnAttribute attribute = GetAttribute(attributeId);
        if (attribute != null)
        {
            attribute.AddModifier(i_Modifier);
        }
        else
        {
            List<tnAttributeModifier> cache = null;
            if (m_ModifiersCache.TryGetValue(attributeId, out cache))
            {
                if (cache == null)
                {
                    cache = new List<tnAttributeModifier>();
                }

                cache.Add(i_Modifier);
            }
            else
            {
                cache = new List<tnAttributeModifier>();
                cache.Add(i_Modifier);

                m_ModifiersCache.Add(attributeId, cache);
            }
        }
    }

    public void RemoveModifier(tnAttributeModifier i_Modifier)
    {
        if (i_Modifier == null)
            return;

        int attributeId = i_Modifier.attributeId;

        tnAttribute attribute = GetAttribute(attributeId);
        if (attribute != null)
        {
            attribute.RemoveModifier(i_Modifier);
        }
        else
        {
            List<tnAttributeModifier> cache = null;
            if (m_ModifiersCache.TryGetValue(attributeId, out cache))
            {
                if (cache != null)
                {
                    cache.Remove(i_Modifier);

                    if (cache.Count == 0)
                    {
                        m_ModifiersCache.Remove(attributeId);
                    }
                }
                else
                {
                    m_CallbacksCache.Remove(attributeId);
                }
            }
        }
    }

    public void AddTag(int i_HashCode)
    {
        for (int tagRefIndex = 0; tagRefIndex < m_Tags.Count; ++tagRefIndex)
        {
            TagRef tagRef = m_Tags[tagRefIndex];

            if (tagRef.hashCode == i_HashCode)
            {
                tagRef.AddRef();
                return;
            }
        }

        // TagRef not found, create a new one.

        TagRef newTagRef = new TagRef(i_HashCode);
        newTagRef.AddRef();

        m_Tags.Add(newTagRef);
    }

    public void AddTag(string i_Id)
    {
        AddTag(StringUtils.GetHashCode(i_Id));
    }

    public void RemoveTag(int i_HashCode)
    {
        for (int tagRefIndex = 0; tagRefIndex < m_Tags.Count; ++tagRefIndex)
        {
            TagRef tagRef = m_Tags[tagRefIndex];

            if (tagRef.hashCode == i_HashCode)
            {
                tagRef.RemoveRef();
                return;
            }
        }
    }

    public void RemoveTag(string i_Id)
    {
        RemoveTag(StringUtils.GetHashCode(i_Id));
    }

    public void RegisterHandler(string i_AttributeId, tnStatChangedCallback i_Callback)
    {
        int hash = StringUtils.GetHashCode(i_AttributeId);
        RegisterHandler(hash, i_Callback);
    }

    public void RegisterHandler(int i_AttributeId, tnStatChangedCallback i_Callback)
    {
        tnAttribute attribute = null;
        if (m_Attributes.TryGetValue(i_AttributeId, out attribute))
        {
            attribute.statChangedEvent += i_Callback;
            i_Callback(attribute.baseValue, attribute.value);
        }
        else
        {
            List<tnStatChangedCallback> cache = null;
            if (m_CallbacksCache.TryGetValue(i_AttributeId, out cache))
            {
                if (cache == null)
                {
                    cache = new List<tnStatChangedCallback>();
                }

                cache.Add(i_Callback);
            }
            else
            {
                cache = new List<tnStatChangedCallback>();
                cache.Add(i_Callback);

                m_CallbacksCache.Add(i_AttributeId, cache);
            }
        }
    }

    public void UnregisterHandler(string i_AttributeId, tnStatChangedCallback i_Callback)
    {
        int hash = StringUtils.GetHashCode(i_AttributeId);
        UnregisterHandler(hash, i_Callback);
    }

    public void UnregisterHandler(int i_AttributeId, tnStatChangedCallback i_Callback)
    {
        tnAttribute attribute = null;
        if (m_Attributes.TryGetValue(i_AttributeId, out attribute))
        {
            attribute.statChangedEvent -= i_Callback;
        }
        else
        {
            List<tnStatChangedCallback> cache = null;
            if (m_CallbacksCache.TryGetValue(i_AttributeId, out cache))
            {
                if (cache != null)
                {
                    cache.Remove(i_Callback);

                    if (cache.Count == 0)
                    {
                        m_CallbacksCache.Remove(i_AttributeId);
                    }
                }
                else
                {
                    m_CallbacksCache.Remove(i_AttributeId);
                }
            }
        }
    }
}

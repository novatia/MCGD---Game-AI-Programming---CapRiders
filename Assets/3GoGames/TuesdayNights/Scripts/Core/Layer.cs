using UnityEngine;

using System;

[Serializable]
public class Layer
{
    // STATIC

    public static bool IsGameObjectInLayerMask(GameObject i_Go, LayerMask i_Mask)
    {
        if (i_Go == null)
        {
            return false;
        }

        int layer = i_Go.layer;
        return IsLayerInMask(layer, i_Mask);
    }

    public static bool IsLayerInMask(int i_Layer, LayerMask i_Mask)
    {
        LayerMask layerMask = (1 << i_Layer);
        int checkResult = (i_Mask.value & layerMask.value);
        return (checkResult > 0);
    }

    public static bool IsValidLayer(int i_Layer)
    {
        return (i_Layer >= 0 && i_Layer <= 31);
    }

    // Serializable fields

    [SerializeField]
    public int m_LayerIndex;

    // LOGIC

    public LayerMask GetMask()
    {
        if (m_LayerIndex < 0)
        {
            return 0;
        }

        return (1 << m_LayerIndex);
    } 

    public static implicit operator int (Layer i_Layer)
    {
        return i_Layer.m_LayerIndex;
    }

    // CTOR

    public Layer()
    {
        m_LayerIndex = 0;
    }
}

using UnityEngine;

public class UINavbarEntry : MonoBehaviour
{
    [SerializeField]
    private string m_IconKey = "";
    private int m_IconKeyHash;

    [SerializeField]
    private string m_Text = "";
    
    [SerializeField]
    private int m_SortIndex = -1;

    public string iconKey
    {
        get
        {
            return m_IconKey;
        }
    }

    public int iconKeyHash
    {
        get
        {
            return m_IconKeyHash;
        }
    }

    public string text
    {
        get
        {
            return m_Text;
        }
    }

    public int sortIndex
    {
        get
        {
            return m_SortIndex;
        }
    }

    // UINavbarEntry's INTERFACE

    public virtual bool isActive
    {
        get
        {
            return true;
        }
    }

    // MonoBehaviour's INTERFACE

    protected virtual void Awake()
    {
        m_IconKeyHash = StringUtils.GetHashCode(m_IconKey);
    }
}

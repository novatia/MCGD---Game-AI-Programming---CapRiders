using UnityEngine;

public class NullUserInfoModuleImpl : IUserInfoModuleImpl
{
    private string m_Username = "";

    // IUserInfoModuleImpl's interface

    public string username
    {
        get
        {
            return m_Username;
        }
    }

    public void Initialize()
    {

    }

    // CTOR

    public NullUserInfoModuleImpl()
    {
        m_Username = "User#" + Random.Range(1000, 9999);
    }
}

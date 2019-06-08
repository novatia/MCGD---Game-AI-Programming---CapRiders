using System;
using System.Diagnostics;

[Serializable]
public struct ObjectId : IEquatable<ObjectId>
{
    private int _Hash;

    public int Hash
    {
        get
        {
            return _Hash;
        }
    }

#if ENABLE_OBJECTID_NAME
        private string _Name;

        public string Name
        {
            get
            {
                return _Name;
            }
        }

        private static bool CheckConsistency(ObjectId first, ObjectId second)
        {
            bool bCheckHash = (first._Hash == second._Hash);
            bool bCheckName = (first._Name == second._Name);
            return (bCheckHash == bCheckName);
        }
#endif // ENABLE_OBJECTID_NAME

    public override string ToString()
    {
#if ENABLE_OBJECTID_NAME
            return _Name;
#else
        return _Hash.ToString();
#endif // ENABLE_OBJECTID_NAME
    }

    public override int GetHashCode()
    {
        return _Hash;
    }

    public bool Equals(ObjectId i_Id)
    {
#if ENABLE_OBJECTID_NAME
            Debug.Assert(CheckConsistency(this, i_Id));
#endif // ENABLE_OBJECTID_NAME

        return this._Hash == i_Id._Hash;
    }

    public override bool Equals(object obj)
    {
        return Equals((ObjectId)obj);
    }

    public static bool operator ==(ObjectId first, ObjectId second)
    {
        return first.Equals(second);
    }

    public static bool operator !=(ObjectId first, ObjectId second)
    {
        return !(first == second);
    }

    public static implicit operator ObjectId(string name)
    {
        return new ObjectId(name);
    }

    public ObjectId(string name)
    {
        _Hash = name.GetHashCode(); // name.GetHashCode();

#if ENABLE_OBJECTID_NAME
            _Name = name;
#endif // ENABLE_OBJECTID_NAME
    }

    public static ObjectId Empty = new ObjectId("");

    public static bool IsEmpty(ObjectId id)
    {
        return id.Equals(Empty);
    }

    public bool IsEmpty()
    {
        return this.Equals(Empty);
    }
}

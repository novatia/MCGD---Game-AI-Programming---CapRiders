using UnityEngine;

public enum PersonaStatus
{
    None,
    Offline,
    Online,
}

public abstract class Friend
{
    // ACCESSORS

    public abstract string name { get; }

    public abstract Sprite icon { get; }
 
    public abstract PersonaStatus status { get; }

    // CTOR

    public Friend()
    {
        
    }
}
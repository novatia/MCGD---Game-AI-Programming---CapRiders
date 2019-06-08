using System;

[Serializable]
public abstract class UserStatCondition
{
    // LOGIC

    public bool Evaluate()
    {
        return OnEvaluate();
    }

    // ABSTRACT

    public abstract void Initialize(StatsModule i_StatsModule);
    protected abstract bool OnEvaluate();
}
public enum NumericConditionType
{
    Equal,
    NotEqual,
    Greater,
    GreaterOrEqual,
    Less,
    LessOrEqual,
}

public enum StringConditionType
{
    Equal,
    NotEqual,
}

public enum BooleanConditionType
{
    isTrue,
    isFalse,
}

public enum UserStatConditionType
{
    Int = 0,
    Bool = 1,
    Float = 2,
    String = 3,
    Not = 4,
    And = 5,
    Or = 6,
    Xor = 7,
}

public enum UserStatType
{
    Invalid = 0,
    Int = 1,
    Bool = 2,
    Float = 3,
    String = 4,
}

public enum NumericCombineFunction
{
    Set,
    Min,
    Max,
    Add,
    Multiply,
}

public enum BooleanCombineFunction
{
    Set,
    Add,
    Multiply,
}

public enum StringCombineFunction
{
    Set,
    Min,
    Max,
    Add,
}
public static class Hash
{
    public static int s_NULL = StringUtils.GetHashCode("NULL");
    public static int s_EMPTY = StringUtils.GetHashCode("");

    public static bool IsValid(int i_Hash)
    {
        return (i_Hash != s_NULL && i_Hash != s_EMPTY);
    }

    public static bool IsNullOrEmpty(int i_Hash)
    {
        return (i_Hash == s_NULL || i_Hash == s_EMPTY);
    }
}
public class tnGameModeIntOption : tnGameModeOption<int>
{
    // CTOR

    public tnGameModeIntOption(tnGameModeIntOptionDescriptor i_Descriptor)
        : base()
    {
        if (i_Descriptor != null)
        {
            foreach (string key in i_Descriptor.keys)
            {
                int value;
                if (i_Descriptor.TryGetValue(key, out value))
                {
                    int hash = StringUtils.GetHashCode(key);
                    InternalAdd(hash, value);
                }
            }
        }
    }
}

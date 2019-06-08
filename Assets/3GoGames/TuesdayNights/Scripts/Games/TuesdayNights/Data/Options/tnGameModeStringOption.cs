public class tnGameModeStringOption : tnGameModeOption<string>
{
    // CTOR

    public tnGameModeStringOption(tnGameModeStringOptionDescriptor i_Descriptor)
        : base()
    {
        foreach (string key in i_Descriptor.keys)
        {
            string value;
            if (i_Descriptor.TryGetValue(key, out value))
            {
                int hash = StringUtils.GetHashCode(key);
                InternalAdd(hash, value);
            }
        }
    }
}

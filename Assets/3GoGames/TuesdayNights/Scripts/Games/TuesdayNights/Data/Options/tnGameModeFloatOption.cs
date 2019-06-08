public class tnGameModeFloatOption : tnGameModeOption<float>
{
    // CTOR

    public tnGameModeFloatOption(tnGameModeFloatOptionDescriptor i_Descriptor)
        : base()
    {
        foreach (string key in i_Descriptor.keys)
        {
            float value;
            if (i_Descriptor.TryGetValue(key, out value))
            {
                int hash = StringUtils.GetHashCode(key);
                InternalAdd(hash, value);
            }
        }
    }
}

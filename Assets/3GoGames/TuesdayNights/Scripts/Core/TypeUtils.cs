using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

public static class TypeUtils
{
    public static List<Type> GetAllDerivedType<T>()
    {
        Type baseType = typeof(T);
        return GetAllDerivedTypeFrom(baseType);
    }

    public static List<Type> GetAllDerivedTypeFrom(Type i_Type)
    {
        if (i_Type == null)
        {
            return null;
        }

        Assembly assembly = Assembly.GetAssembly(i_Type);
        if (assembly != null)
        {
            Type[] types = assembly.GetTypes();
            List<Type> derived = types.Where(t => t != i_Type && i_Type.IsAssignableFrom(t)).ToList();
            return derived;
        }

        return null;
    }
}

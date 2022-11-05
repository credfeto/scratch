using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Implementations;

public static class EnumHelpers
{
    public static string GetName<T>(T value)
        where T : Enum
    {
        Type type = value.GetType();

        IReadOnlyList<FieldInfo> fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);

        DumpFields(fields: fields, type: type);

        return Enum.GetName(enumType: type, value: value) ?? UnknownName(value);
    }

    [Conditional("DEBUG")]
    private static void DumpFields(IReadOnlyList<FieldInfo> fields, Type type)

    {
        foreach (FieldInfo field in fields)
        {
            Enum x = (Enum)field.GetValue(null)!;
            string xName = Enum.GetName(enumType: type, value: x) ?? "NFI";

            Debug.WriteLine($"{field.Name} = {xName}");
        }
    }

    private static string UnknownName(Enum value)
    {
        return "??" + value;
    }
}
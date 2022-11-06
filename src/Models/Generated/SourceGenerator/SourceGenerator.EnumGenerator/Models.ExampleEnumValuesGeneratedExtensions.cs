﻿using System;
using System.CodeDom.Compiler;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Models;

[GeneratedCode(tool: "EnumGenerator", version: "development")]
public static class ExampleEnumValuesGeneratedExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetName(this ExampleEnumValues value)
    {
        return value switch
        {
            ExampleEnumValues.ZERO => "ZERO",
            ExampleEnumValues.ONE => "ONE",
            ExampleEnumValues.THREE => "THREE",
            _ => throw new ArgumentOutOfRangeException(nameof(value), actualValue: value, message: "Unknown enum member")
        };
        
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetDescription(this ExampleEnumValues value)
    {
        return value switch
        {
            ExampleEnumValues.ONE => "One \"1\"",
            ExampleEnumValues.THREE => "Two but one better!",
            _ => GetName(value)
        };
        
    }
    
}


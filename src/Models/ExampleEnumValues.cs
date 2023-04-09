using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Models;

public enum ExampleEnumValues
{
    ZERO = 0,

    [Description("One \"1\"")]
    ONE = 1,

    SAME_AS_ONE = ONE,

    [SuppressMessage(category: "SonarAnalyzer.CSharp", checkId: "S1133: Remove this deprecated code", Justification = "For testing purposes")]
    [Obsolete("This value is deprecated, use " + nameof(THREE) + " instead.")]
    TWO = 2,

    [Description("Two but one better!")]
    THREE = 3
}
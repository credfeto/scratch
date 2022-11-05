using System;
using System.ComponentModel;

namespace Models;

public enum ExampleEnumValues
{
    ZERO = 0,

    ONE = 1,

    SAME_AS_ONE = ONE,

    [Obsolete("This value is deprecated, use " + nameof(THREE) + " instead.")]
    TWO = 2,

    [Description("Two but one better!")]
    THREE = 3
}
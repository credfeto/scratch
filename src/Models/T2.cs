using System.Diagnostics.CodeAnalysis;

namespace Models;

[SuppressMessage(
    category: "Meziantou.Analyzers",
    checkId: "MA0036: Make class static",
    Justification = "cannot be for a test case"
)]
public sealed class T2;

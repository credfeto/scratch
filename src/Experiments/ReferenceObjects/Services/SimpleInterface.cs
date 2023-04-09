using System.Diagnostics.CodeAnalysis;

namespace Experiments.ReferenceObjects.Services;

[SuppressMessage(category: "SonarAnalyzer.CSharp", checkId: "S2094: Empty class make it an interface", Justification = "For testing purposes")]
public sealed class SimpleInterface : ISimpleInterface
{
}
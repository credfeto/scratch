using System.Threading;
using System.Threading.Tasks;
using Experiments.Implementations;
using FunFair.Test.Common;
using Xunit;
using Xunit.Abstractions;

namespace Experiments;

public sealed class BuildOptimised : LoggingTestBase
{
    private const string SOURCE = "/home/markr/work/scratch/";
    private const string DESTINATION = "/home/markr/Downloads/scratch-dest/";
    private readonly IHashedContentOptimizer _hashedContentOptimizer;
    private readonly IHashedFileDetector _hashedFileDetector;

    public BuildOptimised(ITestOutputHelper outpout)
        : base(outpout)
    {
        this._hashedFileDetector = new HashedFileDetector();
        this._hashedContentOptimizer = new HashedContentOptimizer(
            hashedFileDetector: this._hashedFileDetector,
            this.GetTypedLogger<HashedContentOptimizer>()
        );
    }

    [Fact(Skip = "Need to correct paths")]
    public async Task BuildAsync()
    {
        await Task.CompletedTask;

        await this._hashedContentOptimizer.OptimizeAsync(
            source: SOURCE,
            destination: DESTINATION,
            cancellationToken: CancellationToken.None
        );
    }
}

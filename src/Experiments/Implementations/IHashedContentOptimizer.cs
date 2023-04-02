using System.Threading;
using System.Threading.Tasks;

namespace Experiments.Implementations;

public interface IHashedContentOptimizer
{
    Task OptimizeAsync(string source, string destination, CancellationToken cancellationToken);
}
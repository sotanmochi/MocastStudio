using System.Threading;
using System.Threading.Tasks;

namespace MocapSignalTransmission.MotionDataSource
{
    public interface IMotionDataSourceManager
    {
        bool Contains(int dataSourceId);
        Task<IMotionDataSource> CreateAsync(int dataSourceId, MotionDataSourceSettings dataSourceSettings);
        Task<bool> ConnectAsync(int dataSourceId, CancellationToken cancellationToken = default);
        Task DisconnectAsync(int dataSourceId, CancellationToken cancellationToken = default);
    }
}

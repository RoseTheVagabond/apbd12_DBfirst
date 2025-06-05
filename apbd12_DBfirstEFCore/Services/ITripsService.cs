using apbd12_DBfirstEFCore.DTOs;

namespace apbd12_DBfirstEFCore.Services;

public interface ITripsService
{
    Task<TripsResponseDTO> GetAllTripsAsync(CancellationToken cancellationToken = default, int pageNum = 1, int pageSize = 10);
    Task<bool> DoesClientExist(int clientId, CancellationToken cancellationToken = default);
    Task<bool> DoesClientHaveTrips(int clientId, CancellationToken cancellationToken = default);
    Task<int> DeleteClient(int clientId, CancellationToken cancellationToken = default);
}
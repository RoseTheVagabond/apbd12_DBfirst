using apbd12_DBfirstEFCore.DTOs;

namespace apbd12_DBfirstEFCore.Services;

public interface ITripsService
{
    Task<TripsResponseDTO> GetAllTripsAsync(CancellationToken cancellationToken = default, int pageNum = 1, int pageSize = 10);
    
}
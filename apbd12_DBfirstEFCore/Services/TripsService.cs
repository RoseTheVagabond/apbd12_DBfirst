using apbd12_DBfirstEFCore.DTOs;
using apbd12_DBfirstEFCore.Models;
using Microsoft.EntityFrameworkCore;

namespace apbd12_DBfirstEFCore.Services;

public class TripsService : ITripsService
{
    private readonly TripsContext _context;

    public TripsService(TripsContext context)
    {
        _context = context;
    }

    public async Task<TripsResponseDTO> GetAllTripsAsync(CancellationToken cancellationToken = default, int pageNum = 1, int pageSize = 10)
    {
        int totalCount = await _context.Trips.CountAsync(cancellationToken);
        int allPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        TripsResponseDTO tripsResponseDTO = new TripsResponseDTO();
        tripsResponseDTO.pageNum = pageNum;
        tripsResponseDTO.pageSize = pageSize;
        tripsResponseDTO.allPages = allPages;

        tripsResponseDTO.trips = new List<TripDTO>();
        
        List<Trip> allTrips = await _context.Trips.OrderByDescending(t => t.DateFrom).ToListAsync(cancellationToken);
        foreach (Trip trip in allTrips)
        {
            TripDTO tripDTO = new TripDTO()
            {
                Name = trip.Name,
                DateFrom = trip.DateFrom,
                DateTo = trip.DateTo,
                MaxPeople = trip.MaxPeople,
                Counties = trip.IdCountries.Select(c => new CountryDTO()
                {
                    Name = c.Name
                }).ToList(),
                Clients = trip.ClientTrips.Select(c => new ClientDTO()
                {
                    FirstName = c.IdClientNavigation.FirstName,
                    LastName = c.IdClientNavigation.LastName
                }).ToList()
            };
            tripsResponseDTO.trips.Add(tripDTO);
        }
        return tripsResponseDTO;
    }
}
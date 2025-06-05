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
        
        var trips = await _context.Trips.OrderByDescending(t => t.DateFrom)
            .Skip((pageNum - 1) * pageSize).Take(pageSize).Select(trip => new TripDTO
            {
                Name = trip.Name,
                DateFrom = trip.DateFrom,
                DateTo = trip.DateTo,
                MaxPeople = trip.MaxPeople,
                Counties = trip.IdCountries
                    .Select(country => new CountryDTO { Name = country.Name })
                    .ToList(),
                Clients = trip.ClientTrips
                    .Select(ct => new ClientDTO 
                    { 
                        FirstName = ct.IdClientNavigation.FirstName,
                        LastName = ct.IdClientNavigation.LastName
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        return new TripsResponseDTO
        {
            pageNum = pageNum,
            pageSize = pageSize,
            allPages = allPages,
            trips = trips
        };
    }
}
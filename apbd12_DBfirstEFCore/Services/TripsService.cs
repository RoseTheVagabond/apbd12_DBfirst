using System.Data;
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

    public async Task<bool> DoesClientExist(int clientId, CancellationToken cancellationToken = default)
    {
        return await _context.Clients.AnyAsync(c => c.IdClient == clientId, cancellationToken);
    }

    public async Task<bool> DoesClientHaveTrips(int clientId, CancellationToken cancellationToken = default)
    {
       return await _context.ClientTrips.AnyAsync(ct => ct.IdClient == clientId, cancellationToken: cancellationToken);
    }

    public async Task<int> DeleteClient(int clientId, CancellationToken cancellationToken = default)
    {
        if (!await DoesClientExist(clientId, cancellationToken))
        {
            throw new ArgumentException();
        }
        if (await DoesClientHaveTrips(clientId, cancellationToken))
        {
            throw new DataException();
        }
        
        var client = await _context.Clients.FindAsync(clientId, cancellationToken);
        if (client != null)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return clientId;
    }

    public async Task AssignClientToTrip(int idTrip, AssignClientDTO clientDto, CancellationToken cancellationToken)
    {
        var existingClient = await _context.Clients.FirstOrDefaultAsync(ct => ct.Pesel == clientDto.Pesel, cancellationToken);
        if (existingClient == null)
        {
            throw new Exception($"Client with pesel {clientDto.Pesel} does not exist");
        }

        var clientAssigned =
            await _context.ClientTrips.FirstOrDefaultAsync(ct => ct.IdClientNavigation.Pesel == clientDto.Pesel && ct.IdTrip == idTrip,
                cancellationToken);
        if (clientAssigned != null)
        {
            throw new Exception($"Client with pesel {clientDto.Pesel} is already assigned to the trip with id {idTrip}");
        }
        var trip = await _context.Trips.FindAsync(idTrip, cancellationToken);
        if (trip == null)
        {
            throw new Exception($"Trip with id {idTrip} does not exist");
        }

        var futureTrip = await _context.Trips.FirstOrDefaultAsync(t => t.DateFrom > DateTime.Today && t.IdTrip == idTrip, cancellationToken);
        if (futureTrip == null)
        {
            throw new Exception($"Trip with id {idTrip} is not in the future");
        }

        var clientTrip = new ClientTrip()
        {
            IdClient = existingClient.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = clientDto.PaymentDate
        };
        _context.ClientTrips.Add(clientTrip);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
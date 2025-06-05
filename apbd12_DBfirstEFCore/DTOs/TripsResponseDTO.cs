using apbd12_DBfirstEFCore.Models;

namespace apbd12_DBfirstEFCore.DTOs;

public class TripsResponseDTO
{
    public int pageNum { get; set; }
    public int pageSize { get; set; }
    public int allPages { get; set; }
    public List<TripDTO> trips { get; set; }
}

public class TripDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<CountryDTO> Counties { get; set; }
    public List<ClientDTO> Clients { get; set; }
}

public class CountryDTO
{
    public string Name { get; set; }
}
public class ClientDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
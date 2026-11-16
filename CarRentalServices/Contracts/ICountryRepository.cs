
using CarRentalServices.Entities;
using System.Collections.Generic;

namespace CarRentalServices.Contracts
{
    public interface ICountryRepository
    {
        IEnumerable<Country> GetCountries();
        Country? GetCountryById(int id);
        bool InsertCountry(Country country);
        bool UpdateCountry(Country country);
    }
}

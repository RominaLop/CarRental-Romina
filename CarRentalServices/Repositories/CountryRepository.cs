
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace CarRentalServices.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly IMySqlDataService _dataService;

        public CountryRepository(IMySqlDataService dataService)
        {
            _dataService = dataService;
        }

        public IEnumerable<Country> GetCountries()
        {
            var data = _dataService.ExecSP("GetCountries");

            if (data != null && data.Tables.Count > 0)
            {
                return MapCountries(data.Tables[0]);
            }

            return new List<Country>();
        }

        public Country? GetCountryById(int id)
        {
            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("p_CountryId", MySqlDbType.Int32)
                {
                    Value = id
                }
            };

            var data = _dataService.ExecSP("GetCountryById", parameters);

            if (data != null && data.Tables.Count > 0 &&
                data.Tables[0].Rows.Count > 0)
            {
                return MapCountry(data.Tables[0].Rows[0]);
            }

            return null;
        }

        public bool InsertCountry(Country country)
        {
            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("p_CountryName", MySqlDbType.VarChar)
                {
                    Value = country.CountryName
                }
            };

            var result = _dataService.ExecSP("InsertCountry", parameters);

            if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
            {
                country.CountryId = Convert.ToInt32(
                    result.Tables[0].Rows[0]["newCountryId"]);
            }

            return country.CountryId > 0;
        }

        public bool UpdateCountry(Country country)
        {
            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("p_CountryId", MySqlDbType.Int32)
                {
                    Value = country.CountryId
                },
                new MySqlParameter("p_CountryName", MySqlDbType.VarChar)
                {
                    Value = country.CountryName
                }
            };

            var result = _dataService.ExecSP("UpdateCountry", parameters);

            return result.Tables.Count > 0 &&
                   result.Tables[0].Rows.Count > 0 &&
                   Convert.ToInt32(result.Tables[0].Rows[0]["affectedRows"]) > 0;
        }

        private List<Country> MapCountries(DataTable dataTable)
        {
            var countries = new List<Country>();

            foreach (DataRow row in dataTable.Rows)
            {
                countries.Add(MapCountry(row));
            }

            return countries;
        }

        private Country MapCountry(DataRow row)
        {
            return new Country
            {
                CountryId = Convert.ToInt32(row["CountryId"]),
                CountryName = Convert.ToString(row["CountryName"]) ?? ""
            };
        }
    }
}

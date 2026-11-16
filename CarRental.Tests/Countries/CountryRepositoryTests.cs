
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Repositories;
using Moq;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CarRental.Tests.Countries
{
    public class CountryRepositoryTests
    {
        private readonly Mock<IMySqlDataService> _mockDataService;
        private readonly CountryRepository _repository;

        public CountryRepositoryTests()
        {
            _mockDataService = new Mock<IMySqlDataService>();
            _repository = new CountryRepository(_mockDataService.Object);
        }

        #region GetCountries Tests

        [Fact]
        public void GetCountries_WithValidData_ReturnsListOfCountries()
        {
            // Arrange
            var dataSet = CreateCountriesDataSet(
                (1, "Mexico"),
                (2, "United States")
            );

            _mockDataService.Setup(x => x.ExecSP("GetCountries"))
                .Returns(dataSet);

            // Act
            var result = _repository.GetCountries();

            // Assert
            Assert.NotNull(result);
            var countries = result.ToList();

            Assert.Equal(2, countries.Count);
            Assert.Equal("Mexico", countries[0].CountryName);
            Assert.Equal("United States", countries[1].CountryName);
        }

        #endregion

        #region GetCountryById Tests

        [Fact]
        public void GetCountryById_WithValidId_ReturnsCountry()
        {
            // Arrange
            var dataSet = CreateCountriesDataSet((1, "Mexico"));

            _mockDataService.Setup(x =>
                x.ExecSP(
                    "GetCountryById",
                    It.IsAny<IEnumerable<MySqlParameter>>()
                ))
                .Returns(dataSet);

            // Act
            var result = _repository.GetCountryById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.CountryId);
            Assert.Equal("Mexico", result.CountryName);
        }

        #endregion

        #region InsertCountry Tests

        [Fact]
        public void InsertCountry_WithValidData_ReturnsTrue()
        {
            // Arrange
            var country = new Country
            {
                CountryName = "France"
            };

            var dataSet = new DataSet();
            var table = new DataTable();

            table.Columns.Add("newCountryId", typeof(int));
            table.Rows.Add(6);

            dataSet.Tables.Add(table);

            _mockDataService.Setup(x =>
                x.ExecSP(
                    "InsertCountry",
                    It.IsAny<IEnumerable<MySqlParameter>>()
                ))
                .Returns(dataSet);

            // Act
            var result = _repository.InsertCountry(country);

            // Assert
            Assert.True(result);
            Assert.Equal(6, country.CountryId);
        }

        #endregion

        #region UpdateCountry Tests

        [Fact]
        public void UpdateCountry_WithValidData_ReturnsTrue()
        {
            // Arrange
            var country = new Country
            {
                CountryId = 1,
                CountryName = "Mexico Updated"
            };

            var dataSet = new DataSet();
            var table = new DataTable();

            table.Columns.Add("affectedRows", typeof(int));
            table.Rows.Add(1);

            dataSet.Tables.Add(table);

            _mockDataService.Setup(x =>
                x.ExecSP(
                    "UpdateCountry",
                    It.IsAny<IEnumerable<MySqlParameter>>()
                ))
                .Returns(dataSet);

            // Act
            var result = _repository.UpdateCountry(country);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Helper Methods

        private static DataSet CreateCountriesDataSet(
            params (int Id, string Name)[] countries)
        {
            var dataSet = new DataSet();
            var table = new DataTable();

            table.Columns.Add("CountryId", typeof(int));
            table.Columns.Add("CountryName", typeof(string));

            foreach (var country in countries)
            {
                table.Rows.Add(country.Id, country.Name);
            }

            dataSet.Tables.Add(table);
            return dataSet;
        }

        #endregion
    }
}

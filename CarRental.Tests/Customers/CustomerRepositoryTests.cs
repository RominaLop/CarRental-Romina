
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using CarRentalServices.Repositories;
using Moq;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CarRental.Tests.Customers
{
    public class CustomerRepositoryTests
    {
        private readonly Mock<IMySqlDataService> _mockDataService;
        private readonly CustomerRepository _repository;

        public CustomerRepositoryTests()
        {
            _mockDataService = new Mock<IMySqlDataService>();
            _repository = new CustomerRepository(_mockDataService.Object);
        }

        #region GetCustomers Tests

        [Fact]
        public void GetCustomers_WithValidData_ReturnsListOfCustomers()
        {
            // Arrange
            var dataSet = CreateCustomersDataSet(
                (1, "Romina", "Lopez"),
                (2, "Ana", "Garcia")
            );

            _mockDataService.Setup(x => x.ExecSP("GetCustomers"))
                .Returns(dataSet);

            // Act
            var result = _repository.GetCustomers();

            // Assert
            Assert.NotNull(result);
            var customers = result.ToList();

            Assert.Equal(2, customers.Count);
            Assert.Equal("Romina", customers[0].FirstName);
            Assert.Equal("Ana", customers[1].FirstName);
        }

        #endregion

        #region GetCustomerById Tests

        [Fact]
        public void GetCustomerById_WithValidId_ReturnsCustomer()
        {
            // Arrange
            var dataSet = CreateCustomersDataSet(
                (1, "Romina", "Lopez")
            );

            _mockDataService.Setup(x =>
                x.ExecSP(
                    "GetCustomerById",
                    It.IsAny<IEnumerable<MySqlParameter>>()
                ))
                .Returns(dataSet);

            // Act
            var result = _repository.GetCustomerById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.IdCustomer);
            Assert.Equal("Romina", result.FirstName);
            Assert.Equal("Lopez", result.LastName);
        }

        #endregion

        #region InsertCustomer Tests

        [Fact]
        public void InsertCustomer_WithValidData_ReturnsTrue()
        {
            // Arrange
            var customer = new Customer
            {
                FirstName = "Carlos",
                LastName = "Perez",
                SocialSecurityNumber = "TEST123",
                Email = "carlos@test.com",
                CountryName = "Mexico",
                CountryId = 1,
                MobilePhone = "9981234567",
                StateName = "Quintana Roo"
            };

            var dataSet = new DataSet();
            var table = new DataTable();

            table.Columns.Add("newCustomerId", typeof(int));
            table.Rows.Add(10);

            dataSet.Tables.Add(table);

            _mockDataService.Setup(x =>
                x.ExecSP(
                    "InsertCustomer",
                    It.IsAny<IEnumerable<MySqlParameter>>()
                ))
                .Returns(dataSet);

            // Act
            var result = _repository.InsertCustomer(customer);

            // Assert
            Assert.True(result);
            Assert.Equal(10, customer.IdCustomer);
        }

        #endregion

        #region UpdateCustomer Tests

        [Fact]
        public void UpdateCustomer_WithValidData_ReturnsTrue()
        {
            // Arrange
            var customer = new Customer
            {
                IdCustomer = 1,
                FirstName = "Romina",
                LastName = "Lopez",
                SocialSecurityNumber = "TEST123",
                Email = "romina@test.com",
                CountryName = "Mexico",
                CountryId = 1,
                MobilePhone = "9987654321",
                StateName = "Quintana Roo"
            };

            var dataSet = new DataSet();
            var table = new DataTable();

            table.Columns.Add("affectedRows", typeof(int));
            table.Rows.Add(1);

            dataSet.Tables.Add(table);

            _mockDataService.Setup(x =>
                x.ExecSP(
                    "UpdateCustomer",
                    It.IsAny<IEnumerable<MySqlParameter>>()
                ))
                .Returns(dataSet);

            // Act
            var result = _repository.UpdateCustomer(customer);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Helper Methods

        private static DataSet CreateCustomersDataSet(
            params (int Id, string FirstName, string LastName)[] customers)
        {
            var dataSet = new DataSet();
            var table = new DataTable();

            table.Columns.Add("IdCustomer", typeof(int));
            table.Columns.Add("FirstName", typeof(string));
            table.Columns.Add("LastName", typeof(string));
            table.Columns.Add("SocialSecurityNumber", typeof(string));
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("CountryName", typeof(string));
            table.Columns.Add("CountryId", typeof(int));
            table.Columns.Add("MobilePhone", typeof(string));
            table.Columns.Add("StateName", typeof(string));

            foreach (var customer in customers)
            {
                table.Rows.Add(
                    customer.Id,
                    customer.FirstName,
                    customer.LastName,
                    "TEST123",
                    "test@test.com",
                    "Mexico",
                    1,
                    "9981234567",
                    "Quintana Roo"
                );
            }

            dataSet.Tables.Add(table);
            return dataSet;
        }

        #endregion
    }
}

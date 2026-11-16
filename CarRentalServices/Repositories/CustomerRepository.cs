
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;

namespace CarRentalServices.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMySqlDataService _dataService;

        public CustomerRepository(IMySqlDataService dataService)
        {
            _dataService = dataService;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            var data = _dataService.ExecSP("GetCustomers");

            if (data != null && data.Tables.Count > 0)
            {
                return MapCustomers(data.Tables[0]);
            }

            return new List<Customer>();
        }

        public Customer? GetCustomerById(int id)
        {
            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("p_IdCustomer", MySqlDbType.Int32)
                {
                    Value = id
                }
            };

            var data = _dataService.ExecSP("GetCustomerById", parameters);

            if (data != null && data.Tables.Count > 0 &&
                data.Tables[0].Rows.Count > 0)
            {
                return MapCustomer(data.Tables[0].Rows[0]);
            }

            return null;
        }

        public bool InsertCustomer(Customer customer)
        {
            var parameters = CreateParameters(customer, false);

            var result = _dataService.ExecSP("InsertCustomer", parameters);

            if (result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
            {
                customer.IdCustomer = Convert.ToInt32(
                    result.Tables[0].Rows[0]["newCustomerId"]);
            }

            return customer.IdCustomer > 0;
        }

        public bool UpdateCustomer(Customer customer)
        {
            var parameters = CreateParameters(customer, true);

            var result = _dataService.ExecSP("UpdateCustomer", parameters);

            return result.Tables.Count > 0 &&
                   result.Tables[0].Rows.Count > 0 &&
                   Convert.ToInt32(result.Tables[0].Rows[0]["affectedRows"]) > 0;
        }

        public bool DeleteCustomer(int id)
        {
            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("p_IdCustomer", MySqlDbType.Int32)
                {
                    Value = id
                }
            };

            var result = _dataService.ExecSP("DeleteCustomer", parameters);

            return result.Tables.Count > 0 &&
                   result.Tables[0].Rows.Count > 0 &&
                   Convert.ToInt32(result.Tables[0].Rows[0]["affectedRows"]) > 0;
        }

        private List<Customer> MapCustomers(DataTable dataTable)
        {
            var customers = new List<Customer>();

            foreach (DataRow row in dataTable.Rows)
            {
                customers.Add(MapCustomer(row));
            }

            return customers;
        }

        private Customer MapCustomer(DataRow row)
        {
            return new Customer
            {
                IdCustomer = Convert.ToInt32(row["IdCustomer"]),
                FirstName = Convert.ToString(row["FirstName"]) ?? "",
                LastName = Convert.ToString(row["LastName"]) ?? "",
                SocialSecurityNumber = Convert.ToString(row["SocialSecurityNumber"]) ?? "",
                Email = Convert.ToString(row["Email"]) ?? "",
                CountryName = Convert.ToString(row["CountryName"]) ?? "",
                CountryId = Convert.ToInt32(row["CountryId"]),
                MobilePhone = Convert.ToString(row["MobilePhone"]) ?? "",
                StateName = Convert.ToString(row["StateName"]) ?? ""
            };
        }

        private List<MySqlParameter> CreateParameters(Customer customer, bool includeId)
        {
            var parameters = new List<MySqlParameter>
            {
                new MySqlParameter("p_FirstName", MySqlDbType.VarChar)
                {
                    Value = customer.FirstName
                },
                new MySqlParameter("p_LastName", MySqlDbType.VarChar)
                {
                    Value = customer.LastName
                },
                new MySqlParameter("p_SocialSecurityNumber", MySqlDbType.VarChar)
                {
                    Value = customer.SocialSecurityNumber
                },
                new MySqlParameter("p_Email", MySqlDbType.VarChar)
                {
                    Value = customer.Email
                },
                new MySqlParameter("p_CountryId", MySqlDbType.Int32)
                {
                    Value = customer.CountryId
                },
                new MySqlParameter("p_CountryName", MySqlDbType.VarChar)
                {
                    Value = customer.CountryName
                },
                new MySqlParameter("p_MobilePhone", MySqlDbType.VarChar)
                {
                    Value = customer.MobilePhone
                },
                new MySqlParameter("p_StateName", MySqlDbType.VarChar)
                {
                    Value = customer.StateName
                }
            };

            if (includeId)
            {
                parameters.Add(new MySqlParameter("p_IdCustomer", MySqlDbType.Int32)
                {
                    Value = customer.IdCustomer
                });
            }

            return parameters;
        }
    }
}

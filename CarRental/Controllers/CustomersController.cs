
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var lstCustomers = _customerRepository.GetCustomers();
            return Ok(lstCustomers);
        }

        [HttpGet("{id}")]
        public IActionResult GetCustomerById(int id)
        {
            var customer = _customerRepository.GetCustomerById(id);

            if (customer == null)
                return NotFound();

            return Ok(customer);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Customer customer)
        {
            var result = _customerRepository.InsertCustomer(customer);

            if (!result)
                return BadRequest("No fue posible registrar el cliente.");

            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = customer.IdCustomer },
                customer
            );
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Customer customer)
        {
            if (id != customer.IdCustomer)
                return BadRequest("El ID de la ruta no coincide con el ID del cliente.");

            var result = _customerRepository.UpdateCustomer(customer);

            return result ? Ok(customer) : NotFound();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _customerRepository.DeleteCustomer(id);

            return result ? Ok("Cliente eliminado correctamente.") : NotFound();
        }
    }
}

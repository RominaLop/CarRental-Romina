
using CarRentalServices.Contracts;
using CarRentalServices.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountryRepository _countryRepository;

        public CountriesController(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var lstCountries = _countryRepository.GetCountries();
            return Ok(lstCountries);
        }

        [HttpGet("{id}")]
        public IActionResult GetCountryById(int id)
        {
            var country = _countryRepository.GetCountryById(id);

            if (country == null)
                return NotFound();

            return Ok(country);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Country country)
        {
            var result = _countryRepository.InsertCountry(country);

            if (!result)
                return BadRequest("No fue posible registrar el país.");

            return CreatedAtAction(
                nameof(GetCountryById),
                new { id = country.CountryId },
                country
            );
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Country country)
        {
            if (id != country.CountryId)
                return BadRequest("El ID de la ruta no coincide con el ID del país.");

            var result = _countryRepository.UpdateCountry(country);

            return result ? Ok(country) : NotFound();
        }
    }
}

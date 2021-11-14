using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyEcommerce.Api.Entities;
using MyEcommerce.Api.Repositories.Interface;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MyEcommerce.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [Description("Customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerRepository _customerRepository;

        public CustomerController(
            ILogger<CustomerController> logger,
            ICustomerRepository customerRepository)
        {
            _logger = logger;
            _customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _customerRepository.GetAll();
            
            return customers == null || !customers.Any()
                ? NotFound()
                : Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if(id <= 0)
                return BadRequest("Invalid customer id");

            var customer = await _customerRepository.GetById(id);
            
            return customer == null
                ? NotFound()
                : Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            await _customerRepository.Create(customer);

            return CreatedAtAction(
                nameof(GetById),
                new { id = customer.Id },
                customer
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer customer)
        {
            if(id <= 0)
                return BadRequest("Invalid customer id");

            var currentCustomer = await _customerRepository.GetById(id);
            if(currentCustomer == null)
                return NotFound();

            await _customerRepository.Update(id, customer);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if(id <= 0)
                return BadRequest("Invalid customer id");

            var currentCustomer = await _customerRepository.GetById(id);
            if(currentCustomer == null)
                return NotFound();

            await _customerRepository.Delete(id);

            return NoContent();
        }
    }
}

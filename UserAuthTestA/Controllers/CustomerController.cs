using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace UserAuthTestA.Controllers
{
    [ApiController]
    [Route("customers")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDB db;
        public IMapper mapper;
        private readonly IConfiguration configuration;

        public CustomerController(ApplicationDB db, IMapper mapper, IConfiguration configuration)
        {
            this.db = db;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<DTOs.CustomerDTO>> Get()
        {
            var customers = await db.Customers.ToListAsync();
            return mapper.Map<List<DTOs.CustomerDTO>>(customers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DTOs.CustomerDTO>> Get(int id)
        {
            var customer = await db.Customers.FirstOrDefaultAsync(x => x.ID == id);
            if (customer == null) return NotFound();
            var customerDTO = new DTOs.CustomerDTO();
            customerDTO.ID = customer.ID;
            customerDTO.Name = customer.Name;
            customerDTO.BusinessName= customer.BusinessName;
            return customerDTO;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<DTOs.CustomerDTO>> Get(string name)
        {
            var customer = await db.Customers.FirstOrDefaultAsync(x => x.Name.Contains(name));
            if (customer == null)
            {
                return NotFound();
            }

            var customerDTO = new DTOs.CustomerDTO();
            customerDTO.ID = customer.ID;
            customerDTO.Name = customer.Name;
            customerDTO.BusinessName = customer.BusinessName;

            return customerDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(DTOs.CustomerDTO customerDTO)
        {
            var customer = new Model.Customer();
            customer.BusinessName = customerDTO.BusinessName;
            customer.Name = customerDTO.Name;
            db.Add(customer);
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(DTOs.CustomerDTO customerDTO, int id)
        {
            if (customerDTO.ID != id)
            {
                return BadRequest("Customer id does not match with the id in the url");
            }

            var customer = await db.Customers.FirstOrDefaultAsync(x => x.ID == id);
            if (customer == null)
            {
                return NotFound();
            }

            customer.BusinessName = customerDTO.BusinessName;
            customer.Name = customerDTO.Name;
            db.Update(customer);
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await db.Customers.AnyAsync(x => x.ID == id);
            if (!exists)
            {
                return NotFound();
            }
            db.Remove(new Model.Customer() { ID = id });
            await db.SaveChangesAsync();
            return Ok();
        }

    }
}

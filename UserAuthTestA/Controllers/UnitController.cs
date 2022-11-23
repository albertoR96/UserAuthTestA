using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using UserAuthTestA;

namespace UserAuthTestA.Controllers
{
    [ApiController]
    [Route("api/units")]
    public class UnitController : ControllerBase
    {
        private readonly ApplicationDB db;

        public UnitController(ApplicationDB db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<List<Model.Unit>> Get()
        {
            return await db.Units.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Model.Unit>> Get(int id)
        {
            var unit = await db.Units.FirstOrDefaultAsync(x => x.ID == id);
            if (unit == null)
            {
                return NotFound();
            }
            return unit;
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Model.Unit>> Get(string name)
        {
            var unit = await db.Units.FirstOrDefaultAsync(x => x.Name.Contains(name));
            if (unit == null)
            {
                return NotFound();
            }
            return unit;
        }

        [HttpGet("{customer:int}")]
        public async Task<List<Model.Unit>> GetByCustomer(int customerID)
        {
            return (await db.Customers.FirstOrDefaultAsync(x => x.ID == customerID)).Units;
        }

        [HttpPost]
        public async Task<ActionResult> Post(Model.Unit unit)
        {
            db.Add(unit);
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Put(Model.Unit unit, int id)
        {
            if (unit.ID != id)
            {
                return BadRequest("Unit id does not match with the id in the url");
            }
            db.Update(unit);
            await db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await db.Units.AnyAsync(x => x.ID == id);
            if (!exists)
            {
                return NotFound();
            }
            db.Remove(new Model.Unit() { ID = id });
            await db.SaveChangesAsync();
            return Ok();
        }

    }
}

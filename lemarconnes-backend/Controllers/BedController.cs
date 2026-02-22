using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeMarconnes.Data;
using LeMarconnes.Entities; // Voor de User entiteit
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LeMarconnesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BedsController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BedsController(ApplicationDbContext context, UserManager<User> userManager)
            : base(userManager)
        {
            _context = context;
        }

        // GET: api/Beds
        // Openbaar toegankelijk (nodig voor het boekingsformulier en admin create)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bed>>> GetBeds()
        {
            return await _context.Beds.ToListAsync();
        }

        // GET: api/Beds/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Bed>> GetBed(int id)
        {
            var bed = await _context.Beds.FindAsync(id);
            if (bed == null) return NotFound();

            return bed;
        }

        // POST: api/Beds
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Bed>> PostBed(Bed bed)
        {
            if (!await IsAdmin()) return Forbid();

            _context.Beds.Add(bed);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBed), new { id = bed.Id }, bed);
        }

        // PUT: api/Beds/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBed(int id, Bed bed)
        {
            if (!await IsAdmin()) return Forbid();

            if (id != bed.Id) return BadRequest();

            // Haal het bestaande bed uit de context
            var existingBed = await _context.Beds.FindAsync(id);
            if (existingBed == null) return NotFound();

            // Update properties
            existingBed.Type = bed.Type;
            existingBed.numberOfPeople = bed.numberOfPeople;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Beds/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBed(int id)
        {
            if (!await IsAdmin()) return Forbid();

            var bed = await _context.Beds.FindAsync(id);
            if (bed == null) return NotFound();

            _context.Beds.Remove(bed);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BedExists(int id)
        {
            return _context.Beds.Any(e => e.Id == id);
        }
    }
}
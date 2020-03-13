using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ABATON.Models;

namespace ABATON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DosagesController : ControllerBase
    {
        private readonly DosageContext _context;

        public DosagesController(DosageContext context)
        {
            _context = context;
        }

        // GET: api/Dosages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Dosage>>> GetDosages()
        {
            return await _context.Dosages.ToListAsync();
        }

        // GET: api/Dosages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Dosage>> GetDosage(long id)
        {
            var dosage = await _context.Dosages.FindAsync(id);

            if (dosage == null)
            {
                return NotFound();
            }

            return dosage;
        }

        // PUT: api/Dosages/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDosage(long id, Dosage dosage)
        {
            if (id != dosage.Id)
            {
                return BadRequest();
            }

            _context.Entry(dosage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DosageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Dosages
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Dosage>> PostDosage(Dosage dosage)
        {
            _context.Dosages.Add(dosage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDosage", new { id = dosage.Id }, dosage);
        }

        // DELETE: api/Dosages/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Dosage>> DeleteDosage(long id)
        {
            var dosage = await _context.Dosages.FindAsync(id);
            if (dosage == null)
            {
                return NotFound();
            }

            _context.Dosages.Remove(dosage);
            await _context.SaveChangesAsync();

            return dosage;
        }

        private bool DosageExists(long id)
        {
            return _context.Dosages.Any(e => e.Id == id);
        }
    }
}

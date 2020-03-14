using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ABATON.Models;
using ABATON.Services;

namespace ABATON.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class drugsController : ControllerBase
    {
        private readonly DrugContext _context;
        private readonly IRelationshipService _relationshipService;

        public drugsController(DrugContext context, IRelationshipService irs)
        {
            _context = context;
            _relationshipService = irs;
        }

        // GET: api/drugs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Drug>>> GetDrugs()
        {
            return await _context.Drugs.ToListAsync();
        }

        // GET: api/drugs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Drug>> GetDrug(long id)
        {
            var drug = await _context.Drugs.FindAsync(id);

            if (drug == null)
            {
                return NotFound();
            }

            return drug;
        }

        // PUT: api/drugs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDrug(long id, Drug drug)
        {
            if (id != drug.Id)
            {
                return BadRequest();
            }

            var existingdrug = await _context.Drugs.FindAsync(id);

            if (existingdrug == null)
            {
                return NotFound();
            }

            //trying to restore a drug
            if (existingdrug.Deleted && !drug.Deleted)
            {
                return BadRequest("Cannot restore a drug");
            }

            _context.Entry(existingdrug).State = EntityState.Detached;

            _context.Entry(drug).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/drugs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Drug>> PostDrug(Drug drug)
        {
            if (drug.Deleted)
            {
                return BadRequest("Cannot add a deleted drug");
            }

            _context.Drugs.Add(drug);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDrug), new { id = drug.Id }, drug);
        }

        // DELETE: api/drugs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Drug>> DeleteDrug(long id)
        {
            var drug = await _context.Drugs.FindAsync(id);
            if (drug == null)
            {
                return NotFound();
            }

            if (drug.Deleted)
            {
                return BadRequest("drug already deleted");
            }

            drug.Deleted = true;
                        
            _context.Entry(drug).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            
            await _relationshipService.DeleteDosageDrugId(id);
            
            return drug;
        }

        private bool DrugExists(long id)
        {
            return _context.Drugs.Any(e => e.Id == id && !e.Deleted);
        }
    }
}

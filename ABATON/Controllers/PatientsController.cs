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
    public class PatientsController : ControllerBase
    {
        private readonly PatientContext _context;
        private readonly IRelationshipService _relationshipService;

        public PatientsController(PatientContext context, IRelationshipService irs)
        {
            _context = context;
            _relationshipService = irs;
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            return await _context.Patients.ToListAsync();
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(long id)
        {
            var patient = await _context.Patients.FindAsync(id);        

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        // PUT: api/Patients/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(long id, Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest();
            }
            
            var existingPatient = await _context.Patients.FindAsync(id);

            if (existingPatient == null)
            {
                return NotFound();
            }          

            //trying to restore a patient
            if (existingPatient.Deleted && !patient.Deleted)
            {
                return BadRequest("Cannot restore a patient");
            }

            _context.Entry(existingPatient).State = EntityState.Detached;

            _context.Entry(patient).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Patients
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            if (patient.Deleted)
            {
                return BadRequest("Cannot add a deleted patient");
            }

            _context.Patients.Add(patient);           

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Patient>> DeletePatient(long id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            if (patient.Deleted)
            {
                return BadRequest("Patient already deleted");
            }

            patient.Deleted = true;
                       
            _context.Entry(patient).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            await _relationshipService.DeleteDosagePatientId(id);

            return patient;
        }

        private bool PatientExists(long id)
        {
            return _context.Patients.Any(e => e.Id == id && !e.Deleted);
        }
    }
}

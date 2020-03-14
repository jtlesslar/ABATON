using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABATON.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


namespace ABATON.Services
{
    public class RelationshipService:IRelationshipService
    {
        private readonly PatientContext _patientContext;
        private readonly DrugContext _drugContext;
        private readonly DosageContext _dosageContext;

        public RelationshipService(PatientContext patc,DrugContext drugc,DosageContext dosagec)
        {
            _patientContext = patc;
            _drugContext = drugc;
            _dosageContext = dosagec;
        }

        public async Task<ActionResult<Patient>> DeletePatient(long id)
        {
            var patient = await _patientContext.Patients.FindAsync(id);
                        
            patient.Deleted = true;                      

            _patientContext.Entry(patient).State = EntityState.Modified;
            
            await _patientContext.SaveChangesAsync();

            return patient;
        }

        public async Task<ActionResult<Drug>> DeleteDrug(long id)
        {
            var drug = await _drugContext.Drugs.FindAsync(id);

            drug.Deleted = true;

            _drugContext.Entry(drug).State = EntityState.Modified;

            await _drugContext.SaveChangesAsync();

            return drug;
        }
        
        public bool CheckPatientExists(long id)
        {
            return _patientContext.Patients.Any(e => e.Id == id && !e.Deleted);
        }

        public bool CheckDrugExists(long id)
        {
            return _drugContext.Drugs.Any(e => e.Id == id && !e.Deleted);
        }

        public async Task<ActionResult<bool>> DeleteDosagePatientId(long id)
        {
            var dosage = await _dosageContext.Dosages.Where(dose => dose.PatientId == id && !dose.Deleted).ToListAsync();

            foreach (var dose in dosage)
            {
                dose.Deleted = true;

                _dosageContext.Entry(dose).State = EntityState.Modified;
            }           

            await _dosageContext.SaveChangesAsync();

            return true;
        }

        public async Task<ActionResult<bool>> DeleteDosageDrugId(long id)
        {
            var dosage = await _dosageContext.Dosages.Where(dose => dose.DrugId == id && !dose.Deleted).ToListAsync();

            foreach (var dose in dosage)
            {
                dose.Deleted = true;

                _dosageContext.Entry(dose).State = EntityState.Modified;
            }          

            await _dosageContext.SaveChangesAsync();

            return true;
        }

    }
}

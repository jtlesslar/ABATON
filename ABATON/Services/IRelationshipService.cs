using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABATON.Models;
using Microsoft.AspNetCore.Mvc;

namespace ABATON.Services
{
    public interface IRelationshipService
    {
        Task<ActionResult<Patient>> DeletePatient(long id);
        Task<ActionResult<Drug>> DeleteDrug(long id);
        Task<ActionResult<bool>> DeleteDosagePatientId(long id);
        Task<ActionResult<bool>> DeleteDosageDrugId(long id);
        bool CheckPatientExists(long id);
        bool CheckDrugExists(long id);
    }
}

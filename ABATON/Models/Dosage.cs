using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABATON.Models
{
    public class Dosage
    {
        public long Id { get; set; }
        public float MgPerDay { get; set; }
        public long DrugId { get; set; }
        public long PatientId { get; set; }
        public bool Deleted { get; set; }
    }
}

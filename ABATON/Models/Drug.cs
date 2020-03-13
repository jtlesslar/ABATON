using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ABATON.Models
{
    public class Drug
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
    }
}

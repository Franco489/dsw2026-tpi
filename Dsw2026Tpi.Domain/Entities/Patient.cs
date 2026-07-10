using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public class Patient: EntityBase
    {
        public string Dni { get; set; }
        public string Name { get; set; }
        public string PhoneNumber {  get; set; }
    }
}

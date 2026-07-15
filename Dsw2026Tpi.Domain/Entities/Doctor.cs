using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Dsw2026Tpi.Domain.Entities
{
    public class Doctor: EntityBase
    {
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public Speciality Speciality { get; set; }
        public Guid SpecialityId { get; set; }
        public List<Availability> AvailabilityRules { get; set; }
        protected Doctor()
        {
            
        }

        public Doctor(string name, string licenseNumber, Guid specialityId)
        {
            Name = name;
            LicenseNumber = licenseNumber;
            SpecialityId = specialityId;
        }

        public void Update(Guid specialityId = default, string name=null, string licenseNumber=null)
        {
            Name = name ?? Name;
            LicenseNumber = licenseNumber ?? LicenseNumber;
            SpecialityId = specialityId;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public class Specialty: EntityBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        //ICollection<Doctor> Doctors { get; set; } = [];

        public Specialty(string name, string description) 
        {
            Name = name;
            Description = description;
        }

        public void Update(string newName = null, string newDescription = null)
        {

            if (!string.IsNullOrWhiteSpace(newName))
            {
                this.Name = newName;
            }

            if (!string.IsNullOrWhiteSpace(newDescription))
            {
                this.Description = newDescription;
            }
        }
    }
}

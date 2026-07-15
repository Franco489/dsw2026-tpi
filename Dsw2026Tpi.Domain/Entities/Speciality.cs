using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public class Speciality: EntityBase
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; } = true;
        //ICollection<Doctor> Doctors { get; set; } = [];
        protected Speciality()
        {
            
        }
        public Speciality(string name, string description, Guid? id=null) : base(id)
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

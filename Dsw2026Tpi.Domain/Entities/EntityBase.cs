using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Domain.Entities
{
    public abstract class EntityBase
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool Deleted { get; set; } = false;
        protected EntityBase(Guid? id = null)
        {
            //TODO: Esto no recuerdo si venía en la plantilla del profe o lo copiamos de la nuestra. Resulta que esto genera conlfictos con EF en ciertos casos.
            //Id = id ?? Guid.NewGuid(); 
        }

        public void UpdateEntity(DateTime updateDate)
        {
            if (updateDate.CompareTo(UpdatedAt) <= 0) //TODO: Debería tirar una exception?
            {
                //throw new DateValidationException(); 
            }
        }
        public void Delete()
        {
            Deleted = true;
        }

        public void Activate()
        {
            Deleted = false;
        }
    }
}

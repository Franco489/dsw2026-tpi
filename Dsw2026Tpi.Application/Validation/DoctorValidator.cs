using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Validation
{
    public class DoctorValidator
    {
        public static void Validate(DoctorModel.Request request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ValidationException("El nombre no puede ser nulo o vacío.", ErrorCodes.VALIDATION_ERROR);
            }
            
            if (request.SpecialityId == Guid.Empty)
            {
                throw new ValidationException("El ID de la especialidad no puede estar vacío.", ErrorCodes.VALIDATION_ERROR);
            }

            if(request.Name.Length > 100 || request.Name.Length <3)
            {
                throw new ValidationException("El nombre no puede tener más de 100 caracteres.", ErrorCodes.VALIDATION_ERROR);
            }
        }
    }
}

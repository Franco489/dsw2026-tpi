using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;

using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Validation
{
    public class SpecialityValidator
    {
        public static void Validate(SpecialityModel.UpdateRequest request)
        {
            if(request==null)
            {
                throw new ValidationException("La especialidad no puede ser nula", ErrorCodes.VALIDATION_ERROR);
            }
            if (string.IsNullOrWhiteSpace(request.name))
            {
                throw new ValidationException("La especialidad debe tener un nombre", ErrorCodes.VALIDATION_ERROR);
            }

            if (string.IsNullOrWhiteSpace(request.description))
            {
                throw new ValidationException("La especialidad debe tener una descripción", ErrorCodes.VALIDATION_ERROR);
            }

            if (request.name.Length > 100 || request.name.Length <3)
            {
                throw new ValidationException("El nombre de la especialidad debe estar entre los 3 y 100 caracteres", ErrorCodes.VALIDATION_ERROR);
            }

            if (request.description.Length > 100 || request.description.Length < 10)
            {
                throw new ValidationException("La descripcion de la especialidad debe estar entre los 10 y 100 caracteres", ErrorCodes.VALIDATION_ERROR);
            }

        }
    }
}

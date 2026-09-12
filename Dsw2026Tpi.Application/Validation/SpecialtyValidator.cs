using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Resources;

using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Validation
{
    public class SpecialtyValidator
    {
        
        public static void Validate(SpecialtyModel.Request request)
        {
            var errores = new List<(string, string)>(); // Una lista de tuplas, (campo, descripcion de su error)

            if (request==null)
            {
                errores.Add( ("request","La especialidad no puede ser nula") );
            }

            if (string.IsNullOrWhiteSpace(request.name))
            {
                errores.Add(("name","La especialidad debe tener un nombre"));
            }

            if (string.IsNullOrWhiteSpace(request.description))
            {
                errores.Add( ("description","La especialidad debe tener una descripción"));
            }

            if (request.name.Length > 100 || request.name.Length <3)
            {
                errores.Add( ("name","El nombre de la especialidad debe estar entre los 3 y 100 caracteres") );
            }

            if (request.description.Length > 100 || request.description.Length < 10)
            {
                errores.Add(("description","La descripcion de la especialidad debe estar entre los 10 y 100 caracteres"));
            }

            if (errores.Any())
            {
                throw new ValidationException("Se encontraron errores en los datos enviados", nameof(ErrorCodes.VALIDATION_ERROR))
                    .WithDetail(errores);
            }
        }
    }
}

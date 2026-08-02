using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.CrossCutting.Models;
using Dsw2026Tpi.CrossCutting.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Validation;

public class DoctorValidator
{
    public static void Validate(DoctorModel.Request request)
    {
        var errores = new List<(string, string)>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errores.Add( ("Name","El nombre no puede ser nulo o vacío.") );
        }
        
        if (request.SpecialityId == Guid.Empty)
        {
            errores.Add( ("SpecialityId","El ID de la especialidad no puede estar vacío.") );
        }

        if(request.Name.Length > 100 || request.Name.Length <3)
        {
            errores.Add( ("Name" ,"La longitud del nombre debe estar entre 3 y 100 caracteres") );
        }

        if(errores.Any())
        {
          throw new ValidationException("Se encontraron errores en los datos enviados", nameof(ErrorCodes.VALIDATION_ERROR)).WithDetail(errores);
        }
    }
}

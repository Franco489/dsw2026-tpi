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
            errores.Add( ("El nombre no puede ser nulo o vacío.","VALIDATION_ERROR") );
        }
        
        if (request.SpecialityId == Guid.Empty)
        {
            errores.Add( ("El ID de la especialidad no puede estar vacío.", "VALIDATION_ERROR") );

        }

        if(request.Name.Length > 100 || request.Name.Length <3)
        {
            errores.Add( ("El nombre no puede tener más de 100 caracteres.", "VALIDATION_ERROR") );
        }

        if(errores.Any())
        {
          throw new ValidationException("Se encontraron errores en los datos enviados", nameof(ErrorCodes.VALIDATION_ERROR)).WithDetail(errores);
        }
    }
}

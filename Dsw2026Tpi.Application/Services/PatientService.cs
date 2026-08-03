using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.CrossCutting.Exceptions;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Dsw2026Tpi.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IPersistence _persistence;
        public PatientService(IPersistence persistence)
        {
            _persistence = persistence;
        }
        public async Task CompleteProfileAsync(string dni, ProfileModel.Request request)
        {
           var patient = await _persistence.First<Patient>(p => p.Dni == dni);

            if (patient == null)
            {
                throw new EntityNotFoundException($"No se encontró un paciente registrado con el DNI {dni}");
            }

            patient.Name = request.Name;
            patient.PhoneNumber = request.Phone;

            await _persistence.Update(patient);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Dsw2026Tpi.Application.Models;

namespace Dsw2026Tpi.Application.Intefaces
{
    public interface IDoctorManagementService
    {
       
        Task<IEnumerable<DoctorModel.ResponsePruebas>> GetAllDoctors();
        Task<IEnumerable<DoctorModel.Response>> GetDoctorAvailabilities(Guid id);
    }
}

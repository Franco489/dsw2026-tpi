using Dsw2026Tpi.Application.Models;
using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Intefaces
{
    public interface ISpecialityService
    {
        Task<Speciality> GetSpecialityById(Guid id);
        Task<IEnumerable<Speciality>> FilterSpecialityByName(string? name);
        Task AddSpeciality(SpecialityModel.CreateRequest request);
        Task UpdateSpeciality(Guid id,SpecialityModel.UpdateRequest request);
        Task DeleteSpeciality(Guid id);
    }
}

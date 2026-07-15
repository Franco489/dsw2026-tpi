using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Intefaces
{
    public interface ISpecialityService
    {
        Task<Speciality> GetSpecialityById(Guid id);
        Task<Pagination<SpecialityModel.FilterResponse>> FilterSpecialityByName(int pageSize, int pageIndex, string? name = null);
        Task AddSpeciality(SpecialityModel.Request request);
        Task UpdateSpeciality(Guid id,SpecialityModel.Request request);
        Task DeleteSpeciality(Guid id);
    }
}

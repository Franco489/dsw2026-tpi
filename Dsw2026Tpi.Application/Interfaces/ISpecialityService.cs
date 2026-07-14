using System;
using System.Collections.Generic;
using System.Text;
using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;

namespace Dsw2026Tpi.Application.Interfaces
{
    public interface ISpecialityService
    {
        Task<Pagination<SpecialityModel.FilterResponse>> FilterSpecialityByName(int pageSize, int pageIndex, string? name = null);
        Task AddSpeciality(SpecialityModel.CreateRequest request);
        Task UpdateSpeciality(Guid id, SpecialityModel.UpdateRequest request);
        Task RemoveSpeciality(Guid id);
    }
}

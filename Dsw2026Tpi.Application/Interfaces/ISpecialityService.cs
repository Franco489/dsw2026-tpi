using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Intefaces
{
    public interface ISpecialityService
    {
        Task<Pagination<SpecialityModel.FilterResponse>> FilterSpecialityByName(int pageSize, int pageIndex, string? name = null);
        Task<SpecialityModel.CreateResponse> AddSpeciality(SpecialityModel.Request request);
        Task<SpecialityModel.Response> UpdateSpeciality(Guid id,SpecialityModel.Request request);
        Task DeleteSpeciality(Guid id);
        Task ReactivateSpeciality(Guid id);
    }
}

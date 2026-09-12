using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Intefaces
{
    public interface ISpecialtyService
    {
        Task<Pagination<SpecialtyModel.FilterResponse>> FilterSpecialtyByName(int pageSize, int pageIndex, string? name = null);
        Task<SpecialtyModel.CreateResponse> AddSpecialty(SpecialtyModel.Request request);
        Task<SpecialtyModel.Response> UpdateSpecialty(Guid id, SpecialtyModel.Request request);
        Task DeleteSpecialty(Guid id);
        Task ReactivateSpecialty(Guid id);
    }
}

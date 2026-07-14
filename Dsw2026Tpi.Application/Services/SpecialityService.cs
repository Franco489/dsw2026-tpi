using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dsw2026Tpi.Application.Services
{
    public class SpecialityService : ISpecialityService
    {
        private readonly IPersistence _persistence;
        public SpecialityService(IPersistence persistence)
        {
            _persistence = persistence;
        }

        public async Task<Pagination<SpecialityModel.FilterResponse>> FilterSpecialityByName(int pageSize, int pageIndex, string? name = null)
        {
            //if (name == null)
            //{
            //    var spec = await _persistence.GetAll<Speciality>();
            //    return spec.Where(s => s.IsActive).Select(s => new SpecialityModel.FilterResponse(
            //        s.Id,
            //        s.Name,
            //        s.Description));
            //}
            //var result = await _persistence.GetFiltered<Speciality>(s => s.Name.Contains(name));
            //return result.Where(s => s.IsActive).Select(s => new SpecialityModel.FilterResponse(
            //    s.Id,
            //    s.Name,
            //    s.Description));

            var specialities = await _persistence.Paginate<Speciality, string>(pageSize, pageIndex, s => string.IsNullOrWhiteSpace(name) ||
                                                   s.Name.Contains(name), x => x.Name);
            return specialities.Map(s => new SpecialityModel.FilterResponse(
                s.Id,
                s.Name,
                s.Description));

        }

        public async Task AddSpeciality(SpecialityModel.CreateRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateSpeciality(Guid id, SpecialityModel.UpdateRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteSpeciality(Guid id)
        {
            throw new NotImplementedException();
        }   
    }
}

using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Application.Interfaces;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
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
            var specialities = await _persistence.Paginate<Speciality, string>(pageSize, pageIndex, s => string.IsNullOrWhiteSpace(name) ||
                                                   s.Name.Contains(name), x => x.Name);
            return specialities.Map(s => new SpecialityModel.FilterResponse(
                s.Id,
                s.Name,
                s.Description));
        }

        public async Task AddSpeciality(SpecialityModel.CreateRequest request)
        {
            var speciality = new Speciality(request.name, request.description);
            await _persistence.Add<Speciality>(speciality);
        }

        public async Task UpdateSpeciality(Guid id, SpecialityModel.UpdateRequest request)
        {
            var speciality = await _persistence.GetById<Speciality>(id);
            if(speciality == null)
            {
                throw new EntityNotFoundException($"{id}");
            }
            speciality.Update(request.name, request.description);
            await _persistence.Update<Speciality>(speciality);
        }

        public async Task RemoveSpeciality(Guid id)
        {
            var speciality = await _persistence.GetById<Speciality>(id);
            if(speciality ==null)
            {
                throw new EntityNotFoundException($"{id}");
            }
            speciality.Delete();
            await _persistence.Update<Speciality>(speciality);
        }   
    }
}

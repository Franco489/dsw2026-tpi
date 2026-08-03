using Dsw2026Tpi.Application.Intefaces;
using Dsw2026Tpi.Application.Dtos;
using Dsw2026Tpi.Domain.Entities;
using Dsw2026Tpi.Domain.Interfaces;
using Dsw2026Tpi.CrossCutting.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Dsw2026Tpi.Application.Validation;

namespace Dsw2026Tpi.Application.Services
{
    public class SpecialityService : ISpecialityService
    {
        private readonly IPersistence _persistence;
        public SpecialityService(IPersistence persistence)
        {
            _persistence = persistence;
        }
        public async Task<SpecialityModel.CreateResponse> AddSpeciality(SpecialityModel.Request request)
        {
            SpecialityValidator.Validate(request);
            var NewSpeciality = new Speciality(request.name, request.description);
            await _persistence.Add<Speciality>(NewSpeciality);
            return new SpecialityModel.CreateResponse(NewSpeciality.Id, NewSpeciality.Name, NewSpeciality.Description);
        }

        public async Task DeleteSpeciality(Guid id)
        {
            var speciality = await _persistence.GetById<Speciality>(id);
            if(speciality == null)
            {
                throw new EntityNotFoundException(nameof(Speciality));
            }
            speciality.Delete();
            await _persistence.Update(speciality);
        }
        

        public async Task<SpecialityModel.Response> UpdateSpeciality(Guid id, SpecialityModel.Request request)
        {
            var speciality = await _persistence.GetById<Speciality>(id);
            if (speciality is null)
            {
                throw new EntityNotFoundException(nameof(Speciality));
            }
            SpecialityValidator.Validate(request);
            speciality.Update(request.name, request.description);
            await _persistence.Update<Speciality>(speciality);
            return new SpecialityModel.Response(speciality.Name, speciality.Description);
        }


        public async Task<Pagination<SpecialityModel.FilterResponse>> FilterSpecialityByName(int pageSize, int pageIndex, string? name = null)
        {
            var specialities = await _persistence.Paginate<Speciality, string>(pageSize, pageIndex, s => string.IsNullOrWhiteSpace(name) && s.IsActive && !s.Deleted ||
                                                   s.Name.Contains(name) && s.IsActive && !s.Deleted, x => x.Name);
            return specialities.Map(s => new SpecialityModel.FilterResponse(
                s.Id,
                s.Name,
                s.Description));
        }

        public async Task ReactivateSpeciality(Guid id)
        {
            var speciality = await _persistence.GetById<Speciality>(id);
            if (speciality is null)
            {
                throw new EntityNotFoundException(nameof(Speciality));
            }
            speciality.Activate();
            await _persistence.Update<Speciality>(speciality);
        }
    }
}

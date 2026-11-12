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
    public class SpecialtyService : ISpecialtyService
    {
        private readonly IPersistence _persistence;
        public SpecialtyService(IPersistence persistence)
        {
            _persistence = persistence;
        }
        public async Task<SpecialtyModel.CreateResponse> AddSpecialty(SpecialtyModel.Request request)
        {
            SpecialtyValidator.Validate(request);
            var NewSpecialty = new Specialty(request.name, request.description);
            await _persistence.Add<Specialty>(NewSpecialty);
            return new SpecialtyModel.CreateResponse(NewSpecialty.Id, NewSpecialty.Name, NewSpecialty.Description);
        }

        public async Task DeleteSpecialty(Guid id)
        {
            var specialty = await _persistence.GetById<Specialty>(id);
            if(specialty == null)
            {
                throw new EntityNotFoundException(nameof(Specialty));
            }
            specialty.Delete();
            await _persistence.Update(specialty);
        }
        

        public async Task<SpecialtyModel.Response> UpdateSpecialty(Guid id, SpecialtyModel.Request request)
        {
            var specialty = await _persistence.GetById<Specialty>(id);
            if (specialty is null)
            {
                throw new EntityNotFoundException(nameof(Specialty));
            }
            SpecialtyValidator.Validate(request);
            specialty.Update(request.name, request.description);
            await _persistence.Update<Specialty>(specialty);
            return new SpecialtyModel.Response(specialty.Name, specialty.Description);
        }


        public async Task<Pagination<SpecialtyModel.FilterResponse>> FilterSpecialtyByName(int pageSize, int pageIndex, string? name = null)
        {
            var specialties = await _persistence.Paginate<Specialty, string>(pageSize, pageIndex, s => string.IsNullOrWhiteSpace(name) && s.IsActive && !s.Deleted ||
                                                   s.Name.Contains(name) && s.IsActive && !s.Deleted, x => x.Name);
            return specialties.Map(s => new SpecialtyModel.FilterResponse(
                s.Id,
                s.Name,
                s.Description));
        }

        public async Task ReactivateSpecialty(Guid id)
        {
            var specialty = await _persistence.GetById<Specialty>(id);
            if (specialty is null)
            {
                throw new EntityNotFoundException(nameof(Specialty));
            }
            specialty.Activate();
            await _persistence.Update<Specialty>(specialty);
        }
    }
}

using Dsw2026Tpi.Application.Intefaces;
using Dsw2026Tpi.Application.Models;
using Dsw2026Tpi.Data.Interface;
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
        public async Task AddSpeciality(SpecialityModel.CreateRequest request)
        {
            var NewSpeciality = new Speciality(request.name, request.description);
            await _persistence.Add<Speciality>(NewSpeciality);
        }

        public async Task DeleteSpeciality(Guid id)
        {
            var speciality = await _persistence.GetById<Speciality>(id);
            speciality.Delete();
            _persistence.Update(speciality);
        }
        public Task<Speciality> GetSpecialityById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateSpeciality(Guid id,SpecialityModel.UpdateRequest request)
        {
            var speciality = await _persistence.GetById<Speciality>(id);
            speciality.Update(request.name, request.description);
            await _persistence.Update<Speciality>(speciality);   
        }

        public async Task<IEnumerable<Speciality>> FilterSpecialityByName(string? name)
        {
            if (name == null) 
            {
                return await _persistence.GetAll<Speciality>();
            }
            var result = await _persistence.GetFiltered<Speciality>(s => s.Name.Contains(name));
            result.Where(s => s.IsActive).Select(s => new SpecialityModel.FilterResponse(
                s.Id,
                s.Name, 
                s.Description));
           
            return result;
            
        }
    }
}

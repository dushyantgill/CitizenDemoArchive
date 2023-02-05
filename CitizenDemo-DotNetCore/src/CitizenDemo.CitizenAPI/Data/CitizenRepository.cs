﻿using CitizenDemo.CitizenAPI.Models;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenDemo.CitizenAPI.Data
{
    public class CitizenRepository : ICitizenRepository
    {
        private readonly CitizenContext _context;

        public CitizenRepository(IDatabaseSettings settings)
        {
            _context = new CitizenContext(settings);
        }

        public async Task<IEnumerable<Citizen>> GetCitizens()
        {
            return await _context.Citizens.Find(_ => true).Limit(100).ToListAsync();
        }

        public async Task<Citizen> GetCitizen(string citizenId)
        {
            var citizenIdFilter = Builders<Citizen>.Filter.Eq(c => c.CitizenId, citizenId);

            return await _context.Citizens.Find(citizenIdFilter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Citizen>> SearchCitizens(string name, string postalCode, string city, string state, string country)
        {
            var searchFilter = Builders<Citizen>.Filter.Empty;
            var givenNameFilter = Builders<Citizen>.Filter.Eq(c => c.GivenName, name);
            var surnameFilter = Builders<Citizen>.Filter.Eq(c => c.Surname, name);
            var nameFilter = givenNameFilter | surnameFilter;
            var postalCodeFilter = Builders<Citizen>.Filter.Eq(c => c.PostalCode, postalCode);
            var cityFilter = Builders<Citizen>.Filter.Eq(c => c.City, city);
            var stateFilter = Builders<Citizen>.Filter.Eq(c => c.State, state);
            var countryFilter = Builders<Citizen>.Filter.Eq(c => c.Country, country);

            if (!string.IsNullOrEmpty(name)) searchFilter &= nameFilter;
            if (!string.IsNullOrEmpty(postalCode)) searchFilter &= postalCodeFilter;
            if (!string.IsNullOrEmpty(city)) searchFilter &= cityFilter;
            if (!string.IsNullOrEmpty(state)) searchFilter &= stateFilter;
            if (!string.IsNullOrEmpty(country)) searchFilter &= countryFilter;

            return await _context.Citizens.Find(searchFilter).ToListAsync();
        }

        public async Task AddCitizen(Citizen citizen)
        {
            var citizenIdFilter = Builders<Citizen>.Filter.Eq(c => c.CitizenId, citizen.CitizenId);

            var result = await _context.Citizens.Find(citizenIdFilter).FirstOrDefaultAsync();

            if (result == null)
            {
                await _context.Citizens.InsertOneAsync(citizen);
            }
            else
                throw new DuplicateNameException();
        }

        public async Task<bool> RemoveCitizen(string citizenId)
        {
            var citizenIdFilter = Builders<Citizen>.Filter.Eq(c => c.CitizenId, citizenId);

            DeleteResult actionResult = await _context.Citizens.DeleteOneAsync(citizenIdFilter);

            return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
        }

        public async Task<bool> ReplaceCitizen(string citizenId, Citizen citizen)
        {
            var citizenIdFilter = Builders<Citizen>.Filter.Eq(c => c.CitizenId, citizenId);

            ReplaceOneResult actionResult = await _context.Citizens.ReplaceOneAsync(citizenIdFilter, citizen);

            return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
        }
    }
}
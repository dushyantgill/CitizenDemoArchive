﻿using CitizenDemo.CitizenAPI.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenDemo.CitizenAPI.Data
{
    public class CitizenContext
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Citizen> _citizens;

        public CitizenContext(IDatabaseSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);
            _citizens = _database.GetCollection<Citizen>(settings.CitizenCollectionName);
        }

        public IMongoClient Client
        {
            get
            {
                return _client;
            }
        }

        public IMongoDatabase Database
        {
            get
            {
                return _database;
            }
        }

        public IMongoCollection<Citizen> Citizens
        {
            get
            {
                return _citizens;
            }
        }
    }
}

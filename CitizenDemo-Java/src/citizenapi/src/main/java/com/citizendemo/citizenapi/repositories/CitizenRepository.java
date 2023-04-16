package com.citizendemo.citizenapi.repositories;

import com.citizendemo.citizenapi.models.Citizen;
import org.springframework.data.mongodb.repository.MongoRepository;


import java.util.Optional;

// No need implementation, just one interface, and you have CRUD, thanks Spring Data!
public interface CitizenRepository extends MongoRepository<Citizen, String> {
    Optional<Citizen> findByCitizenId(String citizenId);
}
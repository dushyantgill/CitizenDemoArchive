package com.citizendemo.resourceapi.repositories;

import com.citizendemo.resourceapi.models.Resource;
import org.springframework.data.mongodb.repository.MongoRepository;

import java.util.List;
import java.util.Optional;

// No need implementation, just one interface, and you have CRUD, thanks Spring Data!
public interface ResourceRepository extends MongoRepository<Resource, String> {
    Optional<Resource> findByResourceId(String resourceId);

    List<Resource> findAllByCitizenId(String citizenId);

    void deleteAllByCitizenId(String citizenId);
}
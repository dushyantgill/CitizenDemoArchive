package com.citizendemo.citizenapi.controllers;

import com.citizendemo.citizenapi.models.Citizen;
import com.citizendemo.citizenapi.models.Resource;
import com.citizendemo.citizenapi.services.ResourceService;
import com.citizendemo.citizenapi.repositories.CitizenRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.data.mongodb.core.MongoTemplate;
import org.springframework.web.bind.annotation.*;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.mongodb.core.query.Criteria;
import org.springframework.data.mongodb.core.query.Query;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

@RestController
public class CitizenController {
    @Autowired
    private CitizenRepository citizenRepository;
    @Autowired
    private MongoTemplate mongoTemplate;
    @Value("${resourceAPI.url}")
    private String resourceServiceURL;
    private Logger logger = LoggerFactory.getLogger(CitizenController.class);

    @RequestMapping(value = "/citizens", method = RequestMethod.GET)
    public List<Citizen> getAllCitizens() {
        List<Citizen> citizens = new ArrayList<Citizen>();
        Pageable page = PageRequest.of(0, 100);
        try {
            logger.info("getAllCitizens called");

            citizens = citizenRepository.findAll(page).getContent();
        } catch (Exception e) {
            logger.warn("getAllCitizens failed", e);
        }
        logger.info("getAllCitizens returning " + citizens.size() + " citizens");
        return citizens;
    }

    @RequestMapping(value = "/citizens/{citizenId}", method = RequestMethod.GET)
    public Citizen getCitizenByCitizenId(@PathVariable("citizenId") String citizenId) {
        Citizen citizen = null;
        try {
            logger.info("getCitizenByCitizenId called with param " + citizenId);

            Optional<Citizen> result = citizenRepository.findByCitizenId(citizenId);
            if (result.isPresent()) {
                citizen = result.get();
                logger.info("getCitizenByCitizenId returning a citizen");
            }
        } catch (Exception e) {
            logger.warn("getCitizenByCitizenId failed with param" + citizenId, e);
        }
        return citizen;
    }

    @RequestMapping(value = "/citizens/search", method = RequestMethod.GET)
    public List<Citizen> searchCitizens(@RequestParam("name") Optional<String> name,
                                        @RequestParam("postalCode") Optional<String> postalCode,
                                        @RequestParam("city") Optional<String> city,
                                        @RequestParam("state") Optional<String> state,
                                        @RequestParam("country") Optional<String> country) {

        List<Citizen> citizens = new ArrayList<Citizen>();
        try {
            logger.info("searchCitizens called with params " + name + "," + postalCode + "," + city + "," + state + "," + country);

            Query searchQuery = new Query();
            if (name.isPresent() && !name.get().trim().isEmpty()) {
                Criteria nameCriteria = new Criteria();
                nameCriteria.orOperator(Criteria.where("givenName").is(name.get()), Criteria.where("surname").is(name.get()));
                searchQuery.addCriteria(nameCriteria);
            }
            if (postalCode.isPresent() && !postalCode.get().trim().isEmpty())
                searchQuery.addCriteria(Criteria.where("postalCode").is(postalCode.get()));
            if (city.isPresent() && !city.get().trim().isEmpty())
                searchQuery.addCriteria(Criteria.where("city").is(city.get()));
            if (state.isPresent() && !state.get().trim().isEmpty())
                searchQuery.addCriteria(Criteria.where("state").is(state.get()));
            if (country.isPresent() && !country.get().isEmpty())
                searchQuery.addCriteria(Criteria.where("country").is(country.get()));

            citizens = mongoTemplate.find(searchQuery, Citizen.class);
        } catch (Exception e) {
            logger.warn("searchCitizens failed with params " + name + "," + postalCode + "," + city + "," + state + "," + country, e);
        }
        logger.info("searchCitizens returning " + citizens.size() + " citizens");
        return citizens;
    }

    @RequestMapping(value = "/citizens", method = RequestMethod.POST)
    public Citizen createCitizen(@RequestBody Citizen citizen) {
        Citizen createdCitizen = null;
        try {
            logger.info("createCitizen called with param " + citizen.toString());
            Optional<Citizen> existingCitizen = citizenRepository.findByCitizenId(citizen.citizenId);
            if (existingCitizen.isPresent()) {
                logger.info("createCitizen not creating, found existing citizen with citizenId " + citizen.citizenId);
                return null;
            }
            createdCitizen = citizenRepository.save(citizen);
            logger.info("createCitizen created citizen with internalId" + createdCitizen.internalId);
            logger.info("createCitizen calling resourceAPI to provision default resources for citizen with citizenId " + createdCitizen.citizenId);
            ResourceService.ProvisionDefaultResource(citizen, resourceServiceURL);
        } catch (Exception e) {
            logger.warn("createCitizen failed with param " + citizen.toString(), e);
        }
        return createdCitizen;
    }

    @RequestMapping(value = "/citizens/{citizenId}", method = RequestMethod.PUT)
    public Citizen updateCitizen(@PathVariable("citizenId") String citizenId, @RequestBody Citizen citizenUpdates) {
        Citizen updatedCitizen = null;
        try {
            logger.info("updateCitizen called with param " + citizenUpdates.toString());
            Optional<Citizen> oldCitizen = citizenRepository.findByCitizenId(citizenId);
            if (oldCitizen.isPresent()) {
                Citizen newCitizen = oldCitizen.get();
                if (citizenUpdates.givenName != null && !citizenUpdates.givenName.trim().isEmpty())
                    newCitizen.givenName = citizenUpdates.givenName;
                if (citizenUpdates.surname != null && !citizenUpdates.surname.trim().isEmpty())
                    newCitizen.surname = citizenUpdates.surname;
                if (citizenUpdates.phoneNumber != null && !citizenUpdates.phoneNumber.trim().isEmpty())
                    newCitizen.phoneNumber = citizenUpdates.phoneNumber;
                if (citizenUpdates.streetAddress != null && !citizenUpdates.streetAddress.trim().isEmpty())
                    newCitizen.streetAddress = citizenUpdates.streetAddress;
                if (citizenUpdates.city != null && !citizenUpdates.city.trim().isEmpty())
                    newCitizen.city = citizenUpdates.city;
                if (citizenUpdates.state != null && !citizenUpdates.state.trim().isEmpty())
                    newCitizen.state = citizenUpdates.state;
                if (citizenUpdates.postalCode != null && !citizenUpdates.postalCode.trim().isEmpty())
                    newCitizen.postalCode = citizenUpdates.postalCode;
                if (citizenUpdates.country != null && !citizenUpdates.country.trim().isEmpty())
                    newCitizen.country = citizenUpdates.country;

                updatedCitizen = citizenRepository.save(newCitizen);
                logger.debug("updateCitizen returning updated citizen");
            } else {
                logger.debug("updateCitizen did not find citizen with citizenId " + citizenId);
            }
        } catch (Exception e) {
            logger.warn("updateCitizen failed with param " + citizenUpdates.toString(), e);
        }
        return updatedCitizen;
    }

    @RequestMapping(value = "/citizens/{citizenId}", method = RequestMethod.DELETE)
    public void deleteCitizen(@PathVariable("citizenId") String citizenId) {
        try {
            logger.info("deleteCitizen called with param " + citizenId);
            Optional<Citizen> citizen = citizenRepository.findByCitizenId(citizenId);
            if (citizen.isPresent()) {
                citizenRepository.delete(citizen.get());
                logger.info("deleteCitizen deleted citizen with citizenId" + citizenId);

                logger.info("deleteCitizen calling resourceAPI to deprovision all resources for citizen with citizenId " + citizenId);
                ResourceService.DeprovisionAllResources(citizen.get(), resourceServiceURL);
            } else {
                logger.info("deleteCitizen did not find citizen with citizenId " + citizenId);
            }
        } catch (Exception e) {
            logger.warn("deleteCitizen failed with param " + citizenId);
        }
    }
}

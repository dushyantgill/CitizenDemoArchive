package com.citizendemo.citizenapi.controllers;

import com.citizendemo.citizenapi.models.Citizen;
import com.citizendemo.citizenapi.services.ResourceService;
import com.citizendemo.citizenapi.repositories.CitizenRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.Optional;

@RestController
public class CitizenController {
    @Autowired
    private CitizenRepository citizenRepository;
    @Value("${resourceAPI.url}")
    private String resourceServiceURL;
    @RequestMapping(value = "/citizens", method = RequestMethod.GET)
    public List<Citizen> getAllCitizens() {
        return citizenRepository.findAll();
    }

    @RequestMapping(value = "/citizens/{citizenId}", method = RequestMethod.GET)
    public Optional<Citizen> getCitizenByCitizenId(@PathVariable("citizenId") String citizenId) {
        return citizenRepository.findByCitizenId(citizenId);
    }

    @RequestMapping(value = "/citizens", method = RequestMethod.POST)
    public Citizen addNewCitizen(@RequestBody Citizen citizen){
        Optional<Citizen> existingCitizen = citizenRepository.findByCitizenId(citizen.citizenId);
        if(existingCitizen.isPresent()) return null;

        Citizen createdCitizen = citizenRepository.save(citizen);
        ResourceService.ProvisionDefaultResource(citizen, resourceServiceURL);

        return createdCitizen;
    }

    @RequestMapping(value = "/citizens/{citizenId}", method = RequestMethod.PUT)
    public Citizen updateCitizen(@PathVariable("citizenId") String citizenId, @RequestBody Citizen citizenUpdates){
        Optional<Citizen> oldCitizen = citizenRepository.findByCitizenId(citizenId);
        if(oldCitizen.isPresent()) {
            Citizen newCitizen = oldCitizen.get();
            if (citizenUpdates.givenName != null && !citizenUpdates.givenName.trim().isEmpty()) newCitizen.givenName = citizenUpdates.givenName;
            if (citizenUpdates.surname != null && !citizenUpdates.surname.trim().isEmpty()) newCitizen.surname = citizenUpdates.surname;
            if (citizenUpdates.phoneNumber != null && !citizenUpdates.phoneNumber.trim().isEmpty()) newCitizen.phoneNumber = citizenUpdates.phoneNumber;
            if (citizenUpdates.streetAddress != null && !citizenUpdates.streetAddress.trim().isEmpty()) newCitizen.streetAddress = citizenUpdates.streetAddress;
            if (citizenUpdates.city != null && !citizenUpdates.city.trim().isEmpty()) newCitizen.city = citizenUpdates.city;
            if (citizenUpdates.state != null && !citizenUpdates.state.trim().isEmpty()) newCitizen.state = citizenUpdates.state;
            if (citizenUpdates.postalCode != null && !citizenUpdates.postalCode.trim().isEmpty()) newCitizen.postalCode = citizenUpdates.postalCode;
            if (citizenUpdates.country != null && !citizenUpdates.country.trim().isEmpty()) newCitizen.country = citizenUpdates.country;
            return citizenRepository.save(newCitizen);
        }
        return null;
    }

    @RequestMapping(value = "/citizens/{citizenId}", method = RequestMethod.DELETE)
    public void deleteCitizen(@PathVariable("citizenId") String citizenId) {
        Optional<Citizen> citizen = citizenRepository.findByCitizenId(citizenId);
        if(citizen.isPresent()) citizenRepository.delete(citizen.get());
        ResourceService.DeprovisionAllResources(citizen.get(), resourceServiceURL);
    }

}

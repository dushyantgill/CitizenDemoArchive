package com.citizendemo.citizenapi.models;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;

@Document(collection = "citizens")
public class Citizen {
    @Id
    public String internalId;
    public String citizenId;
    public String givenName;
    public String surname;
    public String phoneNumber;
    public String streetAddress;
    public String city;
    public String state;
    public String postalCode;
    public String country;

    public Citizen() {
    }

    public Citizen(String citizenId, String givenName, String surname, String phoneNumber, String streetAddress, String city, String state, String postalCode, String country) {
        this.citizenId = citizenId;
        this.givenName = givenName;
        this.surname = surname;
        this.phoneNumber = phoneNumber;
        this.streetAddress = streetAddress;
        this.city = city;
        this.state = state;
        this.postalCode = postalCode;
        this.country = country;
    }

    @Override
    public String toString() {
        return "Citizen{" +
                "citizenId='" + citizenId + '\'' +
                "givenName='" + givenName + '\'' +
                "surname='" + surname + '\'' +
                "phoneNumber='" + phoneNumber + '\'' +
                "streetAddress='" + streetAddress + '\'' +
                "state='" + state + '\'' +
                "postalCode='" + postalCode + '\'' +
                "country='" + country + '\'' +
                '}';
    }
}
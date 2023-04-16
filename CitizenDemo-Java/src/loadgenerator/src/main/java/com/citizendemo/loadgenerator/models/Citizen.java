package com.citizendemo.loadgenerator.models;

public class Citizen {
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

    public String toJSONString() {
        return "{" +
                "\"citizenId\":\"" + this.citizenId + "\"," +
                "\"givenName\":\"" + this.givenName + "\"," +
                "\"surname\":\"" + this.surname + "\"," +
                "\"phoneNumber\":\"" + this.phoneNumber + "\"," +
                "\"streetAddress\":\"" + this.streetAddress + "\"," +
                "\"city\":\"" + this.city + "\"," +
                "\"state\":\"" + this.state + "\"," +
                "\"postalCode\":\"" + this.postalCode + "\"," +
                "\"country\":\"" + this.country + "\"" +
                "}";
    }
}
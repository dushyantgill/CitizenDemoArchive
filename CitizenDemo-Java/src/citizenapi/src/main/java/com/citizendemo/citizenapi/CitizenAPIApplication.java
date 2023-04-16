package com.citizendemo.citizenapi;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

@SpringBootApplication
public class CitizenAPIApplication {

    public static void main(String[] args) {
        Logger logger = LoggerFactory.getLogger(CitizenAPIApplication.class);
        logger.info("CitizenAPI starting ...");
        SpringApplication.run(CitizenAPIApplication.class, args);
    }

}

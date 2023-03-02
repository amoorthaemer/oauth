package com.example.WeatherForecast;

import java.time.LocalDate;

import com.fasterxml.jackson.annotation.JsonIgnore;

public class WeatherForecast {
    public LocalDate date;
    public int temperatureC;
    public String summary;

    @JsonIgnore
    public int getTemperatureF() {
        return 32 + (int)(temperatureC / 0.5556);
    }
}

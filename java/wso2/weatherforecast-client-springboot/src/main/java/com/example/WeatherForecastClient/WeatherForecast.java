package com.example.WeatherForecastClient;

import java.time.LocalDate;

public class WeatherForecast {
    public LocalDate date;
    public int temperatureC;
    public String summary;

    public int getTemperatureF() {
        return 32 + (int)(temperatureC / 0.5556);
    }
}

package com.example.WeatherForecast;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.security.oauth2.resource.OAuth2ResourceServerProperties;
import org.springframework.context.annotation.Bean;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.config.annotation.method.configuration.EnableGlobalMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.WebSecurityConfigurerAdapter;
import org.springframework.security.oauth2.jwt.JwtDecoder;
import org.springframework.security.oauth2.jwt.NimbusJwtDecoder;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

import com.nimbusds.jose.JOSEObjectType;
import com.nimbusds.jose.proc.DefaultJOSEObjectTypeVerifier;

import java.security.Principal;
import java.time.LocalDate;
import java.util.ArrayList;
import java.util.Random;

@SpringBootApplication
public class Application {

	public static void main(String[] args) {
		SpringApplication.run(Application.class, args);
	}

	@EnableGlobalMethodSecurity(prePostEnabled = true)
    public static class SecurityConfig extends WebSecurityConfigurerAdapter {
        protected void configure(final HttpSecurity http) throws Exception {
            http.authorizeRequests()
                .anyRequest().authenticated()
                .and()
                .oauth2ResourceServer(oauth2 -> {
                    oauth2.jwt();
                });
        }

		@Bean
		public JwtDecoder jwtDecoder(OAuth2ResourceServerProperties properties) {
            String jwkSetUri = properties.getJwt().getJwkSetUri();

			NimbusJwtDecoder jwtDecoder = NimbusJwtDecoder
                .withJwkSetUri(jwkSetUri)
                .jwtProcessorCustomizer(customizer -> {
                    customizer.setJWSTypeVerifier(new DefaultJOSEObjectTypeVerifier<>(
                        new JOSEObjectType("at+jwt")
                        //new JOSEObjectType("jwt")
                    ));
                })
                .build();
			
			return jwtDecoder;
		}

    }

	@RestController
    public class RequestController {
        static final String[] summaries = new String[] {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        @GetMapping("/")
        public String getMessage(Principal principal) {
            return "Hello " + principal.getName();
        }

        @PreAuthorize("hasAuthority('SCOPE_weatherapp')")
        //@PreAuthorize("isAuthenticated()")
        @GetMapping("/forecasts")
        public WeatherForecast[] getForecasts(Principal principal) {
            Random rnd = new Random();
            LocalDate now = LocalDate.now();
            ArrayList<WeatherForecast> forecasts = new ArrayList<WeatherForecast>();
            
            for (int i = 0; i < 5; i++) {
                WeatherForecast forecast = new WeatherForecast();

                forecast.date = now.plusDays(i);
                forecast.temperatureC = rnd.nextInt(-22, 55);
                forecast.summary = summaries[rnd.nextInt(summaries.length)];

                forecasts.add(forecast);
            }

            return forecasts.toArray(new WeatherForecast[0]);
        }
    }
}

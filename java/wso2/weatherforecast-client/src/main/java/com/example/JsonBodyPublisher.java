package com.example;

import java.net.http.HttpRequest.BodyPublisher;
import java.nio.ByteBuffer;
import java.util.concurrent.Flow.Subscriber;

import com.fasterxml.jackson.databind.ObjectMapper;

public class JsonBodyPublisher<T> implements BodyPublisher {

    private Class<T> wClass;
    private ObjectMapper mapper;

    public JsonBodyPublisher(Class<T> wClass) {
        this(wClass, new ObjectMapper());
    }

    public JsonBodyPublisher(Class<T> wClass, ObjectMapper mapper) {
        super();
        this.wClass = wClass;
        this.mapper = mapper;
    }

    @Override
    public void subscribe(Subscriber<? super ByteBuffer> subscriber) {
        // TODO Auto-generated method stub
        
    }

    @Override
    public long contentLength() {
        // TODO Auto-generated method stub
        return 0;
    }
    
}

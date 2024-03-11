# Pixel

## Description

This solution contains 2 major entry points:
* Pixel. HTTP - A minimal API that receives requests at GET /track, collects basic request metadata, and publishes a message to a message broker, in this case, RabbitMQ.
* Pixel.Storage.Service - A daemon that listens to the message broker and processes the RecordVisitCommand, storing the metadata in a text file.

Both projects are containerized, and the entire solution can be run using `docker-compose up`.
 
## Considerations

Due to the time constraints, I kept the overall solution as simple as possible and focused on the following aspects:
* Simplicity
* Testability
* Scalability
* Maintainability

The `Pixel.Http` project is as simple as it gets, a Minimal API that just receives requests and publishes them directly to the message broker using MassTransit.
The `Pixel.Storage.Service` is also very simple. It was designed with a simplified hexagonal architecture, to improve it's testabilitity and lay the ground for future expansion.

The requirements defined that only one instance of `Pixel.Storage.Service` should be running at a time, so the concurrency control for the output file was simplified to a simple semaphore.

If the requirements were different, and we needed to scale the `Pixel.Storage.Service` horizontally, we would need to implement a more robust concurrency control mechanism, like a distributed lock.

The `Pixel.Http` project is also very simple to scale, and due to the output being a message broker, it's also very scalable, as we can just add more instances of the `Pixel.Http` project and they will all publish messages to the same message broker.

## Running the solution

The entire solution can be run using `docker-compose up`, and it will start the following services:
* RabbitMQ
* Pixel.Http
* Pixel.Storage.Service

## Possible Future improvements
* Add a distributed lock to the `Pixel.Storage.Service` to allow for horizontal scaling.
* Opentelemetry for distributed tracing and observability
* Integration tests
* CI/CD pipeline
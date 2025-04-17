
# Simple Personal Finance - Test and Learning Project

This project is a simple personal finance application created for testing and knowledge implementation purposes. It serves as a playground to explore different technologies, architectural patterns, and development practices within the .NET ecosystem and Docker environment.

**Important Notes:**

*   **For Testing Only:** This application is not intended for production use.
*   **Potential Over-Engineering:** Some parts of the application may be over-engineered as a result of experimentation and learning.
*   **Under Constant Development:** This project is continuously being improved and refined. Expect changes and updates as new concepts are explored and implemented.


# Running Docker Compose Configurations

This project provides two Docker Compose configurations: one for a full, Dockerized environment and another for local development.

## Full Dockerized Environment

This environment includes all services necessary to run the application, including the database, Adminer, Seq, Nginx, and the API.

### Prerequisites

*   Docker
*   Docker Compose

### Running the Full Environment

1.  Navigate to the `infra` directory:

    ```bash
    cd infra
    ```

2.  Start the services using Docker Compose:

    ```bash
    docker-compose -f docker-compose.yml up -d
    ```

    This command will start all services defined in `docker-compose.yml` in detached mode.

## Local Development Environment

This environment is designed for local development, allowing you to run the API from your IDE (e.g., Visual Studio) while using Docker for the database, Adminer, and Seq.

### Prerequisites

*   Docker
*   Docker Compose
*   .NET SDK (for running the API locally)

### Running the Local Development Environment

1.  Navigate to the `infra` directory:

    ```bash
    cd infra
    ```

2.  Start the database, Adminer and Seq services:

    ```bash
    docker-compose -f docker-compose.dev.yml up -d
    ```

    This command will start the services defined in `docker-compose.dev.yml` in detached mode.

3.  Configure your API project to connect to the Dockerized database.  This typically involves updating the connection string in your `appsettings.Development.json` file.

4.  Run the API project from your IDE.

## Stopping the Environments

To stop the services, navigate to the `infra` directory and run:

```bash
docker-compose -f <compose-file-name>.yml down
```
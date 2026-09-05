# Simple Personal Finance - Test and Learning Project

This project is a simple personal finance application created for testing and knowledge implementation purposes. It serves as a playground to explore different technologies, architectural patterns, and development practices within the .NET ecosystem and Docker environment.

**Important Notes:**

- **For Testing Only:** This application is not intended for production use.
- **Potential Over-Engineering:** Some parts of the application may be over-engineered as a result of experimentation and learning.
- **Under Constant Development:** This project is continuously being improved and refined.

## Local secrets

Secrets and local credentials are not stored in the repository.

- Docker Compose reads local values from `infra/.env`.
- The ASP.NET Core project is configured for `dotnet user-secrets` during local development.
- Environment variables can override both mechanisms when needed.

Never reuse credentials that appeared in repository history. Generate new local values before running the application.

## Full Dockerized Environment

This environment includes PostgreSQL, Adminer, Seq, Nginx, and the API.

### Prerequisites

- Docker
- Docker Compose

### Configure local secrets

1. Navigate to `infra`:

   ```bash
   cd infra
   ```

2. Create your local environment file from the safe template:

   ```bash
   cp .env.example .env
   ```

3. Edit `.env` and provide values for:

   - `POSTGRES_PASSWORD`
   - `JWT_KEY`

   `POSTGRES_USER` and `POSTGRES_DB` have development defaults in the template and can be changed locally if desired. Use a cryptographically random value for `JWT_KEY`.

### Run

```bash
docker-compose -f docker-compose.yml up -d
```

Docker Compose injects the database connection string and JWT signing key into the API container through environment variables.

## Local Development Environment

This mode runs PostgreSQL, Adminer, and Seq in Docker while the API runs from your IDE or `dotnet run`.

### Prerequisites

- Docker
- Docker Compose
- .NET SDK

### Configure the database container

From `infra`:

```bash
cp .env.example .env
```

Set `POSTGRES_PASSWORD` in `infra/.env`, then start the supporting services:

```bash
docker-compose -f docker-compose.dev.yml up -d
```

### Configure the API with user secrets

From the repository root, replace the placeholder values below with your local values:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Port=5432;Database=spfdb;User Id=spfuser;Password=<local-password>;" --project src/SimplePersonalFinance.API
dotnet user-secrets set "Jwt:Key" "<cryptographically-random-local-key>" --project src/SimplePersonalFinance.API
```

The database password in the connection string must match the password configured in `infra/.env`.

Run the API from your IDE or with:

```bash
dotnet run --project src/SimplePersonalFinance.API
```

## Stopping the Environments

From `infra`:

```bash
docker-compose -f <compose-file-name>.yml down
```

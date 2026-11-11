# IP Forensics Report

Full-stack app for generating and reviewing IP forensics reports. Combines geolocation data from `ip-api.com` with threat reputation metrics from `AbuseIPDB`, storing the merged reports encrypted at rest (AES-256) in MySQL per user.

* **Backend:** .NET 10 Web API, ADO.NET (parameterised SQL, no ORM)
* **Frontend:** Angular 22 (standalone components, signals, Angular Material)
* **Database:** MySQL 8.4 (Docker)

## Prerequisites

* .NET 10 SDK
* Node.js 22+
* Docker & Docker Compose
* An [AbuseIPDB](https://www.abuseipdb.com) API key

## Local Setup

### 1. Database

Copy the environment template, set `MYSQL_ROOT_PASSWORD` and `MYSQL_PASSWORD` in `.env`, and start MySQL:

```bash
cp .env.example .env
docker compose up -d
```

`db/schema.sql` initializes the schema automatically on first boot. If you need to reset the database later, run `docker compose down -v && docker compose up -d`.

### 2. API Secrets

The API fails fast on startup if required configuration keys are missing or invalid. Configure them via .NET User Secrets (replace `<MYSQL_PASSWORD>` with the value from `.env`):

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Port=3306;Database=ip_forensics;User=ipf_app;Password=<MYSQL_PASSWORD>;GuidFormat=Binary16;" --project server/src/IPForensics.Api
dotnet user-secrets set "Jwt:Key" "\$(openssl rand -base64 32)" --project server/src/IPForensics.Api
dotnet user-secrets set "Encryption:Key" "\$(openssl rand -base64 32)" --project server/src/IPForensics.Api
dotnet user-secrets set "AbuseIpDb:ApiKey" "<your_abuseipdb_key>" --project server/src/IPForensics.Api
```


### 3. Run the Backend

```bash
dotnet run --project server/src/IPForensics.Api
```

The API listens on `http://localhost:5272`.

### 4. Run the Frontend

```bash
cd client
npm install
ng serve
```

Open `http://localhost:4200`. API calls to `/api` are proxied to `localhost:5272` via `client/proxy.conf.json`.

## Notes & Design Choices

* **Data Access:** Used `MySqlConnector` with raw parameterised SQL (rather than EF Core to keep dependencies minimal)
* **Encryption at Rest:** Report payloads are serialized and encrypted with AES-256 before persistence so sensitive lookup history isn't stored in plaintext.
* **Input Filtering:** Loopback, private, and reserved CIDR ranges (IPv4 and IPv6) are rejected prior to calling external providers to avoid wasting API quota.
* **Password Hashing:** Used .NET's built-in `PasswordHasher` (PBKDF2-HMAC-SHA512, 220k iterations) to rely on a maintained, FIPS-approved first-party implementation rather than third-party Argon2 packages, despite OWASP's preference for Argon2id.

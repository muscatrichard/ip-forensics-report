CREATE TABLE IF NOT EXISTS users (
                                     id            BINARY(16)   NOT NULL,
    first_name    VARCHAR(100) NOT NULL,
    last_name     VARCHAR(100) NOT NULL,
    email         VARCHAR(254) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at    DATETIME(6)  NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
    PRIMARY KEY (id),
    UNIQUE KEY uq_users_email (email)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE IF NOT EXISTS ip_forensics_reports (
                                                    id          BINARY(16)      NOT NULL,
    user_id     BINARY(16)      NOT NULL,
    payload     VARBINARY(8192) NOT NULL,
    created_at  DATETIME(6)     NOT NULL DEFAULT (UTC_TIMESTAMP(6)),
    PRIMARY KEY (id),
    KEY ix_reports_user_created (user_id, created_at),
    CONSTRAINT fk_reports_user FOREIGN KEY (user_id)
    REFERENCES users (id) ON DELETE CASCADE
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
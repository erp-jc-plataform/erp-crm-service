graph LR
  subgraph Frontend/Client
    FE[Frontend / Client]
  end

  subgraph Gateway
    GW[Business-Gateway:4000\nNode.js/Express]
  end

  subgraph Security
    SEC[Business-Security:8000\nFastAPI + PostgreSQL (Auth)]
  end

  subgraph Licensing
    LIC[Business-Licensing:3001\nNext.js + Prisma + PostgreSQL]
  end

  subgraph Mensajeria
    MSG_API[Business-Mensajeria:5000\nExpress API]
    MSG_WORKER[Mensajeria Workers\nBullMQ]
    REDIS_M[Redis:6379]
    SMTP[SMTP Provider\n(Gmail/SendGrid/AWS SES)]
    TPL[Templates (Handlebars)]
  end

  subgraph Report
    REP_API[Business-Report:3008\nNode API]
    REP_WORKER[Report Worker\nBullMQ]
    REP_ML[ML Worker\nBullMQ + OpenAI]
    MONGO[mongodb:27017]
    REDIS_R[redis:6379]
    CH[ClickHouse:8123]
    TSDB[TimescaleDB:5432]
    KAFKA[kafka:9092]
    ZK[zookeeper:2181]
  end

  %% Client to Gateway
  FE -->|HTTP| GW

  %% Gateway routes
  GW -->|/api/auth| SEC
  GW -->|/api/licencias| LIC
  GW -->|/api/email/*| MSG_API
  GW -->|/api/empleados| EMP[Business-Employees (futuro)]
  GW -->|/api/clientes| CLI[Business-Clients (futuro)]
  GW -->|/api/ventas| VEN[Business-Ventas (futuro)]

  %% Security
  SEC -->|JWT validate /me| GW

  %% Licensing validation
  GW -->|validate module| LIC

  %% Mensajeria flows
  MSG_API --> REDIS_M
  MSG_WORKER --> REDIS_M
  MSG_WORKER --> SMTP
  MSG_WORKER --> TPL

  %% Report deps
  REP_API --> MONGO
  REP_API --> REDIS_R
  REP_API --> CH
  REP_API --> TSDB
  REP_API --> KAFKA
  KAFKA --> ZK
  REP_WORKER --> REDIS_R
  REP_WORKER --> MONGO
  REP_ML --> REDIS_R
  REP_ML --> MONGO

  %% Report -> Mensajeria integration (from docker-compose)
  REP_API -. MENSAJERIA_SERVICE_URL .-> MSG_API

  classDef svc fill:#e8f1ff,stroke:#4a90e2,color:#111
  class SEC,LIC,MSG_API,MSG_WORKER,REP_API,REP_WORKER,REP_ML,GW svc
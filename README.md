# BusinessApp ERP

Plataforma ERP empresarial con arquitectura de microservicios.

---

## Estructura del proyecto

```
BusinessApp/
├── Business-Gateway/        # API Gateway — Node.js/Express (puerto 4000)
├── Business-Security/       # Autenticación — FastAPI/Python (puerto 8000)
├── Business-Licensing/      # Licenciamiento — Next.js (puerto 3001)
├── Business-CRM/            # Clientes — .NET Framework 4.8 (puerto 8003)
├── Business-Log/            # Logs — Node.js (puerto 3005)
├── Business-Mensajeria/     # Mensajería/WebSocket — Node.js (puerto 3006)
├── Business-Notificaciones/ # Notificaciones push — Node.js (puerto 3007)
├── Business-Report/         # Reportes/Analytics — Node.js (puerto 3008)
├── Business-FrontEnd/       # Frontend web — Nx/React
├── Business-Mobile/         # App móvil — Flutter
├── docker-compose.yml       # Infraestructura completa con Docker
└── .env                     # Variables de entorno (no commitear)
```

---

## Requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) 24+
- [Docker Compose](https://docs.docker.com/compose/) v2
- Node.js 20+ (desarrollo local sin Docker)
- Python 3.11+ (desarrollo local sin Docker)
- Flutter 3.x (app móvil)

---

## Levantar con Docker

### 1. Configurar variables de entorno

```bash
cp .env.example .env
```

Editar `.env` con los valores reales (JWT_SECRET, credenciales SMTP, etc.).

### 2. Levantar toda la infraestructura

```bash
docker-compose up -d
```

Esto levanta: PostgreSQL, MongoDB, Redis, Kafka, Elasticsearch, ClickHouse, TimescaleDB y todos los microservicios.

### 3. Levantar solo los servicios esenciales

Para desarrollo rápido (sin Kafka ni analytics):

```bash
docker-compose up -d postgres redis erp-security erp-gateway
```

### 4. Ver logs en tiempo real

```bash
# Todos los servicios
docker-compose logs -f

# Un servicio específico
docker-compose logs -f erp-gateway
docker-compose logs -f erp-security
```

### 5. Detener todo

```bash
docker-compose down
```

Para detener **y borrar volúmenes** (bases de datos):

```bash
docker-compose down -v
```

---

## Desarrollo local (sin Docker)

### Gateway

```bash
cd Business-Gateway
npm install
npm run dev       # http://localhost:4000
```

### Security

```bash
cd Business-Security
python -m venv .venv
.venv\Scripts\activate       # Windows
pip install -r requirements.txt
uvicorn app.main:app --reload --host 0.0.0.0 --port 8000
```

### CRM (.NET Framework 4.8)

> Este servicio **no puede dockerizarse en Linux** por requerir .NET Framework 4.8.
> Correr localmente con Visual Studio o .NET CLI en Windows.

```bash
cd Business-CRM/erp-crm-service
dotnet run        # http://localhost:8003
```

---

## App móvil (Flutter)

### Requisitos previos
- Android Studio + SDK
- Dispositivo físico o emulador conectado

### Correr en dispositivo físico

```bash
cd Business-Mobile/erpmobile

# Ver dispositivos disponibles
flutter devices

# Correr en dispositivo específico
flutter run -d <DEVICE_ID>
```

### Cambiar URL del backend

Editar `lib/core/utils/constants.dart`:

```dart
// Emulador Android
static const String baseUrl = 'http://10.0.2.2:4000';

// Dispositivo físico (reemplazar con la IP de tu PC en la red Wi-Fi)
static const String baseUrl = 'http://192.168.x.x:4000';

// Con Docker en la misma máquina (usando adb reverse)
static const String baseUrl = 'http://localhost:4000';
```

**Tip:** Para evitar configurar el firewall, usar `adb reverse` via USB:

```bash
adb reverse tcp:4000 tcp:4000
adb reverse tcp:8000 tcp:8000
# Luego usar http://localhost:4000 en constants.dart
```

---

## Healthcheck del Gateway

```bash
curl http://localhost:4000/health
```

Respuesta esperada:

```json
{
  "status": "ok",
  "services": [
    { "name": "Auth Service", "status": "online" },
    ...
  ]
}
```

---

## Infraestructura Docker — puertos

| Servicio | Puerto | Descripción |
|---|---|---|
| erp-gateway | 4000 | API Gateway principal |
| erp-security | 8000 | Autenticación JWT |
| erp-licensing | 3001 | Gestión de licencias |
| erp-mensajeria | 3006 | Email/SMS/WhatsApp |
| erp-notificaciones | 3007 | Push notifications |
| erp-report | 3008 | Reportes y analytics |
| erp-log | 3005 | Centralización de logs |
| postgres | 5432 | Base de datos principal |
| mongodb | 27017 | Base de datos NoSQL |
| redis | 6379 | Cache y sesiones |
| kafka | 9092 | Message broker |
| elasticsearch | 9200 | Motor de búsqueda |
| clickhouse | 8123 | Analytics (HTTP) |
| timescaledb | 5433 | Series de tiempo |

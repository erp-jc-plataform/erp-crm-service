# ?? Guía de Pruebas con Postman - ERP CRM API

## ?? Configuración Inicial

### **Base URL**
```
https://localhost:44300
```

### **Headers Comunes**
```
Content-Type: application/json
Accept: application/json
```

---

## ?? **CUSTOMERS - Ejemplos de Requests**

### 1?? **Crear Cliente** (POST)

```http
POST https://localhost:44300/api/customers
Content-Type: application/json

{
  "companyName": "Distribuciones López SL",
  "contactFirstName": "Ana",
  "contactLastName": "López",
  "email": "ana.lopez@distribuciones.com",
  "phone": "+34655444333",
  "address": {
    "street": "Polígono Industrial Norte, Nave 12",
    "city": "Valencia",
    "state": "Valencia",
    "postalCode": "46001",
    "country": "España"
  },
  "taxId": "B76543210",
  "type": 2,
  "creditLimit": 100000.00
}
```

**Response (201 Created)**:
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "companyName": "Distribuciones López SL",
  "contactFirstName": "Ana",
  "contactLastName": "López",
  "email": {
    "value": "ana.lopez@distribuciones.com"
  },
  "status": 1,
  "type": 2,
  "creditLimit": 100000.00,
  "currentBalance": 0.00,
  "isActive": true,
  "createdAt": "2024-01-15T10:30:00Z"
}
```

---

### 2?? **Actualizar Cliente** (PUT)

```http
PUT https://localhost:44300/api/customers/a1b2c3d4-e5f6-7890-abcd-ef1234567890
Content-Type: application/json

{
  "companyName": "Distribuciones López y Asociados SL",
  "contactFirstName": "Ana María",
  "contactLastName": "López García",
  "email": "anamaria.lopez@distribuciones.com",
  "phone": "+34655444334",
  "address": {
    "street": "Polígono Industrial Norte, Nave 12-14",
    "city": "Valencia",
    "state": "Valencia",
    "postalCode": "46001",
    "country": "España"
  },
  "creditLimit": 150000.00,
  "notes": "Cliente preferente - Descuento 10%"
}
```

**Response (200 OK)**: Cliente actualizado completo

---

### 3?? **Suspender Cliente** (PATCH Status)

```http
PATCH https://localhost:44300/api/customers/a1b2c3d4-e5f6-7890-abcd-ef1234567890/status
Content-Type: application/json

{
  "status": 3,
  "reason": "Facturas impagadas: INV-2024-001, INV-2024-003"
}
```

**Response (200 OK)**:
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "status": 3,
  "notes": "Suspended: Facturas impagadas: INV-2024-001, INV-2024-003"
}
```

---

### 4?? **Ajustar Balance del Cliente** (PATCH Balance)

**Agregar balance (cobro)**:
```http
PATCH https://localhost:44300/api/customers/a1b2c3d4-e5f6-7890-abcd-ef1234567890/balance
Content-Type: application/json

{
  "amount": 5000.00,
  "isDebit": false
}
```

**Deducir balance (cargo)**:
```http
PATCH https://localhost:44300/api/customers/a1b2c3d4-e5f6-7890-abcd-ef1234567890/balance
Content-Type: application/json

{
  "amount": 2500.00,
  "isDebit": true
}
```

**Response (200 OK)**:
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "currentBalance": 2500.00
}
```

---

### 5?? **Eliminar Cliente** (DELETE - Soft)

```http
DELETE https://localhost:44300/api/customers/a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

**Response (204 No Content)**: Sin cuerpo

---

### 6?? **Restaurar Cliente** (POST Restore)

```http
POST https://localhost:44300/api/customers/a1b2c3d4-e5f6-7890-abcd-ef1234567890/restore
```

**Response (200 OK)**:
```json
{
  "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "isActive": true
}
```

---

## ?? **PRODUCTS - Ejemplos de Requests**

### 1?? **Crear Producto** (POST)

```http
POST https://localhost:44300/api/products
Content-Type: application/json

{
  "sku": "MON-004",
  "name": "Monitor Samsung 32\" 4K UHD",
  "description": "Monitor profesional 32 pulgadas, resolución 4K, panel IPS",
  "category": 1,
  "unitPrice": 549.99,
  "cost": 380.00,
  "stockQuantity": 30,
  "minimumStock": 8,
  "unit": "Unidad",
  "supplier": "Samsung Electronics"
}
```

**Response (201 Created)**:
```json
{
  "id": "f1e2d3c4-b5a6-7890-cdef-1234567890ab",
  "sku": "MON-004",
  "name": "Monitor Samsung 32\" 4K UHD",
  "unitPrice": 549.99,
  "cost": 380.00,
  "stockQuantity": 30,
  "minimumStock": 8,
  "maximumStock": 80,
  "isActive": true,
  "isDiscontinued": false,
  "createdAt": "2024-01-15T11:00:00Z"
}
```

---

### 2?? **Actualizar Producto** (PUT)

```http
PUT https://localhost:44300/api/products/f1e2d3c4-b5a6-7890-cdef-1234567890ab
Content-Type: application/json

{
  "name": "Monitor Samsung 32\" 4K UHD Curvo",
  "description": "Monitor profesional 32 pulgadas curvo, resolución 4K, panel IPS, HDR10",
  "category": 1,
  "unitPrice": 599.99,
  "cost": 400.00,
  "minimumStock": 10,
  "maximumStock": 100,
  "supplier": "Samsung Electronics Spain"
}
```

**Response (200 OK)**: Producto actualizado

---

### 3?? **Ajustar Stock** (PATCH Stock)

**Agregar stock (entrada de almacén)**:
```http
PATCH https://localhost:44300/api/products/f1e2d3c4-b5a6-7890-cdef-1234567890ab/stock
Content-Type: application/json

{
  "quantity": 50,
  "reason": "Recepción de mercancía - Albarán ALB-2024-045"
}
```

**Reducir stock (salida de almacén)**:
```http
PATCH https://localhost:44300/api/products/f1e2d3c4-b5a6-7890-cdef-1234567890ab/stock
Content-Type: application/json

{
  "quantity": -10,
  "reason": "Venta cliente ABC - Pedido PED-2024-120"
}
```

**Response (200 OK)**:
```json
{
  "id": "f1e2d3c4-b5a6-7890-cdef-1234567890ab",
  "stockQuantity": 70
}
```

---

### 4?? **Actualizar Precios** (PATCH Pricing)

```http
PATCH https://localhost:44300/api/products/f1e2d3c4-b5a6-7890-cdef-1234567890ab/pricing
Content-Type: application/json

{
  "unitPrice": 649.99,
  "cost": 420.00
}
```

**Response (200 OK)**:
```json
{
  "id": "f1e2d3c4-b5a6-7890-cdef-1234567890ab",
  "unitPrice": 649.99,
  "cost": 420.00
}
```

---

### 5?? **Descontinuar Producto** (POST Discontinue)

```http
POST https://localhost:44300/api/products/f1e2d3c4-b5a6-7890-cdef-1234567890ab/discontinue
```

**Response (200 OK)**:
```json
{
  "id": "f1e2d3c4-b5a6-7890-cdef-1234567890ab",
  "isDiscontinued": true,
  "discontinuedDate": "2024-01-15T12:00:00Z"
}
```

---

### 6?? **Eliminar Producto** (DELETE - Soft)

```http
DELETE https://localhost:44300/api/products/f1e2d3c4-b5a6-7890-cdef-1234567890ab
```

**Response (204 No Content)**: Sin cuerpo

---

### 7?? **Restaurar Producto** (POST Restore)

```http
POST https://localhost:44300/api/products/f1e2d3c4-b5a6-7890-cdef-1234567890ab/restore
```

**Response (200 OK)**:
```json
{
  "id": "f1e2d3c4-b5a6-7890-cdef-1234567890ab",
  "isActive": true
}
```

---

## ?? **Colección de Postman**

### **Importar Colección**

Crea un archivo `ERP-CRM-API.postman_collection.json`:

```json
{
  "info": {
    "name": "ERP CRM Service API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Customers",
      "item": [
        {
          "name": "Get All Customers",
          "request": {
            "method": "GET",
            "url": "{{baseUrl}}/api/customers"
          }
        },
        {
          "name": "Create Customer",
          "request": {
            "method": "POST",
            "url": "{{baseUrl}}/api/customers",
            "header": [
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"companyName\": \"Test Company\",\n  \"contactFirstName\": \"John\",\n  \"contactLastName\": \"Doe\",\n  \"email\": \"john@testcompany.com\",\n  \"type\": 2,\n  \"creditLimit\": 10000\n}"
            }
          }
        }
      ]
    },
    {
      "name": "Products",
      "item": [
        {
          "name": "Get All Products",
          "request": {
            "method": "GET",
            "url": "{{baseUrl}}/api/products"
          }
        },
        {
          "name": "Create Product",
          "request": {
            "method": "POST",
            "url": "{{baseUrl}}/api/products",
            "header": [
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "body": {
              "mode": "raw",
              "raw": "{\n  \"sku\": \"TEST-001\",\n  \"name\": \"Test Product\",\n  \"category\": 1,\n  \"unitPrice\": 99.99,\n  \"cost\": 50.00,\n  \"stockQuantity\": 100,\n  \"minimumStock\": 10,\n  \"unit\": \"Unit\"\n}"
            }
          }
        }
      ]
    }
  ],
  "variable": [
    {
      "key": "baseUrl",
      "value": "https://localhost:44300"
    }
  ]
}
```

---

## ?? **Manejo de Errores - Ejemplos**

### **400 Bad Request** - Validación fallida
```json
{
  "message": "El modelo de solicitud no es válido.",
  "modelState": {
    "Email": ["El campo Email es obligatorio."],
    "CompanyName": ["Company name cannot exceed 100 characters"]
  }
}
```

### **400 Bad Request** - Regla de negocio
```json
{
  "message": "Ya existe un cliente con ese email"
}
```

### **404 Not Found**
```json
{
  "message": "No se encontró ningún HttpResource que coincida con el URI de solicitud 'https://localhost:44300/api/customers/invalid-id'."
}
```

### **500 Internal Server Error**
```json
{
  "message": "Se ha producido un error."
}
```

---

## ? **Testing Checklist**

### **Customers**
- [ ] Crear cliente válido
- [ ] Crear cliente con email duplicado (debe fallar)
- [ ] Actualizar cliente existente
- [ ] Actualizar cliente inexistente (debe fallar)
- [ ] Suspender cliente
- [ ] Reactivar cliente
- [ ] Ajustar balance (agregar)
- [ ] Ajustar balance (deducir)
- [ ] Eliminar cliente
- [ ] Restaurar cliente

### **Products**
- [ ] Crear producto válido
- [ ] Crear producto con SKU duplicado (debe fallar)
- [ ] Actualizar producto existente
- [ ] Ajustar stock (agregar)
- [ ] Ajustar stock (reducir)
- [ ] Ajustar stock negativo (debe fallar)
- [ ] Actualizar precios
- [ ] Descontinuar producto
- [ ] Eliminar producto
- [ ] Restaurar producto

---

**¡37 endpoints listos para probar!** ??

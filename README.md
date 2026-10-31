# 🚀 EduPlatform - Microservices Architecture & API Gateway

Una solución de arquitectura empresarial basada en microservicios utilizando **.NET 8/9**, enfocada en alta mantenibilidad, seguridad perimetral y alto rendimiento.

> 📌 **Estado del Proyecto:** En desarrollo activo (Fase 1 completada: YARP API Gateway & Identity Management).

---

## 🛠️ Tecnologías y Herramientas

* **Framework:** .NET 8 / .NET 9
* **API Gateway:** YARP (Yet Another Reverse Proxy)
* **Persistencia de Datos:** Dapper (Micro-ORM de alto rendimiento)
* **Base de Datos:** SQL Server
* **Autenticación & Autorización:** JWT (JSON Web Tokens) con políticas de control de acceso basadas en roles (RBAC) y estrategia de rotación de Refresh Tokens.
* **Patrones de Diseño:** Clean Architecture, Repository Pattern, CQRS / Strategy Pattern (en progreso).
* **Contenedores:** Docker & Docker Compose.

---

## 🏛️ Arquitectura Actual


---

## 🔥 Características Implementadas (Fase 1)

- [x] **YARP API Gateway:** Configuración de rutas, clústeres y filtrado perimetral de peticiones.
- [x] **Gestión de Identidad Stateless & Stateful:**
  - Emisión de **Access Tokens (JWT)** con tiempo de expiración corto (15 min) para validación en memoria en el Gateway.
  - Implementación de **Refresh Token Rotation** guardado en SQL Server mediante **Dapper** para renovación transparente de sesión.
- [x] **Seguridad por Roles (RBAC):** Enrutamiento protegido por políticas (`AdminOnly`, `AcademicAccess`) enforced a nivel de Gateway (401 Unauthorized / 403 Forbidden).
- [x] **Patrón Repositorio Decoplado:** Separación limpia de la persistencia de datos mediante `IUserRepository`.

---

# ??? Enrollment Microservices - Sistema de Inscripciones y Pagos

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![SQLite](https://img.shields.io/badge/SQLite-3-003B57?logo=sqlite)](https://www.sqlite.org/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

Sistema distribuido de microservicios para gesti車n de inscripciones con procesamiento de pagos mediante webhook idempotente.

---

## ??? Arquitectura de Microservicios

El sistema est芍 dividido en **4 microservicios independientes**:

---

## ?? Por qu谷 Microservicios

### Decisi車n Arquitect車nica

Se opt車 por una **arquitectura de microservicios** en lugar de un monolito por las siguientes razones:

#### ? Ventajas Implementadas

1. **Separaci車n de Responsabilidades (SRP)**
   - Cada servicio tiene una 迆nica responsabilidad bien definida
   - **Auth Service**: Solo maneja autenticaci車n/autorizaci車n
   - **Enrollment Service**: Solo gestiona inscripciones
   - **Payment Webhook**: Solo procesa notificaciones de pago

2. **Escalabilidad Independiente**

   2.1. Escalar solo el servicio de enrollments si hay alta demanda
   ```bash
   docker-compose up -d --scale enrollment-service=3
   ```
   2.2. Cada microservicio puede escalar seg迆n su carga espec赤fica
   2.3. No desperdicias recursos escalando funcionalidades que no lo necesitan

3. **Despliegue Independiente**
   - Puedes actualizar el webhook de pagos sin tocar el resto del sistema
   - Reduce riesgo de deployments (blast radius limitado)
   - Permite CI/CD independiente por servicio

4. **Aislamiento de Fallos**
   - Si el servicio de pagos falla, las inscripciones siguen funcionando
   - Resilencia mejorada del sistema completo
   - Implementaci車n de Circuit Breakers m芍s efectiva

5. **Flexibilidad Tecnol車gica**
   - Cada servicio puede usar tecnolog赤as diferentes si es necesario
   - En el futuro, podr赤as migrar un servicio a otro lenguaje sin afectar a los dem芍s
   - Facilita adopci車n de nuevas tecnolog赤as gradualmente

6. **Equipos Aut車nomos**
   - Diferentes equipos pueden trabajar en diferentes servicios
   - Menos conflictos en Git
   - Ciclos de desarrollo m芍s r芍pidos

7. **Seguridad Mejorada**
   - El webhook tiene autenticaci車n diferente (API Key)
   - Los servicios internos usan JWT
   - Separaci車n de preocupaciones de seguridad

#### ?? Comparaci車n: Monolito vs Microservicios

| Aspecto | Monolito | Microservicios | ?Por qu谷 gan車? |
|---------|----------|----------------|----------------|
| **Despliegue** | Todo o nada | Independiente por servicio | ? Menos riesgo |
| **Escalabilidad** | Escala todo | Escala solo lo necesario | ? Eficiencia de recursos |
| **Fallas** | Un error tumba todo | Fallas aisladas | ? Mayor disponibilidad |
| **Desarrollo** | Equipos bloqueados | Equipos independientes | ? Velocidad |
| **Complejidad** | Simple al inicio | Mayor complejidad inicial | ?? Trade-off aceptado |

## ?? Por qu谷 .NET 8

### Decisi車n de Tecnolog赤a

Se selecci車n **.NET 8 (LTS)** por los siguientes motivos:

- **Rendimiento mejorado:** .NET 8 ofrece mejoras significativas en el rendimiento de APIs, Entity Framework Core y serializaci車n JSON.
- **Soporte a largo plazo (LTS):** Garantiza estabilidad y soporte extendido por parte de Microsoft.
- **Integraci車n nativa con contenedores:** Soporte optimizado para Docker y Kubernetes, simplificando despliegues.
- **Minimal APIs y mejoras en C# 12:** Facilita el desarrollo de servicios ligeros y de alto rendimiento.
- **Compatibilidad multiplataforma:** Permite ejecutar los servicios en Linux, Windows o macOS sin modificaciones.

---

## Estructura del Proyecto

```
ApiGateway/              # Puerta de entrada que enruta las peticiones
AuthService/      #Microservicio de simulaci車n de Token JWT
    /AuthService.Api 
    /AuthService.Business
EnrollmentsService/      #Microservicio de enrolamiento
    /EnrollmentsService.Api 
    /EnrollmentsService.Business
    /EnrollmentsService.Data
    /EnrollmentsService.Infraestructura
PaymentWebhookService/      #Microservicio de pagos
    /PaymentWebhookService.Api 
    /PaymentWebhookService.Business
    /PaymentWebhookService.Data
    /PaymentWebhookService.Infraestructura
docker-compose.yml          # Orquestaci車n de los microservicios
README.md
```

---

## Requisitos Previos

Antes de comenzar, aseg迆rate de tener instalado:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)  
- [Git](https://git-scm.com/)  
- [Sqlite3](https://sqlite.org/)
- Opcional: [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (para ejecutar localmente sin contenedores)

---

## Pasos para Ejecutar el Proyecto

### 1 Clonar el repositorio

```bash
git clone 
cd proyecto-microservicios-dotnet8
```

### 2 Construir los contenedores

El siguiente comando descarga las im獺genes base y compila cada microservicio:

```bash
docker compose build
```

### 3 Iniciar los servicios

Levanta todos los microservicios, bases de datos y el gateway:

```bash
docker compose up
```

O bien, en segundo plano:

```bash
docker compose up -d
```

### 4 Verificar el estado

Puedes listar los contenedores en ejecuci車n con:

```bash
docker ps
```

### 5 Acceder a los servicios

- **API Gateway:** [http://localhost:5000](http://localhost:5000)  

---

## Comandos 迆tiles

- **Detener los servicios:**
  ```bash
  docker compose down
  ```

- **Reconstruir y reiniciar (si cambiaste c車digo):**
  ```bash
  docker compose up --build
  ```

- **Ver logs en tiempo real:**
  ```bash
  docker compose logs -f
  ```

---

## Tecnolog赤as Utilizadas

- **.NET 8 Web API**
- **Entity Framework Core 8**
- **Sqlite3**
- **Ocelot API Gateway**
- **Docker Compose**
- **Swagger (OpenAPI)**

---

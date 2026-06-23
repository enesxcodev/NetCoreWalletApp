# 💰 NetCore Wallet Application

> **Modern Enterprise Architecture in Action** | Clean Architecture + DDD + CQRS + Event-Driven Architecture

[![NET](https://img.shields.io/badge/.NET-10.0-purple?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue?style=for-the-badge)](https://github.com/ardalis/CleanArchitecture)
[![Docker](https://img.shields.io/badge/Docker-Compose-blue?style=for-the-badge&logo=docker)](https://www.docker.com/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-4.0-orange?style=for-the-badge&logo=rabbitmq)](https://www.rabbitmq.com/)
[![MSSQL](https://img.shields.io/badge/MSSQL-2022-red?style=for-the-badge&logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![MongoDB](https://img.shields.io/badge/MongoDB-Latest-green?style=for-the-badge&logo=mongodb)](https://www.mongodb.com/)
[![Redis](https://img.shields.io/badge/Redis-Alpine-dc382d?style=for-the-badge&logo=redis)](https://redis.io/)
[![Tests](https://img.shields.io/badge/Tests-XUnit-blue?style=for-the-badge)](https://xunit.net/)

---

## 📖 İçindekiler

- [Proje Tanımı](#-proje-tanımı)
- [Mimari Özellikleri](#-mimari-özellikleri)
- [Teknik Stack](#-teknik-stack)
- [Proje Yapısı](#-proje-yapısı)
- [Best Practices](#-best-practices-özeti)
- [Kurulum](#-kurulum--çalıştırma)

---

## 🎯 Proje Tanımı

**NetCore Wallet Application**, kurumsal ölçekte bir para cüzdanı simülasyonudur. **Best practice** ve **en son mimarisi kalıplarının** gerçek senaryolarda implementasyonunu göstermeye çalıştım.

### Proje Hedefleri
✅ **Clean Architecture** ilkelerine tam uygun katman yapısı  
✅ **Domain-Driven Design (DDD)** - zengin domain modeli  
✅ **CQRS** - okuma/yazma ayrımı  
✅ **Event-Driven** - asenkron iş akışları  
✅ **Polyglot Persistence** - MSSQL + MongoDB + Redis  
✅ **Unit Tests** - XUnit + Moq  
✅ **Docker** - multi-container orchestration

---

## 🏗️ Mimari Özellikleri

### 💎 1. Domain-Driven Design (DDD) & Rich Domain Model

Veri tabanı odaklı *anemic domain* yaklaşımından uzak, **tüm iş kuralları doğrudan entity'lerde** tanımlı:

```csharp
public class Wallet : BaseEntity
{
	public string Code { get; private init; }
	public decimal Balance { get; private set; }

	public void Deposit(decimal amount)
	{
		if (amount <= 0) 
			throw new DomainException("Tutar pozitif olmalı");
		Balance += amount;
	}
}
```

**Avantajlar:**
- ✅ Type-safe business logic
- ✅ Data integrity guaranteed
- ✅ Reusable domain rules
- ✅ No duplicate validation

---

### 🔄 2. CQRS Pattern (Read/Write Segregation)

Yazma ve okuma **tamamen ayrı** veri tabanlarında:

```
WRITE: MSSQL (ACID, normalized)
READ:  
  - Hot: Redis (cache-aside)
  - Historical: MongoDB (flexible schema)
  - Real-time: MSSQL (direct)
```

**Avantajlar:**
- ✅ Independent scalability
- ✅ Each DB optimized for purpose
- ✅ Write consistency + Read performance
- ✅ Elastic read layer

---

### ⚡ 3. End-to-End Async Architecture

**Tüm I/O asenkron** - Controller → Handler → Repository → Database

```csharp
public async Task<IActionResult> Register(RegisterCreateCommand command, CancellationToken ct)
{
	var result = await _mediator.Send(command, ct);
	return CreateActionResult(result);
}
```

**Avantajlar:**
- ✅ High throughput
- ✅ Thread pool efficiency
- ✅ Concurrent requests
- ✅ Cancellation support

---

### 🎯 4. Event-Driven Architecture

**MassTransit + RabbitMQ** ile asenkron messaging:

```
User Registered → Event Published → Consumer Processes → Wallet Created
(No blocking, loosely coupled)
```

**Avantajlar:**
- ✅ Loose coupling
- ✅ Asynchronous processing
- ✅ Built-in retry logic
- ✅ Audit trail

---

### 🛡️ 5. Result Pattern & Type-Safe Errors

```csharp
public async Task<Result<TransactionDto>> TransferAsync(...)
{
	if (insufficient) 
		return Result<TransactionDto>.Failure("Yetersiz bakiye", ResultStatus.BadRequest);

	return Result<TransactionDto>.Success(tx, ResultStatus.Ok);
}

// JSON Response
{
  "isSuccess": true,
  "status": 200,
  "data": { "id": "...", "amount": 100 },
  "error": null
}
```

---

## 🛠️ Teknik Stack

### Core (Domain & Application)
- **C# 13** - Primary constructors, records
- **MediatR** - CQRS + handlers
- **FluentValidation** - Declarative rules
- **AutoMapper** - DTO mapping
- **MassTransit** - Event bus

### Infrastructure
- **EF Core 10** - ORM + migrations
- **Dapper** - Complex queries
- **MSSQL 2022** - Relational DB (Write)
- **MongoDB** - Document DB (Read)
- **Redis** - Cache layer
- **RabbitMQ** - Message broker

### Presentation
- **ASP.NET Core 10** - REST API
- **JWT Bearer** - Authentication
- **Scalar UI** - API documentation
- **Serilog** - Structured logging

### Testing & DevOps
- **xUnit** - Tests
- **Moq** - Mocking
- **Docker** - Containers
- **Docker Compose** - Orchestration

---

## 📁 Proje Yapısı

```
Wallet/
├── Core/
│   ├── Domain/
│   │   ├── Entities/          ← Rich domain models
│   │   ├── Exceptions/        ← Business exceptions
│   │   └── Common/
│   │
│   └── Application/
│       ├── Common/            ← Result pattern, Behaviors
│       ├── Contracts/         ← Interfaces (IRepository, etc)
│       └── Features/          ← CQRS Commands/Queries/Handlers
│
├── Infrastructure/
│   └── Persistence/
│       ├── Context/           ← EF Core DbContext
│       ├── Repository/        ← Repository pattern impl
│       ├── Consumers/         ← Event handlers
│       └── Services/          ← Business services
│
├── Presentation/
│   └── WebApi/
│       ├── Controllers/       ← REST endpoints
│       ├── Middlewares/       ← Exception handling
│       ├── Program.cs         ← Auto-migration setup
│       └── appsettings.json
│
├── Tests/                      ← Unit tests
└── docker-compose.yml
```

---

## ✨ Best Practices Özeti

### SOLID Principles
- ✅ **S** - Single Responsibility → Her class bir nedenden değişir
- ✅ **O** - Open/Closed → Extension açık, modification kapalı
- ✅ **L** - Liskov Substitution → Inheritance güvenli
- ✅ **I** - Interface Segregation → Focused interfaces
- ✅ **D** - Dependency Inversion → Abstraction'lara bağımlı

### Design Patterns
- ✅ **Repository** - Data access abstraction
- ✅ **Unit of Work** - Atomic transactions
- ✅ **Decorator** - Pipeline behaviors
- ✅ **Observer** - Event-driven
- ✅ **Strategy** - Multiple implementations (Dapper vs EF)

### Code Quality
- ✅ **No magic strings** → Constants/Enums
- ✅ **No null checks hell** → Guard clauses
- ✅ **No circular deps** → DI
- ✅ **No fat controllers** → MediatR handlers
- ✅ **No data leaks** → Result pattern
- ✅ **Type-safe** → Record types, primary constructors
- ✅ **Async-first** → Cancellation tokens
- ✅ **Testable** → Dependency injection

---

## 📦 Kurulum & Çalıştırma

### Quick Start (Docker)

```bash
# Klonla
git clone https://github.com/enesxcodev/NetCoreWalletApp.git
cd Wallet

# Başlat
docker-compose up -d --build

# Logları izle
docker-compose logs -f wallet-api

# API'ya eriş
http://localhost:5001/scalar/
```

### Services & Ports

| Servis | Port | URL |
|--------|------|-----|
| **API** | 5001 | http://localhost:5001 |
| **API Docs** | 5001 | http://localhost:5001/scalar/ |
| **SQL Server** | 1433 | localhost:1433 |
| **RabbitMQ** | 15672 | http://localhost:15672 |
| **MongoDB** | 27017 | localhost:27017 |
| **Redis** | 6379 | localhost:6379 |

### Local Development

```bash
# Migrationları oluştur
dotnet ef migrations add [Name] -p Infrastructure/Persistence

# Testleri çalıştır
dotnet test Tests/Test/Test.csproj

# F5 ile başlat (appsettings.Development.json configure et)
```

---

## 🔌 API Endpoints

### Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "Ahmet",
  "lastName": "Yıldırım",
  "email": "ahmet@example.com",
  "userName": "ahmetyildirim",
  "password": "SecurePass123!"
}

Response: 201 Created
{ "isSuccess": true, "status": 201, "data": "user-id" }
```

### Login
```http
POST /api/auth/login
{ "userName": "ahmetyildirim", "password": "SecurePass123!" }

Response: 200 OK
{ "isSuccess": true, "data": { "token": "jwt...", "expiresIn": 3600 } }
```

### Wallet Deposit
```http
POST /api/wallet/deposit
Authorization: Bearer {token}
{ "amount": 100 }

Response: 200 OK
{ "isSuccess": true, "data": { "walletId": "...", "newBalance": 100 } }
```

---

## 📊 Proje Güçlü Yanları

1. ✅ **Solid Architecture** - Perfect layering
2. ✅ **Rich Domain Model** - Type-safe business logic
3. ✅ **CQRS** - Read/write separation
4. ✅ **Event-Driven** - Loose coupling
5. ✅ **Async-First** - High throughput
6. ✅ **Result Pattern** - Type-safe errors
7. ✅ **Polyglot Persistence** - Optimized for each use case
8. ✅ **Repository Pattern** - Testable, abstract
9. ✅ **Unit of Work** - Transaction safety
10. ✅ **Comprehensive Tests** - XUnit + Moq
11. ✅ **Docker Ready** - Production-ready
12. ✅ **Modern C#** - Cutting-edge features
13. ✅ **JWT Security** - Token-based auth
14. ✅ **Structured Logging** - Observable
15. ✅ **Global Exception Handler** - Centralized error handling

---

## 🚀 Öğrenme Kaynakları

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://www.domainlanguage.com/ddd/)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [Result Pattern](https://www.youtube.com/watch?v=bUQY-RO1gvs)

---

**⭐ Eğer yararlı oldu, bir star vermeyi unutmayın!**

Temiz kodlamalar! 🚀

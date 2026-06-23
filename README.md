# 💰 Wallet Simulation & Microservice Infrastructure

[![NET](https://img.shields.io/badge/.NET-8.0%20%2F%209.0-purple?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-24.0-blue?style=for-the-badge&logo=docker)](https://www.docker.com/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-4.0-orange?style=for-the-badge&logo=rabbitmq)](https://www.rabbitmq.com/)
[![MSSQL](https://img.shields.io/badge/MSSQL-2022-red?style=for-the-badge&logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![MongoDB](https://img.shields.io/badge/MongoDB-Latest-green?style=for-the-badge&logo=mongodb)](https://www.mongodb.com/)
[![Redis](https://img.shields.io/badge/Redis-Alpine-darkred?style=for-the-badge&logo=redis)](https://redis.io/)

## 🚀 Proje Hakkında
Bu proje, ölçeğinden bağımsız olarak modern bir yazılım mimarisinin nasıl kurgulanması gerektiğini gösteren, **Best Practice** standartlarında hazırlanmış bir **Cüzdan (Wallet) Simülasyonu**dur. 

Projenin asıl amacı iş mantığının büyüklüğü değil; **Clean Architecture**, **DDD (Domain-Driven Design)**, **CQRS** gibi kurumsal mimari kalıplarının; **RabbitMQ**, **Redis** ve **MongoDB** gibi teknolojilerle en doğru ve efektif şekilde nasıl entegre edileceğini canlı bir senaryo üzerinden simüle etmektir.

---

## 🏗️ Mimari ve Tasarım Kalıpları (Architectural Patterns)

Project, tamamen gevşek bağlı (loosely coupled) ve yüksek sürdürülebilirlik (maintainability) hedefleyen **Clean Architecture** prensiplerine göre katmanlandırılmıştır.

### 💎 Domain-Driven Design (DDD) & Rich Domain Model
* Veri tabanı odaklı (Anemic Domain) yaklaşım yerine, tüm iş kurallarının ve validasyonların entity'lerin kendi içinde yönetildiği **Rich Domain Model** benimsenmiştir.
* Nesne türetim süreçleri kontrollü hale getirilmiş, veri bütünlüğü domain seviyesinde garanti altına alınmıştır.

### ⚡ CQRS & MediatR Pipeline Filters
* Okuma (Query) ve Yazma (Command) operasyonları **CQRS** ile tamamen birbirinden ayrılmıştır.
* **MediatR Pipeline Behaviors** kullanılarak; cross-cutting concern yapıları (Validation, Logging vb.) ana iş koduna (Handler) dokunmadan, istek araya kesilerek filtre seviyesinde işlenir.

### 🗄️ Polyglot Persistence (Çoklu Veri Tabanı Mimarisi)
Sistemde her iş yükü için en doğru veri tabanı türü seçilmiştir:
* **MSSQL (EF Core):** Finansal, ilişkisel ve ACID kurallarına sıkı sıkıya bağlı ana işlemler (User, Wallet vb.) için.
* **MongoDB:** Hızlı şema esnekliği gerektiren ve yüksek okuma performansı hedeflenen raporlama/loglama süreçleri için.
* **Redis:** Dağıtık önbellekleme (Distributed Caching) ile mükemmel performans optimizasyonu için.

---

## 🛠️ Katmanlar ve Öne Çıkan Özellikler

### 🛡️ Core (Domain & Application)
* **Primary Constructors & Record Yapıları:** Modern C# (.NET 8+) özellikleri aktif olarak kullanılmış; DTO'lar, Command ve Query nesneleri `record` yapılarıyla immutability (değişmezlik) esasına göre tasarlanmıştır. Dependency Injection süreçleri `Primary Constructor` ile sadeleştirilmiştir.
* **Result Pattern:** Metotlardan geriye `true/false` veya `exception` fırlatmak yerine, operasyonun başarı durumunu, mesajını ve verisini tip güvenli (Type-safe) taşıyan kurumsal **Result Pattern** (`Result<T>`) uygulanmıştır.
* **CQRS Validation Behavior:** İstekler işlenmeden önce **FluentValidation** kurallarından geçer. Geçersiz bir istek durumunda hata, MediatR pipeline üzerinde yakalanarak Result tipine dönüştürülür.
* **Enums:** Sistem içindeki tüm durum yönetimleri (örn: Transaction Status) güçlü enum yapılarıyla kontrol edilir.

### 💾 Infrastructure (Persistence & Messaging)
* **EF Core & Fluent API Configuration:** Veri tabanı modellerinin eşleşmesi ve kuralları (Precision, Scale, Index vb.) `Identity` bağımlılıklarından uzak, tamamen **Fluent API** konfigürasyon sınıfları (`IEntityTypeConfiguration<T>`) ile merkezi olarak yönetilir.
* **Dapper Entegrasyonu:** Performans kritik okuma (Query) operasyonlarında EF Core overhead'inden kaçınmak adına hafif siklet ORM olan **Dapper** entegre edilmiştir.
* **Generic Repository & Unit of Work (UoW):** Veri tabanı işlemlerinde soyutlama (Abstraction) katmanı olarak Generic Repository Interface tasarlanmış; dağıtık işlemlerin tek bir transaction altında atomik olarak tamamlanması **Unit of Work** ile güvenceye alınmıştır.
* **Event Background Service (RabbitMQ Worker):** MassTransit veya saf `RabbitMQ.Client` kullanılarak asenkron mesajlaşma altyapısı kurulmuştur. Kullanıcı kayıt olduğunda tetiklenen event, arka planda sürekli dinlemede olan bir `.NET BackgroundService (Worker)` tarafından asenkron olarak yakalanır ve cüzdan oluşturma süreci (Event-Driven) işletilir.

### 🌐 Presentation (WebAPI)
* **Global Exception Handler Middleware:** Uygulama içinde oluşabilecek beklenmedik tüm çalışma zamanı (Runtime) hataları merkezi bir middleware katmanında yakalanır, loglanır ve dış dünyaya standartlaştırılmış güvenli bir JSON objesi olarak dönülür.
* **JWT Token Authentication:** Kullanıcı oturum ve yetkilendirme süreçleri güvenli **JSON Web Token (JWT)** altyapısı ile korunmaktadır.
* **Scalar OpenAPI:** Klasik Swagger arayüzü yerine, modern ve efektif bir API dökümantasyon standardı sunan **Scalar UI** entegre edilmiştir.

---

## 🐳 Docker & Docker Compose Altyapısı

Projenin yerel bilgisayarda hiçbir harici kuruluma ihtiyaç duymadan tek tıkla çalışabilmesi için tüm bağımlılıklar izole bir **Docker Network** üzerinde kurgulanmıştır.

`docker-compose.yml` içerisinde yer alan servis yapılandırması:
* **wallet-api:** ASP.NET Core Web API uygulaması (`depends_on` ve `healthcheck` mekanizmaları ile diğer servislerin hazır olması beklenir).
* **sqlserver:** Microsoft SQL Server 2022 (`WalletDb` veritabanı).
* **rabbitmq:** RabbitMQ 4.0 Management Alpine sürümü.
* **mongodb:** Raporlama veri tabanı.
* **redis:** Dağıtık cache altyapısı.

---

## 🏃‍♂️ Projeyi Yerelde Çalıştırma (Quick Start)

Projenin `Program.cs` dosyasına entegre edilen **Otomatik Migration** mekanizması sayesinde, veri tabanını elinizle oluşturmanıza veya migration basmanıza gerek yoktur. Konteyner ayağa kalktığı anda tablolar otomatik olarak oluşturulur.

1. Projeyi bilgisayarınıza klonlayın:
   ```bash
   git clone [https://github.com/enesxcodev/NetCoreWalletApp.git](https://github.com/enesxcodev/NetCoreWalletApp.git)
   cd Wallet
   
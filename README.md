# 💰 Wallet Simulation & Microservice Infrastructure

[![NET](https://img.shields.io/badge/.NET-8.0%20%2F%209.0-purple?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-24.0-blue?style=for-the-badge&logo=docker)](https://www.docker.com/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-4.0-orange?style=for-the-badge&logo=rabbitmq)](https://www.rabbitmq.com/)
[![MSSQL](https://img.shields.io/badge/MSSQL-2022-red?style=for-the-badge&logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![MongoDB](https://img.shields.io/badge/MongoDB-Latest-green?style=for-the-badge&logo=mongodb)](https://www.mongodb.com/)
[![Redis](https://img.shields.io/badge/Redis-Alpine-darkred?style=for-the-badge&logo=redis)](https://redis.io/)

## 🚀 Proje Hakkında
Bu proje, ölçeğinden bağımsız olarak modern bir yazılım mimarisinin nasıl kurgulanması gerektiğini gösteren, **Best Practice** standartlarında hazırlanmış bir **Cüzdan (Wallet) Simülasyonu**dur. 

Projenin asıl amacı sadece iş mantığını kodlamak değil; **Clean Architecture**, **DDD (Domain-Driven Design)**, **CQRS** gibi kurumsal mimari kalıplarının; **RabbitMQ**, **Redis** ve **MongoDB** gibi teknolojilerle en doğru ve efektif şekilde nasıl entegre edileceğini canlı bir senaryo üzerinden simüle etmektir.

---

## 🏗️ Mimari ve Tasarım Kalıpları (Architectural Patterns)

### 💎 Domain-Driven Design (DDD) & Rich Domain Model
* Veri tabanı odaklı (Anemic Domain) yaklaşım yerine, tüm iş kurallarının ve validasyonların entity'lerin kendi içinde yönetildiği **Rich Domain Model** benimsenmiştir.
* Nesne türetim süreçleri kontrollü hale getirilmiş, veri bütünlüğü domain seviyesinde garanti altına alınmıştır.
* Migrationları sistem çalıştığında kendi yapıyor

### 🔄 CQRS & Okuma/Yazma Ayrımı (Read/Write Segregation) Stratejisi
Sistem üzerinde performans optimizasyonunu en üst düzeye çıkarmak amacıyla verinin yazıldığı ve okunduğu kanallar (CQRS) mimari seviyede tamamen ayrılmıştır:
* **Yazma (Write) Operasyonları:** Tüm veri ekleme, güncelleme ve silme (Command) işlemleri, finansal tutarlılığı ve veri bütünlüğünü (ACID) garanti altına almak amacıyla **MSSQL** veri tabanına yapılır.
* **Okuma (Read) Operasyonları:** Verinin boyutuna ve ihtiyaç duyulan performans gereksinimine göre akıllı yönlendirme yapılır:
  * **Yüksek Hızlı Önbellekleme (Redis):** Sık erişilen, görece küçük boyutlu ve anlık dönülmesi gereken veriler için **Redis Dağıtık Önbellek (Distributed Cache)** katmanı kullanılır.
  * **Büyük Veri ve Esnek Raporlama (MongoDB):** Veri hacminin büyük olduğu, karmaşık ilişkiler yerine şema esnekliği gerektiren geçmiş işlemler (Transaction Logs) ve cüzdan raporlama süreçlerinde okuma istekleri doğrudan **MongoDB** üzerinden karşılanır.

### ⚡ %100 Asenkron Mimari (End-to-End Async Pipeline)
* Uygulama içerisindeki tüm akış (Controller seviyesinden başlayarak, MediatR Handler'lar, EF Core/Dapper repository katmanları ve harici mesaj kuyruğu entegrasyonları dahil) tamamen **Asenkron (`async/await`)** olarak tasarlanmıştır.
* Bu sayede I/O bound (Girdi/Çıktı) işlemlerinde thread bloklanmalarının önüne geçilmiş ve uygulamanın aynı anda karşılayabileceği istek kapasitesi (Throughput) maksimuma çıkarılmıştır.

---

## 🛠️ Katmanlar ve Öne Çıkan Özellikler

### 🛡️ Core (Domain & Application)
* **Primary Constructors & Record Yapıları:** Modern C# özellikleri aktif olarak kullanılmış; DTO'lar, Command ve Query nesneleri `record` yapılarıyla immutability (değişmezlik) esasına göre tasarlanmıştır. Dependency Injection süreçleri `Primary Constructor` ile sadeleştirilmiştir.
* **Result Pattern:** Metotlardan geriye `true/false` veya beklenmedik `exception` fırlatmak yerine, operasyonun başarı durumunu, mesajını ve verisini tip güvenli (Type-safe) taşıyan kurumsal **Result Pattern** (`Result<T>`) uygulanmıştır.
* **CQRS Validation Behavior:** İstekler işlenmeden önce **FluentValidation** kurallarından geçer. Geçersiz bir istek durumunda hata, MediatR pipeline üzerinde yakalanarak Result tipine dönüştürülür.
* **Enums:** Sistem içindeki tüm durum yönetimleri (örn: Transaction Status) güçlü enum yapılarıyla kontrol edilir.

### 💾 Infrastructure (Persistence & Messaging)
* **EF Core & Fluent API Configuration:** Veri tabanı modellerinin eşleşmesi ve kuralları (Precision, Scale, Index vb.) `Identity` bağımlılıklarından uzak, tamamen **Fluent API** konfigürasyon sınıfları (`IEntityTypeConfiguration<T>`) ile merkezi olarak yönetilir.
* **Dapper Entegrasyonu:** Performans kritik ve ham SQL gücü gerektiren okuma (Query) operasyonlarında EF Core overhead'inden kaçınmak adına hafif siklet ORM olan **Dapper** entegre edilmiştir.
* **Generic Repository & Unit of Work (UoW):** Veri tabanı işlemlerinde soyutlama (Abstraction) katmanı olarak Generic Repository Interface tasarlanmış; dağıtık işlemlerin tek bir transaction altında atomik olarak tamamlanması **Unit of Work** ile güvenceye alınmıştır.
* **Event Background Service (RabbitMQ Worker):** MassTransit kullanılarak asenkron mesajlaşma altyapısı kurulmuştur. Kullanıcı kayıt olduğunda tetiklenen event, arka planda sürekli dinlemede olan bir `.NET BackgroundService (Worker)` tarafından asenkron olarak yakalanır ve cüzdan oluşturma süreci (Event-Driven) işletilir.

### 🌐 Presentation (WebAPI)
* **Global Exception Handler Middleware:** Uygulama içinde oluşabilecek beklenmedik tüm çalışma zamanı (Runtime) hataları merkezi bir middleware katmanında yakalanır, loglanır ve dış dünyaya standartlaştırılmış güvenli bir JSON objesi olarak dönülür.
* **JWT Token Authentication:** Kullanıcı oturum ve yetkilendirme süreçleri güvenli **JSON Web Token (JWT)** altyapısı ile korunmaktadır.
* **Scalar OpenAPI:** Klasik Swagger arayüzü yerine, modern ve efektif bir API dökümantasyon standardı sunan **Scalar UI** entegre edilmiştir.

---

## 🐳 Docker & Docker Compose Altyapısı

Projenin yerel bilgisayarda hiçbir harici kuruluma ihtiyaç duymadan tek tıkla çalışabilmesi için tüm bağımlılıklar izole bir **Docker Network** üzerinde kurgulanmıştır. 

`docker-compose.yml` içerisindeki ortam değişkenleri (Environment Variables), `appsettings.json` içerisindeki yerel ayarları otomatik olarak ezerek (override) konteynerlar arası iletişimin kusursuz akmasını sağlar.

### 🔌 Konteyner ve Port Yapılandırması

| Servis Adı | İmaj (Image) | Dış Port | İç Port | Açıklama |
| :--- | :--- | :--- | :--- | :--- |
| **wallet-api** | `wallet-app-api` (Local Build) | **5001** | 8080 | ASP.NET Core Web API Uygulaması |
| **sqlserver** | `mcr.microsoft.com/mssql/server:2022-latest` | **1433** | 1433 | Ana İlişkisel Veri Tabanı (Write DB) |
| **rabbitmq** | `rabbitmq:4.0-management-alpine` | **5672 / 15672** | 5672 / 15672 | Mesaj Kuyruğu ve Yönetim Paneli |
| **mongodb** | `mongo:latest` | **27017** | 27017 | Raporlama/Büyük Veri Tabanı (Read DB) |
| **redis** | `redis:alpine` | **6379** | 6379 | Dağıtık Önbellek Katmanı (Cache) |

> ℹ️ **Akıllı Başlatma (Healthcheck):** > `wallet-api` konteynerı, `depends_on` altındaki `condition: service_healthy` kuralları sayesinde `sqlserver` ve `rabbitmq` servislerinin sadece açılmasını değil, tamamen çalışmaya hazır olduğunu denetler ve ardından ayağa kalkar. Bu sayede "Connection Refused" hataları kalıcı olarak engellenmiştir.

---

## 🏃‍♂️ Projeyi Yerelde Çalıştırma (Quick Start)

Projenin `Program.cs` dosyasına entegre edilen **Otomatik Migration** mekanizması sayesinde, veri tabanını elinizle oluşturmanıza veya harici araçlarla migration basmanıza gerek yoktur. Konteyner ayağa kalktığı anda MSSQL üzerindeki tablolar otomatik olarak inşa edilir.

### Gereksinimler
* Bilgisayarınızda **Docker Desktop**'ın kurulu ve çalışır durumda olması yeterlidir.

### Kurulum Adımları

1. Projeyi bilgisayarınıza klonlayın:
   ```bash
   git clone [https://github.com/kullanici_adin/Wallet.git](https://github.com/kullanici_adin/Wallet.git)
   cd Wallet
   ```
2. Docker Compose ile tüm ekosistemi tek komutla, arka planda (detached mode) çalışacak şekilde inşa edin ve ayağa kaldırın:
	```bash
	docker compose up -d --build
	```
3. Tüm konteynerların durumunu ve loglarını kontrol etmek için:
	```bash
	docker compose ps
	docker compose logs -f wallet-api
	👉 http://localhost:5001/scalar/
	```
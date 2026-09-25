# Wallet Frontend

React, Vite ve TypeScript ile geliştirilen Wallet arayüzü.

## Geliştirme

```sh
npm install
Copy-Item .env.example .env
npm run dev
```

API'yi Docker ile açmak için repo kökünde `docker compose up -d --build` çalıştırın. Ardından
`http://localhost:5173` adresinden Wallet arayüzünü, `http://localhost:5001` adresinden API'yi açın.
Geliştirme sunucusunu ayrı çalıştırıyorsanız compose içindeki `frontend` servisini başlatmayın; ikisi de
5173 portunu kullanır. `docker compose down` servisleri durdurur. Veritabanı verilerini silmek için
`-v` eklemeyin; bu seçenek volume'ları da siler.

API adresi `VITE_API_BASE_URL` ile belirlenir; varsayılan adres `http://localhost:5001`.

## Kontroller

```sh
npm run lint
npm run typecheck
npm test
npm run build
```

Kimlik doğrulama uçları `POST /api/Auth/login` ve `POST /api/Auth/register` adreslerini kullanır. Başarılı cevap backend'in `Result<T>` zarfından okunur. JWT yalnızca açık sekmenin oturum depolamasında tutulur.

## Wallet ekranları

- `/dashboard`: Güncel bakiye, cüzdan kodu, hızlı işlemler ve son transferler.
- `/deposit` ve `/withdraw`: Pozitif tutar doğrulamasıyla para yatırma/çekme.
- `/transfer`: Özet ve onay adımı. Ağ hatasında aynı `Idempotency-Key` ile yeniden deneme.
- `/transactions`: MongoDB okuma modelinden gelen transfer geçmişi ve sayfalama.
- `/contacts`: Cüzdanı olan kullanıcıları ad soyad, kullanıcı adı veya e-postayla arama; cüzdan kodunu panoya kopyalama.
- `/architecture`: MSSQL → Outbox → RabbitMQ → MongoDB → Redis veri yolunu açıklar. Kesinti düğmeleri eğitim simülasyonudur; gerçek para işlemi yapmaz ve servis sağlığını ölçmez.

Dashboard için API'de `GET /api/Wallet` bulunur. Yerel Vite adresi (`http://localhost:5173`) API CORS politikasında açıktır. API farklı adreste çalışıyorsa `.env` içindeki `VITE_API_BASE_URL` değerini güncelleyin.

Transfer kişileri `GET /api/Wallet/contacts?search=...&page=1&pageSize=10` adresinden gelir. Yanıt ad, soyad, kullanıcı adı ve cüzdan kodunu içerir; e-posta yalnızca sunucu tarafında arama için kullanılır. Kullanıcının kendi hesabı listelenmez.

## Docker ve testler

- Frontend Docker imajı `Presentation/Frontend/Dockerfile` üzerinden üretilir; Nginx SPA rotalarını `index.html`'e yönlendirir.
- `docker compose up -d --build` tüm sistemi `http://localhost:5173` adresinde çalıştırır.
- `npm run lint`, `npm run typecheck`, `npm test` ve `npm run build` kalite kontrolleridir.
- `npm run test:e2e` Playwright ile girişten test transferine kadar akışı çalıştırır. `npm run test:e2e -- --headed` görünür Chrome penceresinde, `npm run test:e2e:ui` etkileşimli Playwright test panelinde çalıştırır. Bu ayar sistemde Google Chrome bulunmasını gerektirir. E2E API cevaplarını tarayıcıda taklit eder; gerçek veritabanına işlem yazmaz.

Mimari sayfasındaki servis kartları altyapının görevlerini anlatır, canlı servis durumu değildir. Gerçek servis sağlığı için backend health endpoint'leri ve gözlemleme altyapısı ayrıca eklenmelidir.

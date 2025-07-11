### Audiophile E-commerce web application
---

## 🛠️ Tecnologie utilizzate

### Frontend
- **Angular 17+**
- **SCSS / Tailwind CSS**
- - **TypeScript**, Reactive Forms, Routing, Lazy Loading

### Backend
- **ASP.NET Core Web API**
- **Entity Framework Core**
- **SQLite / SQL Server**
- **Architettura a microservizi**
- **Pattern multilivello (Controller → Service → Data → DTO/Model)**
- **Dependency Injection nativa**
- **Swagger**
---
### 🔹 Radice del progetto
```
/ecommerce-app
│
├── frontend/                 <-- Angular App
├── backend/                  <-- ASP.NET Web API
├── README.md
└── database/                 <-- Script SQL
```
### 📁 Frontend (Angular)

```
/frontend
│
├── src/
│   ├── app/
│   │   ├── core/             <-- Auth, Guards, Services comuni
│   │   ├── shared/           <-- Componenti condivisi (header, footer, modals)
│   │   ├── cart/             <-- Gestione carrello
│   │   ├── checkout/         <-- Checkout form e validazioni
│   │   ├── products/         <-- Product list, detail
│   │   ├── order/            <-- Order summary, confirmation
│   │   ├── models/           <-- Interfaces e tipi TypeScript
│   │   └── app-routing.module.ts
│   └── assets/
│       └── images/           <-- Immagini prodotti
├── environments/
│   └── environment.ts        <-- API URLs
└── angular.json
```

### 📁 Backend (ASP.NET Web API)

```
/backend
│
├── Ecommerce.API/
│   ├── Controllers/
│   │   ├── ProductsController.cs
│   │   ├── OrdersController.cs
│   │   └── CartController.cs (opzionale, se persistente lato server)
│   ├── Models/
│   │   ├── Product.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   └── CustomerInfo.cs
│   ├── DTOs/
│   │   └── OrderDTO.cs, ProductDTO.cs
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── SeedData.cs
│   ├── Services/
│   │   └── OrderService.cs
│   └── Program.cs
├── Ecommerce.API.sln
└── appsettings.json
```
---

🙋‍♂️ Author

### Mohamed Mousa

🔗 Portfolio: http://mohamedmousa.it

🔗 LinkedIn:  https://www.linkedin.com/in/mohamedmousa-/

### 🛍️ Audiophile E-commerce Web Application
---
<img width="700" height="513" alt="project-overview" src="https://github.com/user-attachments/assets/f82c3092-c955-494c-8b13-e8431759087c" />


## 🛠️ Technologies Used

### 🎨 Frontend – Angular

- **Angular 17+**

- **TypeScript**

- **Reactive Forms, Routing, Lazy Loading**

- **Bootstrap 5 + Custom CSS**

- **HTTPClient for backend communication**

---

### ⚙️ Backend

- **ASP.NET Core Web API**

- **Entity Framework Core (ORM)**

- **SQL Server LocalDB (or SQLite as alternative)**

- **Swagger for interactive API documentation**

- **Postman for API testing**

- **Microservices-inspired architecture**

- **Multi-layered pattern → Controller → Services → Data → Models/DTOs**

- **Built-in Dependency Injection**

- **Data Annotations & Fluent API for validation and DB configuration**

- **EF Core Migrations to manage database schema**

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
│   │   ├── core/               <-- Auth, Interceptors, Global services
│   │   ├── shared/             <-- Reusable UI components (navbar, footer, modals)
│   │   ├── pages/              <-- Main route pages
│   │   │   ├── home/           <-- Homepage with hero section and highlights
│   │   │   ├── categories/     <-- Category listing: Headphones, Speakers, Earphones
│   │   │   ├── products/       <-- Product detail page
│   │   │   ├── cart/           <-- Cart page and logic
│   │   │   ├── checkout/       <-- Checkout form and validation
│   │   │   └── order/          <-- Order confirmation / summary
│   │   ├── models/             <-- TypeScript interfaces and data types
│   │   ├── directives/         <-- Custom Angular directives (e.g., hover effects)
│   │   ├── pipes/              <-- Custom Angular pipes (e.g., truncate, currency formatting)
│   │   └── app-routing.module.ts
│   ├── assets/
│   │   └── images/             <-- Product images and assets
│   ├── environments/
│   │   └── environment.ts      <-- API base URLs and environment config
│   └── angular.json
```

### 📁 Backend (ASP.NET Web API)

```
/backend
│
├── Ecommerce.API/
│   ├── Controllers/
│   │   ├── ProductsController.cs
│   │   ├── OrdersController.cs
│   │   └── CartController.cs
│   ├── Models/
│   │   ├── Cart.cs, CartItem.cs
│   │   ├── Product.cs
│   │   ├── Order.cs, OrderItem.cs
│   │   └── CustomerInfo.cs
│   ├── DTOs/
│   │   └── OrderDTO.cs,
│   │   └── ProductDTO.cs
│   │   └── OrderItem.cs
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── SeedData.cs
│   ├── Services/
│   │   ├── ProductService.cs / IProductService.cs
│   │   ├── CartService.cs / ICartService.cs
│   │   ├── OrderService.cs / IOrderService.cs
│   │   └── SeedDataService.cs / ISeedDataService.cs
│   ├── Migrations/  <-- EF Core DB migrations
│   └── Program.cs   <-- App entry point & service configuration
├── Ecommerce.API.sln
└── appsettings.json
```
---

🙋‍♂️ Author

### Mohamed Mousa

🔗 Portfolio: http://mohamedmousa.it

🔗 LinkedIn:  https://www.linkedin.com/in/mohamedmousa-/

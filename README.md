<div align="center">
<h1 align="center">🛒 Ez2Buy – E-Commerce Web App (ASP.NET Core MVC)</h1>
Ez2Buy is an eCommerce web application built with ASP.NET Core MVC, providing a modern and secure platform for managing products, orders, and customers online.
<br />
<br />
<a href="https://ez2buy.azurewebsites.net"><strong>➥ Live Website</strong></a>
<br />
<br />
</div>

## 1. Project Requirements

### 1.1 Functional Requirements

#### Administration Features
- **User Management:** Assign roles and manage user accounts
- **Order Management:** View and update order statuses
- **Product & Category Management:** Create, update, and delete products and categories

#### Customer Features
- **Product Catalog:** Browse products using pagination
- **Shopping Cart:** Add/remove items and proceed to checkout
- **Order Processing:** Place orders and track order status
- **Payment Integration:** Secure payment gateway support
- **Notifications:** Email confirmations for orders and account activities

### 1.2 Non-Functional Requirements
- **Usability:** Responsive design for all device types
- **Security:** Role-based access control and data protection
- **Reliability:** Comprehensive error handling and logging
- **Maintainability:** Clean code architecture with documentation
- **Testability:** Extensive unit test coverage

## 2. Architecture Overview

### 2.1 Layered Architecture
- **Presentation Layer:**  MVC Controllers, Views, and ViewModels
- **Data Access Layer:** UnitOfWork and Repository implementations
- **Data Layer:** Entity Framework Core with SQL Server

### 2.2 Design Patterns
- **Repository Pattern:** Abstraction for data access operations
- **Unit of Work Pattern:** Transaction management
- **MVC Pattern:** Separation of concerns in UI design
- **ViewModel Pattern:** Data shaping for presentation
- **Session Management:** Shopping cart data persistence

## 3. Technology Stack

- **Framework:** ASP.NET 8.0 Core MVC
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity
- **Frontend:** HTML5, CSS3, Bootstrap, jQuery
- **Database:** SQL Server
- **Cloud Platform:** Microsoft Azure

## 4. Future Enhancements

- **API Development:** RESTful API endpoints for third-party integration
- **AI Integration:** Product recommendations based on user behavior
- **Customer Support:** Live chat support and intelligent chatbot
- **Enhanced UX:** Wishlists and product reviews
- **Vendor Portal:** Supplier registration, dashboard with analytics


<div align="center">
<h1 align="center">🛒 Ez2Buy – E-Commerce Web App (ASP.NET Core MVC)</h1>
Ez2Buy is an eCommerce web application built with ASP.NET Core MVC, providing a modern and secure platform for managing products, orders, and customers online.
<br />
<br />
<a href="https://ez2buy.azurewebsites.net"><strong>➥ Live Website</strong></a>
<br />
<br />
</div>

1.1 Non- Functional Requirements:
•	Administration features
o	User Management: Assign roles and manage user accounts.
o	Order Management: View and update order statuses.
o	Product & Category Management
•	User Managamenet:
o	Product Catalog search and filter
o	Order Placement & Shopping Cart
o	Payment gateway integration
o	Email notification for confirmation

1.2 Functional Requirements:
•	Usability: Responsive design for device types
•	Security: role-based Access
•	Reliability & Availability: Comprehensive error handling
•	Maintainability: Clean code with Comprehensive documentation and Unit tests coverage 


Architectural Decisions:
•	layered architecture:
o	Presentation Layer: MVC Controllers and Views
o	Data Access Layer: UnitOfWork and Repository implementations
o	Data Layer: Entity Framework Core with SQL Server
•	Repository Pattern for data access abstraction
•	Unit of Work Pattern for transaction management
•	MVC Pattern for separation of concerns
•	ViewModel Pattern for UI data shaping
•	Using Session data to save data for shopping cart


Technology Stack Selection:
•	ASP.NET 8.0 Core MVC web framework
•	Entity Framework Core
•	ASP.NET Core Identity


Middleware:
1. app.UseHttpsRedirection() 
2. app.UseRouting() 
3. app.UseAuthentication() 
4. app.UseAuthorization() 
5. app.UseSession()


Next steps: Advanced User Experience
•	completing API versions
•	AI-powered product recommendations based on browsing and purchase history
•	Live chat support or chatbot for instant customer help
•	Voice search integration for accessibility and convenience
•	adding Wishlist's and Products Reviews
•	Enhanced Security & Trust: Fraud detection mechanisms
•	Supplier/vendor registration and approval workflow 
•	Supplier dashboard with analytics, product status, and earnings


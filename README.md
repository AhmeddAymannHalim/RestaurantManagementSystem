# Restaurant Management System

A full-stack restaurant management solution built with .NET 8 and Clean Architecture principles. This system handles everything from menu management to order processing with real-time tracking and multi-language support.

## Overview

This is a complete restaurant management platform designed to streamline daily operations in modern restaurants. The system allows staff to manage menu items, track table availability, process orders from start to finish, and monitor all activities through an intuitive web interface. With built-in support for both English and Arabic languages, it's suitable for restaurants operating in diverse regions.

## Architecture & Design

### Clean Architecture

The project follows Clean Architecture principles, organizing code into four distinct layers:

**Domain Layer** - Houses core business entities like User, Order, MenuItem, Table, and defines the fundamental business rules. This layer has no dependencies on other layers.

**Application Layer** - Contains business logic, data transfer objects (DTOs), service interfaces, and validation rules. It defines what the system can do without specifying how.

**Infrastructure Layer** - Implements data access through Entity Framework Core, handles caching with Redis, manages email notifications, and contains all external service integrations.

**API Layer** - Exposes RESTful endpoints, handles HTTP requests/responses, manages authentication/authorization, and serves the frontend application.

This layered approach means we can swap out databases, change UI frameworks, or modify external services without affecting core business logic.

### Design Patterns

**Repository Pattern** - All database operations are centralized through repository interfaces. Instead of writing SQL queries throughout the codebase, we have dedicated repositories for each entity. This makes testing easier (repositories can be mocked), keeps data access logic in one place, and allows us to switch databases if needed.

**Unit of Work Pattern** - Multiple database operations are grouped into single transactions. When creating an order, we need to insert the order record, create order items, and update table status - all atomically. If any step fails, everything rolls back automatically.

**Dependency Injection** - All services are registered in the DI container and injected where needed. This promotes loose coupling, makes the code more testable, and allows for easy configuration changes.

**DTO Pattern** - We separate internal domain models from API contracts. This prevents over-posting attacks, controls exactly what data gets exposed, optimizes bandwidth, and provides clear API documentation.

### FluentValidation

All incoming data is validated before reaching the database. We use FluentValidation to define readable, testable validation rules that can check multiple properties, perform async database lookups, and return clear error messages to users.

## Performance Optimization

### Caching Strategy

The system implements a smart caching layer using Redis. Frequently accessed data that changes infrequently gets cached to reduce database load:

- Menu categories cached for 30 minutes
- Available menu items cached for 10 minutes  
- Available tables cached for 1 minute
- System settings cached for 1 hour

This approach reduces database queries by approximately 80% for common operations. The first request fetches from the database, but subsequent requests get instant responses from cache until expiration.

We specifically avoid caching order status (changes too frequently) and user authentication data (security-sensitive).

### Database Indexing

Strategic indexes are placed on frequently queried columns. Order numbers have unique indexes for instant lookups. Composite indexes on order date and status enable fast filtering. User email and username are indexed for quick authentication checks.

Without indexes, finding a specific order requires scanning every database row. With proper indexing, the database jumps directly to the right record - roughly 10x faster for typical queries.

### Query Optimization

The system uses eager loading to prevent the N+1 query problem. When fetching an order, we also load its items, menu details, table info, and user data in a single database call using SQL JOINs. Without this optimization, loading one order with 10 items would require 20+ separate queries. With eager loading, it's just one query.

## Security Features

### Authentication

JSON Web Tokens (JWT) provide secure, stateless authentication. When users log in successfully, they receive a token valid for 24 hours. This token contains encrypted claims about the user's identity and role. Every API request includes this token, allowing the server to verify identity and check permissions without database calls.

### Password Protection

User passwords are hashed using BCrypt with 12 rounds of salting before storage. Even if someone gains access to the database, they cannot reverse these hashes back to plain text passwords. The high work factor (12 rounds) makes brute force attacks computationally expensive.

### Password Reset

The forgot password flow uses one-time passwords for security:
1. User requests password reset via email
2. System generates random 6-digit OTP
3. OTP is hashed and stored in database
4. Plain OTP is emailed to user
5. User has 15 minutes to use the OTP
6. After successful reset, OTP is marked as used

This approach is more secure than permanent reset links because OTPs are short-lived and single-use.

## Audit Logging

Every significant system action is automatically logged for compliance, security, and debugging purposes.

### What Gets Tracked

- **Who** performed the action (user ID)
- **What** they did (create, update, delete operations)
- **When** it happened (precise timestamp)
- **Where** it came from (IP address)
- **What changed** (before and after states in JSON format)
- **How** they accessed it (browser/app user agent)

### Why This Matters

**Compliance** - Many industries legally require audit trails. Healthcare (HIPAA), finance (SOX), and data protection (GDPR) regulations mandate tracking who accessed what data and when.

**Security** - Audit logs help detect suspicious activity patterns like multiple failed login attempts from one IP address, unauthorized access attempts, or unusual activity outside normal business hours.

**Debugging** - When something goes wrong, audit logs show exactly what happened. You can trace an order's complete history, see who changed a price, or identify when a table status was updated.

**Analytics** - Understanding user behavior helps improve the system. Which features get used most? When is peak activity? What operations take longest?

### Log Management

Audit logs grow quickly, so we handle them carefully:

**Indexed queries** - Logs are indexed by timestamp, user ID, action type, and entity name for fast searching even with millions of records.

**Automatic archiving** - Logs older than 90 days are automatically moved to cold storage or archive tables, keeping the active table small and fast.

**Async processing** - Logging happens asynchronously so it doesn't slow down API responses. Users don't wait for logs to be written.

## Database Design

The system uses 10 core tables to manage restaurant operations:

### Core Operational Tables

**Users** - Stores staff accounts including admins, waiters, and kitchen staff. Each user has credentials, role assignments, and activity status.

**Tables** - Tracks restaurant seating with table numbers, seating capacity, floor section location, and real-time availability status.

**Categories** - Organizes menu items into logical groups like Appetizers, Main Courses, Desserts, and Beverages. Supports custom ordering for display.

**MenuItems** - Contains all dishes and drinks with names (English and Arabic), descriptions, prices, preparation times, and availability flags.

**Orders** - Represents customer orders with unique order numbers, timestamps, status tracking, and calculated totals including tax.

**OrderItems** - Individual line items within orders, linking to menu items with quantities, unit prices, and special requests.

### Supporting Tables

**Settings** - Key-value store for system configuration like tax rates, business hours, and operational parameters.

**EmailSettings** - SMTP server configuration for automated email notifications (order confirmations, password resets).

**PasswordResetTokens** - Temporary OTP tokens for secure password recovery with expiration and usage tracking.

**AuditLogs** - Complete activity history capturing who did what, when, and from where for compliance and security.

### Data Relationships

The tables are connected through foreign keys:
- Each Order belongs to one Table and one User (waiter)
- Each OrderItem references one Order and one MenuItem
- Each MenuItem belongs to one Category
- Each AuditLog optionally references one User

This relational structure ensures data integrity and enables complex queries like "show all orders from table 5 today" or "list all items in the appetizers category that are currently available."

## Getting Started

### System Requirements

- .NET 8 SDK or later
- SQL Server 2019+ or Azure SQL Database
- Redis Server (optional but recommended for production)
- Modern web browser (Chrome, Firefox, Safari, Edge)

### Installation Steps

1. **Clone Repository** - Download or clone the project to your local machine

2. **Configure Database** - Open appsettings.json and update the connection string to point to your SQL Server instance

3. **Apply Migrations** - Navigate to the API project folder and run the Entity Framework migrations to create the database schema

4. **Configure Email (Optional)** - Update SMTP settings if you want password reset functionality

5. **Run Application** - Start the API project which also serves the frontend

6. **Access System** - Open browser to https://localhost:7054 and log in with default admin credentials

7. **Change Default Password** - Immediately update the admin password for security

### Initial Configuration

After first login, configure:
- Create user accounts for staff members
- Set up menu categories
- Add menu items with prices
- Define restaurant tables
- Configure email settings for notifications

## API Documentation

The system exposes a comprehensive REST API documented with Swagger/OpenAPI. Once running, visit the Swagger UI at https://localhost:7054/swagger for interactive API exploration.

### Authentication Flow

Users must authenticate to receive a JWT token, which is then included in subsequent requests via the Authorization header. Tokens expire after 24 hours for security.

### Key Endpoints

**Authentication** - Register users, login, request password reset, reset password with OTP

**Menu Management** - List categories, create/update/delete categories, manage menu items, toggle item availability

**Order Processing** - Create orders, update order status, view order history, cancel orders

**Table Management** - View all tables, check available tables, update table status

**Settings** - View and update system configuration

All endpoints return consistent JSON responses with success flags, data payloads, and error messages when applicable.

## Frontend Application

The web interface is a single-page application built with vanilla JavaScript - no framework dependencies. This keeps it lightweight, fast, and easy to customize.

### User Experience Features

**Smooth Animations** - CSS transitions with GPU acceleration create fluid 60fps animations. Content fades in/out smoothly when loading data, preventing jarring layout shifts.

**Responsive Design** - The interface adapts to different screen sizes, from mobile phones to desktop monitors. Touch-friendly controls work well on tablets.

**Loading States** - Clear visual feedback during API calls. Users see spinners and disabled buttons so they know the system is processing.

**Error Handling** - Friendly error messages appear as toast notifications. Network errors, validation failures, and server issues all get communicated clearly.

**Real-time Updates** - Order statuses update instantly. When a waiter marks an order as ready, the kitchen display refreshes automatically.

### Language Support

The interface supports both English and Arabic with complete internationalization:

**Dynamic Translation** - All text loads from language files. Switching languages updates every label, button, and message instantly without page reload.

**RTL Layout** - Arabic mode automatically switches to right-to-left layout. Menus, tables, and forms all mirror properly.

**Bilingual Data** - Menu items and categories can have both English and Arabic names. The interface displays the appropriate language based on user preference.

**Persistent Preference** - Language choice saves to browser storage and persists across sessions.

### State Management

User authentication state stores in browser localStorage. The JWT token and user profile persist across page refreshes but clear immediately on logout for security.

The application tracks which tab is active, what filters are applied, and which items are selected - all managed through simple JavaScript variables and DOM manipulation.

## Technology Stack

### Backend Technologies

**.NET 8** - Latest long-term support version of Microsoft's cross-platform framework. Provides high performance, modern C# language features, and excellent tooling.

**Entity Framework Core 8** - Object-relational mapper that translates LINQ queries into optimized SQL. Handles database migrations, change tracking, and relationship loading.

**SQL Server** - Enterprise-grade relational database with excellent performance, security features, and integration with .NET ecosystem.

**Redis** - In-memory data structure store used for caching. Provides microsecond latency for frequently accessed data.

**FluentValidation** - Popular validation library that separates validation logic from domain models. Supports complex rules, async validation, and custom error messages.

**BCrypt.Net** - Cryptographic library for secure password hashing with configurable work factors.

**JWT Bearer** - Industry-standard tokens for secure authentication without server-side session storage.

### Frontend Technologies

**Vanilla JavaScript** - Pure ES6+ JavaScript without framework overhead. Direct DOM manipulation for maximum control and minimal bundle size.

**CSS3** - Modern styling with flexbox, grid, transforms, and animations. CSS variables for consistent theming.

**HTML5** - Semantic markup with proper accessibility attributes and SEO-friendly structure.

## What Makes This Project Different

### Balanced Complexity

Most restaurant systems fall into two categories: oversimplified tutorials that skip important features, or enterprise behemoths so complex they're impossible to learn from.

This project aims for the middle ground - sophisticated enough to demonstrate professional practices, yet simple enough to understand completely. Every design decision has a practical justification.

### Production-Ready Patterns

The codebase uses patterns you'll find in real production systems:
- Clean Architecture for long-term maintainability
- Repository and Unit of Work for testable data access  
- Comprehensive validation at API boundaries
- Proper security with hashing and tokens
- Performance optimizations that actually matter

### Real-World Features

Beyond basic CRUD operations, the system includes features essential for production:
- Audit logging for compliance and debugging
- Caching strategy to handle load
- Multi-language support for global deployment
- Email notifications for user engagement
- Proper error handling throughout

### Educational Value

The code is intentionally readable and well-organized. Comments explain why, not just what. Patterns are consistent across the codebase. Variable and method names are descriptive.

Someone learning .NET can read this codebase and understand how pieces fit together. An experienced developer can use it as a reference for implementing similar patterns.


### Guidelines

- Follow existing code style and naming conventions
- Add validation for new endpoints
- Include tests for new business logic
- Update documentation for API changes
- Keep commits focused and descriptive

### Development Process

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request with clear description

## License

This project is released under the MIT License. You're free to use it for personal learning, commercial applications, or as a foundation for your own projects. Attribution appreciated but not required.

## Support

For questions, issues, or suggestions:
- Check the API documentation at /swagger endpoint
- Review existing GitHub issues
- Open a new issue with detailed description
- Include error messages and steps to reproduce

The project maintainers will respond as time permits. This is primarily an educational resource, so community support is appreciated.

---

**Built with .NET 8, Clean Architecture, and real-world production practices.**

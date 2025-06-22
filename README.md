# MicroServices.Minikube

This repository contains a collection of microservices that can be deployed on Minikube. The services are designed to demonstrate various aspects of microservices architecture, including service discovery, load balancing, and inter-service communication.

## Architecture

The solution consists of the following microservices:
- **Auth Service**: Handles authentication and JWT token management
- **Users Service**: Manages user data and orchestrates data from other services
- **Orders Service**: Provides order-related functionality
- **Products Service**: Manages product information
- **Web Application**: ASP.NET Core Razor Pages frontend

### Security
- JWT-based authentication using RSA key pairs
- Cookie-based authentication for the web frontend
- Token revocation support
- Secure token transmission using HttpOnly cookies

## Prerequisites

- .NET 8.0 SDK
- Minikube
- Docker
- kubectl

## Installation

1. Clone the repository:
	```cmd
    git clone https://github.com/kondwa/MicroServices.Minikube.git 
    cd MicroServices.Minikube
    ```
2. Generate RSA key pair:
   - Place `private.pem` in the Auth service directory
   - Place `public.pem` in the Users, Orders, and Products service directories

3. Configure JWT settings in `appsettings.json` for each service:
	```
    { "JwtSettings": { "Issuer": "your-issuer", "Audience": "your-audience", "ExpirationMinutes": 60 } }
    ```

## Services

### Auth Service (Internal URL: `http://auth-api`)
Handles user authentication and token management.
- `/login` - Authenticates users and issues JWT tokens
- `/logout` - Revokes active tokens

### Users Service (Internal URL: `http://users-api`)
Aggregates user-related data from multiple services.
- `/users` - Returns user information with related orders and products
- `/orders` - Retrieves user orders
- `/products` - Retrieves available products

### Orders Service (Internal URL: `http://orders-api`)
Manages order data.
- `/orders` - Returns list of orders

### Products Service (Internal URL: `http://products-api`)
Manages product data.
- `/products` - Returns list of products

### Web Application (Internal URL: `http://web-mvc`)
Razor Pages frontend providing user interface for:
- User authentication
- Viewing user data
- Managing orders
- Browsing products

## Development

Each service can be run independently using:
```cmd
dotnet run --project ServiceName
```

## Docker Support

Build images:
```cmd
.\build.bat
```

## Minikube Deployment

1. Start Minikube:
   ```cmd
   minikube start
   ```
2. Enable Minikube Addons:
   ```cmd
   minikube addons enable dns ingress ingress-dns registry
   ```
3. Apply Kubernetes configurations:
   ```cmd
   kubectl apply -f k8s/
   ```
4. Add to `/etc/hosts` or `c:\Windows\System32\drivers\etc\hosts` file:
   ```
   127.0.0.1 auth.test
   127.0.0.1 users.test
   127.0.0.1 orders.test
   127.0.0.1 products.test
   127.0.0.1 web.test
   ```
5. Start minikube tunnel:
   ```cmd
   minikube tunnel
   ```
6. Access the web app:
   ```
   http://web.test
   ```
   
## Authentication Flow

1. User logs in via Web Application
2. Auth Service validates credentials and issues JWT token
3. Token is stored in HttpOnly cookie
4. Subsequent requests include token automatically
5. Microservices validate tokens using public key

## API Documentation

Each service includes Swagger UI for API documentation and testing:
- Auth Service: `http://localhost:5001/swagger`
- Users Service: `http://localhost:5002/swagger`
- Orders Service: `http://localhost:5003/swagger`
- Products Service: `http://localhost:5004/swagger`

## License

This project is licensed under the Boost Software License 1.0 - see the LICENSE file for details.
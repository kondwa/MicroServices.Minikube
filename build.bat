FOR /f "tokens=*" %%i IN ('minikube -p minikube docker-env --shell cmd') DO @%%i

docker build -t users-api:latest Users
docker build -t orders-api:latest Orders
docker build -t products-api:latest Products
docker build -t auth-api:latest Auth
docker build -t web-mvc:latest Web

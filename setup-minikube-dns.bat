@echo off
REM Get the Minikube IP and store it in a variable
FOR /F "tokens=*" %%i IN ('minikube ip') DO SET MINIKUBE_IP=%%i

echo Minikube IP detected: %MINIKUBE_IP%

REM Add static route to Cluster DNS (10.96.0.10) via Minikube IP
REM This requires admin privileges!
route ADD 10.96.0.10 MASK 255.255.255.255 %MINIKUBE_IP%

echo Route added: 10.96.0.10 -> %MINIKUBE_IP%
pause

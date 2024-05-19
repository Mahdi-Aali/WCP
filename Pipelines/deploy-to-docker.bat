@echo off

cd ../

git pull

cd /Api

dotnet restore

dotnet build

if exist dist rmdir /s /q dist

dotnet publish -o dist -f net8.0 -c release

docker rmi web-configuration-provider

docker build . -t web-configuration-provider

docker stop web-configuration-provider

docker rm web-configuration-provider

docker run -p 7000:8080 -p 7001:8081 --name web-configuration-provider -d -it --mount source=wcp-volume,target=/app/Projects web-configuration-provider

pause
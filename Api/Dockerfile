FROM mcr.microsoft.com/dotnet/aspnet:8.0
USER app
WORKDIR /app
COPY /dist /app
EXPOSE 80/tcp
EXPOSE 443/tcp

ENTRYPOINT ["dotnet", "Api.dll"]
# Dockerfile для timetable_service
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Debug -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=builder /app/out .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8083
CMD ["dotnet", "timetable_service.dll"]
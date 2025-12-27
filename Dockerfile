# =====================
# Build stage
# =====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore
COPY Assessment/Assessment.csproj Assessment/
RUN dotnet restore Assessment/Assessment.csproj

# Copy all source code
COPY . .

# Publish
RUN dotnet publish Assessment/Assessment.csproj -c Release -o /app/publish

# =====================
# Runtime stage
# =====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published output only
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "Assessment.dll"]

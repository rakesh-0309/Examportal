# Use SDK image to build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY Assessment.sln ./
COPY Assessment/Assessment.csproj Assessment/

# Restore dependencies
RUN dotnet restore Assessment/Assessment.csproj

# Copy the rest of your code
COPY . .

# Build and publish
RUN dotnet publish Assessment/Assessment.csproj -c Release -o /app/publish

# Use runtime image for final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /src
COPY . .
WORKDIR /src/Assessment
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish


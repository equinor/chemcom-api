# Use .NET 8 SDK based on Alpine for the build environment
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

# Install additional packages needed for compatibility and globalization
RUN apk add --no-cache libc6-compat icu

# Copy the project file(s) and restore dependencies as distinct layers
COPY ChemDec.Api/*.csproj ./ChemDec.Api/
RUN dotnet restore ChemDec.Api/ChemDec.Api.csproj

# Copy the rest of the application files and build the application
COPY . ./
RUN dotnet publish ChemDec.Api -c Release -o ChemDec.Api/out

# Build runtime image using .NET 8 ASP.NET based on Alpine
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

WORKDIR /app

# Set environment variables for globalization and timezone
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV TZ="Europe/Oslo"

# Install necessary packages for ICU and timezone data
RUN apk -U add --no-cache icu tzdata

# Copy the published output from the build stage
COPY --from=build /app/ChemDec.Api/out .

# Create a non-root user and group with numeric UID/GID for security
RUN addgroup -g 1001 non-root-group && \
    adduser -u 1001 -G non-root-group -s /bin/sh -D non-root-user

# Switch to the non-root user for better security practices
USER 1001:1001

# Expose the port on which the application will run
EXPOSE 5000

# Define the entry point for the application
ENTRYPOINT ["dotnet", "ChemDec.Api.dll"]

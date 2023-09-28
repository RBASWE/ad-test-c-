# Use the appropriate .NET SDK image as the build environment
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

# Copy the .csproj file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application code
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the ASP.NET Core runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# Set the environment variables for Windows Authentication
# ENV ASPNETCORE_URLS=http://+:80
# ENV ASPNETCORE_ENVIRONMENT=Production

# RUN echo "Europe/Vienna" > /etc/timezone
# RUN dpkg-reconfigure -f noninteractive tzdata

# Expose the port for the application
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "ad.api.dll"]


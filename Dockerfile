# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Project files and NuGet
COPY ["CONVERTinator.WebAPI/CONVERTinator.WebAPI.csproj", "CONVERTinator.WebAPI/"]
COPY ["CONVERTinator/CONVERTinator.csproj", "CONVERTinator/"]
RUN dotnet restore "CONVERTinator.WebAPI/CONVERTinator.WebAPI.csproj"
COPY . .
WORKDIR "/src/CONVERTinator.WebAPI"

# Compile /app/publish
RUN dotnet publish "CONVERTinator.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Prodaction
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set 8080 port
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "CONVERTinator.WebAPI.dll"]
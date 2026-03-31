FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Wrap.FilmProductionManagementSystem.Web/Wrap.FilmProductionManagementSystem.Web.csproj", "Wrap.FilmProductionManagementSystem.Web/"]
COPY ["Wrap.GCommon/Wrap.GCommon.csproj", "Wrap.GCommon/"]
COPY ["Wrap.Services.Models/Wrap.Services.Models.csproj", "Wrap.Services.Models/"]
COPY ["Wrap.Web.Infrastructure/Wrap.Web.Infrastructure.csproj", "Wrap.Web.Infrastructure/"]
COPY ["Wrap.Services.Core/Wrap.Services.Core.csproj", "Wrap.Services.Core/"]
COPY ["Wrap.Data.Models/Wrap.Data.Models.csproj", "Wrap.Data.Models/"]
COPY ["Wrap.Data/Wrap.Data.csproj", "Wrap.Data/"]
COPY ["Wrap.Web.ViewModels/Wrap.Web.ViewModels.csproj", "Wrap.Web.ViewModels/"]
RUN dotnet restore "Wrap.FilmProductionManagementSystem.Web/Wrap.FilmProductionManagementSystem.Web.csproj"
COPY . .
WORKDIR "/src/Wrap.FilmProductionManagementSystem.Web"
RUN dotnet build "./Wrap.FilmProductionManagementSystem.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Wrap.FilmProductionManagementSystem.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wrap.FilmProductionManagementSystem.Web.dll"]

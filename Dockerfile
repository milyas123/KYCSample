FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["KYC_AzaKaw_WebApp/KYC_AzaKaw_WebApp.csproj", "KYC_AzaKaw_WebApp/"]
RUN dotnet restore "KYC_AzaKaw_WebApp/KYC_AzaKaw_WebApp.csproj"
COPY . .
WORKDIR "/src/KYC_AzaKaw_WebApp"
RUN dotnet build "KYC_AzaKaw_WebApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "KYC_AzaKaw_WebApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "KYC_AzaKaw_WebApp.dll"]
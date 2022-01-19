FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["RegistrationSample/RegistrationSample.csproj", "RegistrationSample/"]
RUN dotnet restore "RegistrationSample/RegistrationSample.csproj"
COPY . .
WORKDIR "/src/RegistrationSample"
RUN dotnet build "RegistrationSample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RegistrationSample.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RegistrationSample.dll"]

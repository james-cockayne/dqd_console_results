FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY dqd_console_results.sln .
COPY src/DqdConsoleResults/DqdConsoleResults.csproj src/DqdConsoleResults/
COPY tests/DqdConsoleResults.Tests/DqdConsoleResults.Tests.csproj tests/DqdConsoleResults.Tests/
RUN dotnet restore
COPY . .
RUN dotnet publish src/DqdConsoleResults/DqdConsoleResults.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "DqdConsoleResults.dll"]

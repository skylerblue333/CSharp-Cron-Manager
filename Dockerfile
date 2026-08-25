FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY CSharp-Cron-Manager.csproj ./
RUN dotnet restore CSharp-Cron-Manager.csproj
COPY Program.cs Schedule.cs ./
RUN dotnet publish CSharp-Cron-Manager.csproj -c Release --no-restore -o /out /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build --chown=app:app /out ./
USER app
ENTRYPOINT ["dotnet", "SkySchedule.dll"]

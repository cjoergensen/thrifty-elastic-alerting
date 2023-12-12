FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /source

COPY . .
RUN dotnet publish ./Worker/Worker.csproj -c Release -a $TARGETARCH -o /app /p:DebugType=None /p:DebugSymbols=false

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["./ThriftyElasticAlertingWorker"]
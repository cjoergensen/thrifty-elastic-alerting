FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG TARGETARCH
WORKDIR /source

COPY . .
RUN dotnet publish ./src/Worker/Worker.csproj -a $TARGETARCH -o /app --self-contained /p:PublishTrimmed=false /p:PublishReadyToRun=true /p:PublishSingleFile=true

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:8.0-jammy-chiseled
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
ENTRYPOINT ["./ThriftyElasticAlerting.Worker"]
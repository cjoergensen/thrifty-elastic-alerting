FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
COPY ["/app", "/app"]
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-libs krb5-libs libgcc libintl libssl1.1 libstdc++ zlib tzdata

ENV APP_UID=10000
ENV APP_NAME=appuser

RUN adduser -u $APP_UID --disabled-password $APP_NAME
USER $APP_UID
CMD ["./Kibana.Alerts"]
# ENTRYPOINT ["dotnet", "Kibana.Alerts.dll"] 
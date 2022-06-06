#FROM microsoft/dotnet:1.1.1-sdk
FROM mcr.microsoft.com/dotnet/core/sdk:1.1

COPY . /service

WORKDIR /service

RUN chmod +x ./docker_init.sh

EXPOSE 40500/tcp

CMD ["./docker_init.sh"]

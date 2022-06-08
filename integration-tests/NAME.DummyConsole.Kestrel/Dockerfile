FROM mcr.microsoft.com/dotnet/core/sdk:2.2

COPY . /service

WORKDIR /service

RUN chmod +x ./docker_init.sh

EXPOSE 40500/tcp

CMD ["./docker_init.sh"]

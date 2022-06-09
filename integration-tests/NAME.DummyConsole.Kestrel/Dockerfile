FROM mcr.microsoft.com/dotnet/core/sdk:2.2

COPY . /service

WORKDIR /service

RUN sed -i -e 's/\r$//' ./docker_init.sh \
    && chmod +x ./docker_init.sh

EXPOSE 40500/tcp

CMD ["./docker_init.sh"]

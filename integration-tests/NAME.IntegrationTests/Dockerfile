FROM mcr.microsoft.com/dotnet/core/sdk:2.2
WORKDIR /integration
COPY . ./
# COPY ../../Output/Artifacts/NuGets/Release ./nugets

RUN sed -i -e 's/\r$//' ./docker_init.sh \
    && chmod +x ./docker_init.sh \
    && sed -i -e 's/\r$//' ./multi-wait-for-it.sh \
    && chmod +x ./multi-wait-for-it.sh
ENV OPERATING_SYSTEM=debian

CMD ["./docker_init.sh"]

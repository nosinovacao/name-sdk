FROM mcr.microsoft.com/dotnet/core/sdk:2.2
WORKDIR /integration
COPY . ./
# COPY ../../Output/Artifacts/NuGets/Release ./nugets

RUN chmod +x ./docker_init.sh
RUN chmod +x ./multi-wait-for-it.sh
ENV OPERATING_SYSTEM=debian

CMD ["./docker_init.sh"]

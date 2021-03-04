FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build-base
ARG runtime=alpine.3.12
ENV RUNTIME_ID=${runtime}

FROM build-base AS build-base-arm
ENV RUNTIME_ID=${RUNTIME_ID}-arm

FROM build-base AS build-base-arm64
ENV RUNTIME_ID=${RUNTIME_ID}-arm64

FROM build-base AS build-base-amd64
ENV RUNTIME_ID=${RUNTIME_ID}-x64

FROM --platform=$BUILDPLATFORM build-base-${TARGETARCH} as build-env
ARG build_id=0
ARG commit_id=dirty
ARG trim=true
WORKDIR /app

COPY FreeModBot.sln .
COPY ./src/ ./src

RUN dotnet publish \
    --configuration=Release \
    --output=out \
    --runtime=${RUNTIME_ID} \
    --version-suffix=ci \
    -p:BuildId=${build_id} \
    -p:SourceRevisionId=${commit_id} \
    -p:PublishTrimmed=${trim} \
    src/FreeModBot

FROM alpine
RUN apk add --no-cache \
        ca-certificates \
        \
        # .NET Core dependencies
        krb5-libs \
        libgcc \
        libintl \
        libssl1.1 \
        libstdc++ \
        zlib \
        icu-libs

ENV \
    # Configure web servers to bind to port 80 when present
    #ASPNETCORE_URLS=http://+:80 \
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build-env /app/out /app
WORKDIR /app
ENTRYPOINT [ "/app/FreeModBot" ]

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim
ENV DEBIAN_FRONTEND=noninteractive

RUN apt-get update && apt-get -y install --no-install-recommends apt-utils dialog 2>&1 && \
    apt-get -y install openssh-client less iproute2 apt-transport-https gnupg2 curl lsb-release\
    git procps python3-pip redis-tools ca-certificates vim nano groff zip file jq wget

# Add Microsoft's public GPG key
RUN wget -qO- https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.asc.gpg
RUN mv microsoft.asc.gpg /etc/apt/trusted.gpg.d/
RUN wget -q https://packages.microsoft.com/config/debian/10/prod.list
RUN mv prod.list /etc/apt/sources.list.d/microsoft-prod.list
RUN apt-get update && apt-get -y install aspnetcore-runtime-3.1


# Install support for docker-in-docker
RUN apt-get install -y apt-transport-https ca-certificates curl gnupg-agent software-properties-common lsb-release \
    && curl -fsSL https://download.docker.com/linux/$(lsb_release -is | tr '[:upper:]' '[:lower:]')/gpg | apt-key add - 2>/dev/null \
    && add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/$(lsb_release -is | tr '[:upper:]' '[:lower:]') $(lsb_release -cs) stable" \
    && apt-get update \
    && apt-get install -y docker-ce-cli \
    #
    # Install Docker Compose
    && curl -sSL "https://github.com/docker/compose/releases/download/1.28.2/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose \
    && chmod +x /usr/local/bin/docker-compose

# Cleanup APT
RUN apt-get autoremove -y \
    && apt-get clean -y \
    && rm -rf /var/lib/apt/lists/*

# This is needed for Global Tools to work
ENV PATH="${PATH}:/root/.dotnet/tools"
# Install .NET Global Tools
RUN dotnet tool install -g dotnet-format \
    && dotnet tool install -g dotnet-reportgenerator-globaltool \
    && dotnet tool install -g dotnet-counters \
    && dotnet tool install -g dotnet-search

ENV DEBIAN_FRONTEND=dialog

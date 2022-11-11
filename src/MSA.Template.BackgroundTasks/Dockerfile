FROM mcr.microsoft.com/dotnet/aspnet:6.0.11-bullseye-slim-arm64v8 AS base
RUN groupadd -g 999 appuser && \
    useradd -r -u 999 -g appuser appuser
USER appuser
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0.403-bullseye-slim-arm64v8 AS build
WORKDIR /src
COPY ["src/MSA.Template.BackgroundTasks/MSA.Template.BackgroundTasks.csproj", "MSA.Template.BackgroundTasks/"]
RUN dotnet restore "src/MSA.Template.BackgroundTasks/MSA.Template.BackgroundTasks.csproj"
COPY . .
WORKDIR /src/MSA.Template.BackgroundTasks
RUN dotnet build "MSA.Template.BackgroundTasks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MSA.Template.BackgroundTasks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSA.Template.BackgroundTasks.dll"]

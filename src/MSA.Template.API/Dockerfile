FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
RUN groupadd -g 999 appuser && \
    useradd -r -u 999 -g appuser appuser
USER appuser
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "src/MSA.Template.API/MSA.Template.API.csproj" && \
    dotnet build "src/MSA.Template.API/MSA.Template.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/MSA.Template.API/MSA.Template.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSA.Template.API.dll"]

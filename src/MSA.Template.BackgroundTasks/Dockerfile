FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MSA.Template.BackgroundTasks/MSA.Template.BackgroundTasks.csproj", "MSA.Template.BackgroundTasks/"]
RUN dotnet restore "src/MSA.Template.BackgroundTasks/MSA.Template.BackgroundTasks.csproj"
COPY . .
WORKDIR "/src/MSA.Template.BackgroundTasks"
RUN dotnet build "MSA.Template.BackgroundTasks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MSA.Template.BackgroundTasks.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MSA.Template.BackgroundTasks.dll"]

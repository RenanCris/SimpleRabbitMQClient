FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY SimpleRabbitMQ.Validation/SimpleRabbitMQ.Validation.csproj ./
RUN dotnet restore "SimpleRabbitMQ.Validation.csproj"
COPY SimpleRabbitMQ.Validation .
WORKDIR "/src/."
RUN dotnet build "SimpleRabbitMQ.Validation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleRabbitMQ.Validation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleRabbitMQ.Validation.dll"]
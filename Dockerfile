FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ECommerce.OrderProcessing.sln .

COPY ECommerce.OrderProcessing.Api/ECommerce.OrderProcessing.Api.csproj ECommerce.OrderProcessing.Api/
COPY ECommerce.OrderProcessing.Application/ECommerce.OrderProcessing.Application.csproj ECommerce.OrderProcessing.Application/
COPY ECommerce.OrderProcessing.Domain/ECommerce.OrderProcessing.Domain.csproj ECommerce.OrderProcessing.Domain/
COPY ECommerce.OrderProcessing.Infrastructure/ECommerce.OrderProcessing.Infrastructure.csproj ECommerce.OrderProcessing.Infrastructure/
COPY ECommerce.OrderProcessing.Application.Tests/ECommerce.OrderProcessing.Application.Tests.csproj ECommerce.OrderProcessing.Application.Tests/

RUN dotnet restore ECommerce.OrderProcessing.sln

COPY . .

WORKDIR /app/ECommerce.OrderProcessing.Api
RUN dotnet publish -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "ECommerce.OrderProcessing.Api.dll"]
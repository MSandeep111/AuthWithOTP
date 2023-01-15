#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PhoneAuthDemo/PhoneAuthDemo.csproj", "PhoneAuthDemo/"]
RUN dotnet restore "PhoneAuthDemo/PhoneAuthDemo.csproj"
COPY . .
WORKDIR "/src/PhoneAuthDemo"
RUN dotnet build "PhoneAuthDemo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PhoneAuthDemo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PhoneAuthDemo.dll"]
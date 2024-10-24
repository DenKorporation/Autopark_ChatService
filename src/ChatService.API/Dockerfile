FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ChatService.API/ChatService.API.csproj", "src/ChatService.API/"]
COPY ["src/ChatService.BLL/ChatService.BLL.csproj", "src/ChatService.BLL/"]
COPY ["src/ChatService.DAL/ChatService.DAL.csproj", "src/ChatService.DAL/"]
RUN dotnet restore "src/ChatService.API/ChatService.API.csproj"
COPY . .
WORKDIR "/src/src/ChatService.API"
RUN dotnet build "ChatService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ChatService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatService.API.dll"]

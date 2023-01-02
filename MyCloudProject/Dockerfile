#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app
COPY ["MyCloudProject/ffmpegexecutable/", "ffmpegexecutable"]

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MyCloudProject/MyCloudProject.csproj", "MyCloudProject/"]
COPY ["MyCloudProject.Common/MyCloudProject.Common.csproj", "MyCloudProject.Common/"]
COPY ["MyExperiment/MyExperiment.csproj", "MyExperiment/"]
RUN dotnet restore "MyCloudProject/MyCloudProject.csproj"
COPY . .
WORKDIR "/src/MyCloudProject"
RUN dotnet build "MyCloudProject.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyCloudProject.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN chmod -R 777 /app
RUN chmod -R 777 /app/ffmpegexecutable
RUN chmod -R 777 /app/ffmpegexecutable/ffprobe.exe
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyCloudProject.dll"]
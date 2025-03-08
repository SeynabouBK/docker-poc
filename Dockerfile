# Étape 1 : Construire l'application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copier le fichier .csproj AVANT le reste du projet
COPY ["MonApi/MonApi.csproj", "./"]

# Restaurer les dépendances
RUN dotnet restore

# Copier tous les fichiers de l'application
#COPY . ./
COPY MonApi/. ./

# Compiler l’application en mode Release
RUN dotnet publish -c Release -o /out

# Étape 2 : Exécuter l'application
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out ./

# Exposer le port 8080
EXPOSE 8080

# Démarrer l'application
CMD ["dotnet", "MonApi.dll"]




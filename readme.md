# Projet POC - API REST avec Docker, PostgreSQL et Nginx

## Description
Ce projet est une **Proof of Concept (POC)** qui montre comment créer une API REST avec :
-  **Backend** : ASP.NET Core (C#)
-  **Base de données** : PostgreSQL
-  **Reverse Proxy** : Nginx
-  **CI/CD** : Pipeline GitHub Actions
-  **Docker** : Conteneurs Docker pour le déploiement et l'orchestration


##  Fonctionnalités principales
 Création automatique de la base de données PostgreSQL  
 Création automatique de la table `users`  
 Ajout automatique d'utilisateurs par défaut  
 Envoi d'un message automatique sur Discord via Webhook  
 Reverse Proxy avec Nginx pour rediriger les requêtes  

---

## Configuration Initiale**
### **Prérequis :**
- Docker installé  
- Git installé  
- Port **5432** (PostgreSQL), **8080** (API) et **80** (Nginx) disponibles  


## DOCKER HUB
- Télécharger les images depuis  docker hub => docker-compose pull
- installer les conteneurs, volumes, reseau... => docker-compose up -d
- URL => http://localhost:8080/api/users
- tu peux te connecter à postgres => docker exec -it db-container bash puis taper psql -U postgres -d monapi (verifier les utilisateurs => SELECT * FROM users;)






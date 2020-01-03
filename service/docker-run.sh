#!/usr/bin/env bash
# docker run -p 8010:8002 --env ASPNETCORE_ENVIRONMENT=Docker --name auth-service --network auth-db_authService auth-service
docker-compose -f "docker-compose.yml" up -d --build
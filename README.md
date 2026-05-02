# Запуск
Из корневой директории DSA_kursach (где README.md):
`dotnet run --project AirlineTickets/AirlineTicketing.csproj`

# Запуск в Docker
Из корневой директории DSA_kursach:

```bash
docker build -t airline-tickets .
docker run -it --rm airline-tickets
```

Удаление образа:
`docker image rm airline-tickets`

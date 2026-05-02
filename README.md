# Запуск (требуется установленный dotnet 10*)
*Можно попробовать в `AirlineTickets/AirlineTicketing.csproj` поменять
на линии 5 `<TargetFramework>net10.0</TargetFramework>` на установленную у вас версию,
возможно, запустится.

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

Если долго скачивается, попробуйте отключить ВПН.

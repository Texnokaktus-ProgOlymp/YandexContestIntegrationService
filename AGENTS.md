# Инструкции для агентов

## Назначение и навигация

- Репозиторий содержит ASP.NET Core gRPC-сервис на .NET 10 для интеграции ProgOlymp с Яндекс Контестом. Описание запуска и конфигурации находится в `README.md`.
- Основной проект — `src/Texnokaktus.ProgOlymp.YandexContestIntegrationService/`. Регистрация зависимостей, middleware и gRPC-сервисов находится в `Program.cs`.
- Реализации RPC находятся в `Services/Grpc/`, получение OAuth-токена для исходящих запросов — в `Authentication/TokenProvider.cs`, общие преобразования моделей — в `Extensions/CommonMapping.cs`. Преобразования, нужные одному сервису, также встречаются в его file-local классах.
- Контракты находятся в git submodule `modules/Common.Contracts`, в каталогах `proto/YandexContest/` и `proto/Common/`. Приложение подключает необходимые `.proto` и `Texnokaktus.ProgOlymp.Common.Contracts/Grpc/Common/DecimalValue.cs` напрямую в `.csproj` и генерирует серверные gRPC-типы. Это отдельный репозиторий; при изменении контрактов учитывай состояние submodule и изменение его commit pointer в основном репозитории.

## Структура и зависимости

- Сохраняй формат solution `.slnx`. Production-проекты размещай в `src/`, тестовые проекты — в `tests/`; сейчас тестовых проектов нет.
- Общие свойства сборки задаются в корневом `Directory.Build.props`. Если добавляешь вложенный props, явно импортируй родительский файл, чтобы сохранить наследование.
- Версии NuGet-пакетов приложения задаются в корневом `Directory.Packages.props`, ссылки `PackageReference` в `.csproj` указываются без версии. Приложение не ссылается на `.csproj` submodule.
- Источники пакетов и Package Source Mapping определены в корневом `NuGet.Config`. При добавлении пакетов из GitHub Packages проверь соответствующий шаблон, включая транзитивные зависимости; учётные данные в репозиторий не добавляй.
- При переносе проектов обновляй пути в `.slnx`, `ProjectReference`, связанных Content-файлах, Dockerfile и `.github/workflows/dotnet.yml`. Docker build context — корень репозитория; props и проекты должны копироваться до `dotnet restore`.
- Поддерживай Solution Items для общих файлов и документации. Пути в solution должны вести к существующим файлам.

## Команды и проверки

Команды выполняются из корня репозитория:

```powershell
git submodule update --init --recursive
dotnet restore Texnokaktus.ProgOlymp.YandexContestIntegrationService.slnx
dotnet build Texnokaktus.ProgOlymp.YandexContestIntegrationService.slnx --no-restore
```

- Для restore требуется доступ к GitHub Packages организации `Texnokaktus-ProgOlymp`; учётные данные источника `github` задаются вне репозитория, например через `NuGetPackageSourceCredentials_github`.
- После изменения иерархии props проверяй эффективные свойства проекта через `dotnet msbuild <project.csproj> -getProperty:TargetFramework,Nullable,ImplicitUsings,TreatWarningsAsErrors`.
- При появлении тестовых проектов запускай тесты, относящиеся к изменению. Для правок только документации проверь ссылки, пути и соответствие примеров текущему коду.
- Если сборка прервалась с MSB5021 без ошибок компиляции, повтори её с `-m:1`: такой случай зафиксирован в `.agent/friction.md`.

## Особенности реализации

- Логирование и телеметрия подключаются через `Texnokaktus.ProgOlymp.Platform` и `builder.UsePlatform()`. Пакет требует переменную окружения `SERVICE_NAME`; она задана в профиле запуска и Dockerfile. Дополнительные источники трассировки настраивай через `ConfigureOpenTelemetry`, сохраняя источник Kiota.
- RPC-контракты и модели API Яндекс Контеста различаются. При изменении преобразований сверяй nullable-поля, enum и единицы времени с исходными моделями и `.proto`.
- Передавай `ServerCallContext.CancellationToken` в поддерживающие отмену внешние вызовы и потоковые операции.
- Сервис использует общий OAuth-токен под ключом `DEFAULT`. Redis нужен при старте, а также хранит токены и ключи Data Protection. Автоматическая генерация ключей отключена в `Program.cs`.
- `DownloadSubmissionSource` загружает исходник в S3 bucket `submissions` и возвращает ключ объекта. Bucket задан в коде; сохраняй согласованность реализации и документации при изменении этого поведения.
- При изменении RPC, конфигурации, портов или порядка запуска обновляй `README.md`. Секреты и рабочие токены передавай через окружение или внешнюю конфигурацию.
- Записывай значимые затруднения среды, инструментов и архитектуры в `.agent/friction.md`, соблюдая формат существующего журнала.

## Git

- Пиши сообщения всех Git-коммитов на английском языке, включая заголовок и тело сообщения, независимо от языка общения и документации проекта. Сообщения делай краткими. Избегай многострочных конструкций. Сообщения начинай со строчной буквы.

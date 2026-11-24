# EventFlow

**EventFlow** — система управления мероприятиями, разработанная в рамках курса по C#. Организаторы публикуют мероприятия, пользователи находят их, регистрируются и покупают билеты.

> ⚠️ Проект в активной разработке.

---

## ✨ Функционал

### 🔐 Аутентификация
- Регистрация, авторизация (JWT), Refresh-токены, подтверждение email

### 🎉 Мероприятия
- Создание, получение, обновление, загрузка изображений

### 🎟️ Регистрации
- Регистрация на мероприятие, отмена, список регистраций пользователя, список регистраций на мероприятие

### 💳 Оплата
- Инициирование оплаты, подтверждение через Wolverine + RabbitMQ (асинхронно), статусы оплаты

### 👤 Профиль
- Получение и обновление профиля

### 👥 Роли
**User** · **Organizer** · **Admin**

---

## 🏗️ Стек и архитектура

- **.NET 10 / ASP.NET Core Web API**
- **Clean Architecture**, **CQRS**, **MediatR**, **FluentValidation**
- **PostgreSQL** (Code-First), **EF Core**, **Microsoft Identity**
- **JWT** (авторизация + Refresh)
- **Wolverine + RabbitMQ** — асинхронная обработка платежей
- **Serilog** — структурированное логирование
- **xUnit** — тесты

---

## 📂 Структура

```
EventFlow/
├── EventFlow.Domain       # сущности, enum'ы
├── EventFlow.Application  # CQRS handlers, DTOs, interfaces
├── EventFlow.Infrastructure  # EF Core, миграции, репозитории
├── EventFlow.API          # запускаемый проект
└── EventFlow.Tests        # unit-тесты
```

---

## 🚀 Запуск

**Требования:** .NET 10 SDK, **PostgreSQL**, **RabbitMQ** (в Docker).

> ⚠️ Без запущенного RabbitMQ приложение не начнёт работу — Wolverine выбросит ошибку при попытке подключения к AMQP.

### 1. RabbitMQ в Docker

Запустите RabbitMQ с management-плагином:

```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=admin -e RABBITMQ_DEFAULT_PASS=admin123 rabbitmq:3.13-management-alpine
```

Панель управления будет доступна по адресу **http://localhost:15672** (логин: `admin`, пароль: `admin123`).

> ⚠️ Очередь `payment-processing` создаётся автоматически при старте приложения, но если RabbitMQ недоступен — приложение упадёт. Убедитесь, что контейнер запущен до начала `dotnet run`.

### 2. Клонировать
```bash
git clone https://github.com/Goblin-pskv/C--2026-05-team-3.git
cd EventFlow
```

### 3. User Secrets
Секреты хранятся в User Secrets проекта `EventFlow.API`. Добавьте свои значения:

```bash
cd EventFlow.API

dotnet user-secrets set "JwtSettings:SecretKey" "<your-secret-key>"
dotnet user-secrets set "ConnectionStrings:LogsConnection" "<your-logs-connection-string>"
dotnet user-secrets set "ConnectionStrings:LocalPostgres" "<your-postgres-connection-string>"
```

Пример строк подключения к локальному PostgreSQL:
```
Host=localhost;Port=5432;Database=EventFlow;Username=postgres;Password=<password>
```
> 📝 `LogsConnection` используется Serilog для записи логов в отдельную БД PostgreSQL (`EventFlowLog`).

### 4. Миграции
DbContext и миграции находятся в `EventFlow.Infrastructure`, поэтому указываем оба проекта:

```bash
dotnet ef database update --project EventFlow.Infrastructure --startup-project EventFlow.API
```

### 5. Запуск
```bash
cd EventFlow.API
dotnet run
```

Swagger доступен по адресу **https://localhost:{port}/swagger** (или http при отсутствии HTTPS).

### Тесты
```bash
dotnet test
```

---

## 🛣️ Планы

- [x] Платёжная система
- [ ] Расширение функционала админа
- [ ] Интеграционные тесты

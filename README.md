# EventFlow

**EventFlow** — система управления мероприятиями, разработанная в рамках курса по C#. Организаторы публикуют мероприятия, пользователи находят их, регистрируются и покупают билеты.

> ⚠️ Проект в активной разработке.

---

## ✨ Функционал

### 🔐 Аутентификация
- Регистрация, авторизация (JWT), Refresh-токены, подтверждение email

### 🎉 Мероприятия
- Создание, получение, обновление

### 🎟️ Регистрации
- Регистрация на мероприятие, отмена, список регистраций пользователя, список регистраций на мероприятие

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
- **Serilog** — структурированное логирование
- **xUnit** — тесты

**Внутренние сервисы:** `JwtService`, `EmailService`, `EmailConfirmTokenService`, `ValidationService`

---

## 📂 Структура

```
EventFlow/
├── EventFlow.Domain
├── EventFlow.Application
├── EventFlow.Infrastructure   // EF Core, миграции
├── EventFlow.API              // запускаемый проект
└── EventFlow.Tests
```

---

## 🚀 Запуск

**Требования:** .NET 10 SDK, PostgreSQL.

### 1. Клонировать
```bash
git clone https://github.com/Goblin-pskv/C--2026-05-team-3.git
cd EventFlow
```

### 2. User Secrets
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

### 3. Миграции
DbContext и миграции находятся в `EventFlow.Infrastructure`, поэтому указываем оба проекта:

```bash
dotnet ef database update --project EventFlow.Infrastructure --startup-project EventFlow.API
```

### Тесты
```bash
dotnet test
```

---

## 🛣️ Планы

- [ ] Платёжная система
- [ ] Расширение функционала админа
- [ ] Интеграционные тесты

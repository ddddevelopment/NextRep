# NextRep

## Обзор

NextRep — это комплексное приложение для отслеживания тренировок, которое помогает пользователям регистрировать свои тренировки, упражнения и подходы. Оно состоит из нескольких микросервисов, телеграм-бота для взаимодействия с пользователем и набора инструментов для мониторинга и администрирования.

## Архитектура

Приложение построено на основе микросервисной архитектуры с использованием .NET, Docker и PostgreSQL.

### Сервисы

- **Users API (`usersapi`)**: Управляет данными пользователей.
- **Users gRPC Service (`usersgrpcservice`)**: Предоставляет gRPC-интерфейс для доступа к данным пользователей.
- **Auth API (`authapi`)**: Отвечает за аутентификацию и авторизацию пользователей.
- **Workouts API (`workoutsapi`)**: Управляет данными о тренировках, упражнениях и подходах.
- **Telegram Bot (`telegrambot`)**: Обеспечивает взаимодействие с пользователем через Telegram.

### Инфраструктура

- **PostgreSQL (`postgres`)**: Основная база данных для хранения данных пользователей и тренировок.
- **pgAdmin (`pgadmin`)**: Веб-интерфейс для администрирования базы данных PostgreSQL.
- **Prometheus (`prometheus`)**: Система мониторинга и сбора метрик.
- **Grafana (`grafana`)**: Платформа для визуализации и анализа метрик.
- **Jaeger (`jaeger`)**: Система для распределенной трассировки.

---

## Документация по API

### Users API

Сервис для управления пользователями.

- **URL**: `http://localhost:8080`
- **Контроллер**: `UsersController`

#### Эндпоинты

- `POST /Users`: Создать нового пользователя.
- `GET /Users/{id}`: Получить пользователя по ID.
- `GET /Users/by-email/{email}`: Получить пользователя по email.
- `GET /Users`: Получить всех пользователей.
- `PUT /Users`: Обновить данные пользователя.
- `DELETE /Users?id={id}`: Удалить пользователя по ID.

### Users gRPC Service

gRPC-сервис для работы с пользователями.

- **URL**: `http://localhost:8081`
- **Сервис**: `UsersGrpcService`

#### Методы

- `GetUserByEmail(GetUserRequest)`: Получить пользователя по email.
- `CreateUser(CreateUserRequest)`: Создать нового пользователя.

### Auth API

Сервис для аутентификации.

- **URL**: `http://localhost:7080`
- **Контроллер**: `AuthController`

#### Эндпоинты

- `POST /Auth/login`: Аутентификация пользователя и получение токена доступа.
- `POST /Auth/register`: Регистрация нового пользователя.

### Workouts API

Сервис для управления тренировками.

- **URL**: `http://localhost:8050`
- **Контроллеры**:
    - `WorkoutsController`
    - `ExercisesController`
    - `SetsController`
    - `ExerciseInfosController`

#### Эндпоинты

##### `WorkoutsController`

- `POST /api/Workouts`: Создать новую тренировку.
- `GET /api/Workouts/{id}`: Получить тренировку по ID.
- `GET /api/Workouts`: Получить все тренировки текущего пользователя.
- `PUT /api/Workouts/{id}`: Обновить тренировку.
- `DELETE /api/Workouts/{id}`: Удалить тренировку.

##### `ExercisesController`

- `POST /api/workouts/{workoutId}/exercises`: Добавить упражнение в тренировку.
- `GET /api/workouts/{workoutId}/exercises/{exerciseId}`: Получить упражнение по ID.
- `GET /api/workouts/{workoutId}/exercises`: Получить все упражнения в тренировке.
- `PUT /api/workouts/{workoutId}/exercises/{exerciseId}`: Обновить упражнение.
- `DELETE /api/workouts/{workoutId}/exercises/{exerciseId}`: Удалить упражнение из тренировки.

##### `SetsController`

- `POST /api/workouts/{workoutId}/exercises/{exerciseId}/sets`: Добавить подход в упражнение.
- `GET /api/workouts/{workoutId}/exercises/{exerciseId}/sets/{setId}`: Получить подход по ID.
- `GET /api/workouts/{workoutId}/exercises/{exerciseId}/sets`: Получить все подходы в упражнении.
- `PUT /api/workouts/{workoutId}/exercises/{exerciseId}/sets/{setId}`: Обновить подход.
- `DELETE /api/workouts/{workoutId}/exercises/{exerciseId}/sets/{setId}`: Удалить подход из упражнения.

##### `ExerciseInfosController`

- `POST /api/ExerciseInfos`: Создать информацию об упражнении.
- `GET /api/ExerciseInfos/{id}`: Получить информацию об упражнении по ID.
- `GET /api/ExerciseInfos`: Получить всю информацию об упражнениях.
- `PUT /api/ExerciseInfos/{id}`: Обновить информацию об упражнении.
- `DELETE /api/ExerciseInfos/{id}`: Удалить информацию об упражнении.

---

## Telegram Bot

Телеграм-бот предоставляет пользователям удобный интерфейс для взаимодействия с приложением.

### Команды

#### Основные

- `/start`: Начало работы с ботом.
- `/help`: Показать справочную информацию.
- `/menu`: Показать главное меню.
- `/profile`: Показать профиль пользователя.
- `/stats`: Показать статистику.
- `/logout`: Выйти из системы.

#### Аутентификация

- `/login <email> <password>`: Войти в систему.
- `/register <email> <password> <firstName> <lastName> <telephone>`: Зарегистрироваться.

#### Управление информацией об упражнениях

- `/create_exercise_info "Название" "Группа мышц" ["Описание"]`: Создать новое упражнение.
- `/get_all_exercise_infos`: Получить список всех упражнений.
- `/get_exercise_info_by_id <ID>`: Получить информацию об упражнении по ID.
- `/delete_exercise_info <ID>`: Удалить упражнение.
- `/update_exercise_info <ID> "Новое название" "Новая группа мышц" ["Новое описание"]`: Обновить упражнение.

#### Управление тренировками

- `/create_workout "Имя тренировки" ["Заметки"]`: Создать новую тренировку.
- `/workouts`: Показать список тренировок.
- `/get_workout_by_id <ID>`: Получить тренировку по ID.
- `/update_workout <ID> "Новое имя" ["Новые заметки"]`: Обновить тренировку.
- `/delete_workout <ID>`: Удалить тренировку.

#### Управление упражнениями в тренировке

- `/add_exercise_to_workout <ID тренировки> <ID упражнения> ["заметки"]`: Добавить упражнение в тренировку.
- `/get_exercises <ID тренировки>`: Получить список упражнений в тренировке.
- `/get_exercise_by_id <ID тренировки> <ID упражнения>`: Получить упражнение из тренировки.
- `/update_exercise <ID тренировки> <ID упражнения> ["новые заметки"]`: Обновить упражнение в тренировке.
- `/delete_exercise <ID тренировки> <ID упражнения>`: Удалить упражнение из тренировки.

#### Управление подходами

- `/add_set <ID тренировки> <ID упражнения> [вес] [повторения] ["заметка"]`: Добавить подход.
- `/get_sets <ID тренировки> <ID упражнения>`: Получить список подходов.
- `/update_set <ID тренировки> <ID упражнения> <ID подхода> [вес] [повторения] ["заметка"]`: Обновить подход.
- `/delete_set <ID тренировки> <ID упражнения> <ID подхода>`: Удалить подход. 
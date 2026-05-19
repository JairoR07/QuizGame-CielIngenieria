# 🎯 Quiz Game — Ciel Ingeniería

Aplicación web de concurso de preguntas con 5 niveles de dificultad progresiva y acumulación de premios. Desarrollada como prueba técnica para **Ciel Ingeniería**.

---

## 🚀 Ejecución rápida con Docker

> **Requisito único:** tener [Docker Desktop](https://www.docker.com/products/docker-desktop) instalado.

```bash
git clone https://github.com/JairoR07/QuizGame-CielIngenieria.git
cd QuizGame-CielIngenieria
docker-compose up --build
```

| Servicio | URL |
|----------|-----|
| 🎮 Juego (Frontend) | http://localhost:7182 |
| 📄 API Swagger | http://localhost:7008/swagger |

---

## 🏗️ Arquitectura de la solución

```
QuizGameSolution/
├── ApiQuizGame/          # Backend — ASP.NET Core Web API
│   ├── Controllers/      # Endpoints REST
│   ├── Services/         # Lógica de negocio + integración IA
│   ├── Data/             # DbContext + Seeder
│   ├── DTOs/             # Objetos de transferencia
│   ├── Entities/         # Modelos de base de datos
│   ├── Middlewares/      # Manejo global de excepciones
│   └── Migrations/       # Entity Framework migrations
│
├── QuizGame/             # Frontend — ASP.NET Core MVC
│   ├── Controllers/      # HomeController
│   ├── Views/            # Razor Pages (Index, Question, Result)
│   ├── Services/         # GameApiService (cliente HTTP)
│   └── wwwroot/          # CSS, JS, Bootstrap
│
└── docker-compose.yml    # Orquestación de contenedores
```

---

## ⚙️ Stack tecnológico

| Capa | Tecnología |
|------|-----------|
| Backend | ASP.NET Core 8 Web API |
| Frontend | ASP.NET Core 8 MVC + Razor |
| ORM | Entity Framework Core 8 |
| Base de datos | SQL Server 2022 |
| IA generativa | Google Gemini API (gemini-2.0-flash) |
| Documentación API | Swagger / OpenAPI |
| Logging | Serilog (consola + archivo) |
| Contenedores | Docker + Docker Compose |

---

## 🎮 Funcionalidades

- ✅ Configurar categorías con niveles y premios
- ✅ Gestionar preguntas con 4 opciones (vía Swagger)
- ✅ Generación automática de preguntas con IA (Google Gemini)
- ✅ Iniciar partida con nombre de jugador
- ✅ Preguntas aleatorias por nivel
- ✅ Responder pregunta y validar resultado
- ✅ Acumular premio por cada respuesta correcta
- ✅ Retiro voluntario conservando el acumulado
- ✅ Fin forzado por respuesta incorrecta (pierde acumulado)
- ✅ Pantalla de ganador al completar nivel 5
- ✅ Manejo global de excepciones
- ✅ Logging con Serilog

---

## 💰 Escala de premios

| Nivel | Premio ronda | Acumulado |
|-------|-------------|-----------|
| 1 — Básico | $100.000 | $100.000 |
| 2 — Intermedio | $200.000 | $300.000 |
| 3 — Avanzado | $500.000 | $800.000 |
| 4 — Experto | $1.000.000 | $1.800.000 |
| 5 — Maestro 🏆 | $5.000.000 | $6.800.000 |

---

## 📡 Endpoints principales

### Categorías
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/categories` | Listar todas las categorías |
| POST | `/api/categories` | Crear nueva categoría |
| PUT | `/api/categories/{id}` | Actualizar categoría |
| DELETE | `/api/categories/{id}` | Eliminar categoría |

### Preguntas
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/questions` | Listar todas las preguntas |
| POST | `/api/questions` | Crear pregunta con 4 opciones |
| DELETE | `/api/questions/{id}` | Eliminar pregunta |

### Juego
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/games/start` | Iniciar nueva partida |
| GET | `/api/games/{id}/question` | Obtener pregunta del nivel actual |
| POST | `/api/games/{id}/answer` | Responder pregunta |
| POST | `/api/games/{id}/withdraw` | Retirarse del juego |

---

## 🤖 Integración con IA

El sistema integra **Google Gemini** para generar preguntas automáticamente cuando no hay disponibles para un nivel. Las preguntas generadas se guardan en la BD para reutilizarlas en futuras partidas.

Para habilitarlo, crea un archivo `.env` en la raíz del proyecto:

```env
GEMINI_API_KEY=tu_api_key_aqui
```

Obtén tu API Key gratuita en [aistudio.google.com](https://aistudio.google.com).

---

## 🗄️ Estados de una partida

| Estado | Descripción |
|--------|-------------|
| `InProgress` | Partida activa |
| `Winner` | Ganó el nivel 5 |
| `Lost` | Respondió incorrectamente |
| `Withdrawn` | Se retiró voluntariamente |

---

## 📋 Ejecución sin Docker (desarrollo local)

**Requisitos:** .NET 8 SDK + SQL Server + Visual Studio 2022

1. Actualiza la cadena de conexión en `ApiQuizGame/appsettings.json`
2. Aplica las migraciones:
```bash
cd ApiQuizGame
dotnet ef database update
```
3. Ejecuta ambos proyectos desde Visual Studio con **F5** configurando inicio múltiple.

---

## 📁 Documentación

| Documento | Descripción |
|-----------|-------------|
| `01_Casos_de_Uso.docx` | Especificación de casos de uso |
| `02_Entidad_Relacion.docx` | Modelo de datos |
| `03_Arquitectura.docx` | Arquitectura de la solución |
| `04_Script_BaseDatos.sql` | Script de base de datos |
| `05_Instalacion.docx` | Guía de instalación y configuración |

---

## 👨‍💻 Autor

**Jairo Riaño** — Prueba técnica Ciel Ingeniería — Mayo 2026

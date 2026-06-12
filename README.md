# EscuelaApp — Proyecto Final Base de Datos Avanzado

Aplicación web para gestión escolar: registro de alumnos y sus calificaciones.
Construida con ASP.NET Core MVC y MongoDB 8.0.26 como base de datos principal.
Permite login, registro y operaciones CRUD completas sobre dos entidades relacionadas.

## Entidades y relación

- **Alumno**: nombre, email, matrícula, carrera
- **Calificacion**: materia, calificación, periodo → referencia a Alumno via `AlumnoId` (ObjectId)

**Relación: Referencia (ObjectId)**
Se eligió referencia porque un alumno puede tener muchas calificaciones a lo largo del tiempo.
Guardarlas embebidas haría crecer el documento sin control y dificultaría editar o eliminar una calificación individual.

## Versión de MongoDB

**8.0.26**

## Stack

- Backend: ASP.NET Core 8 MVC (C#)
- Base de datos: MongoDB 8.0.26 via MongoDB.Driver
- Autenticación: Cookie Authentication + BCrypt
- Despliegue: Railway

## Índices declarados

- `email` único en colección `alumnos`
- `email` único en colección `usuarios`
- `matricula` única en colección `alumnos`

## Cómo correr el seed

El seed se ejecuta **automáticamente** al iniciar la aplicación (`Program.cs` llama `SeedAsync()`).
Carga el usuario demo y 4 alumnos con calificaciones de ejemplo.

Usuario demo: `demo@demo.com` / `Demo1234`

## Cómo correr localmente

```bash
# 1. Clonar el repositorio
git clone https://github.com/luisalfonsoinzunzaaguilar/Sistema-Escuela-BD-con-Mongo.git
cd EscuelaApp

# 2. Configurar la cadena de conexión en appsettings.json
#    Reemplazar "TU_MONGODB_URI_AQUI" con tu URI de Atlas

# 3. Ejecutar
dotnet run
```

📁 GestorDocumentos API - DocumentosController
Requisitos

.NET 7 SDK o superior
SQL Server u otro proveedor compatible con EF Core
Visual Studio o VS Code

dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

Install-Package BCrypt.Net-Next
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.1 (asegurate que tengas la version correcta)
Install-Package Swashbuckle.AspNetCore
dotnet add package Swashbuckle.AspNetCore.Annotations
--->Ejecuta en tu consola en menu "Herramientas" -> en el item "Administracion de Paquetes Nuguet" -> "Consola de administrador de paquetes"

  Scaffold-DbContext "Server=DANIEL_FUENTES\SQLEXPRESS;Database=GestionProductos;User Id=user;Password=Pekit4s2022;Encrypt=False" 
  Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -forcé
Configura la cadena de conexión en appsettings.json: { "ConnectionStrings": { "ConexionPredeterminada": "Server=DANIEL_FUENTES\SQLEXPRESS;Database=GestionDocumentos;User Id=user;Password=Pekit4s2022;Encrypt=False" }, "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } }, "AllowedHosts": "*", "JWT_Configuracion": { "LLaveSecreta": "Q2h6Rn5vYk4!@x0npz9$Jt5HgL1wM8g1YQ3E&bK6k2W#CmXrP6tV7z5eZ1QbUfO" } }


Agrega la llave secreta del JWT en appsettings.json: "JWT_Configuracion": { "LLaveSecreta": "Q2h6Rn5vYk4!@x0npz9$Jt5HgL1wM8g1YQ3E&bK6k2W#CmXrP6tV7z5eZ1QbUfO" }

Aplica las migraciones de Entity Framework:

dotnet ef migrations add InitialCreate dotnet ef database update

Asegurate de tener instalada la herramienta de EF CLI: dotnet tool install --global dotnet-ef

Ejecuta el proyecto: dotnet run
Este token se debe incluir en las peticiones protegidas usando el encabezado: Todos los endpoints (excepto /login) requieren autorizacion JWT.
________________________________________
 Autenticación
La mayoría de los endpoints requieren un token JWT válido.
Puedes obtenerlo a través del endpoint /usuarios/login.
 Ejemplo de autenticación
POST /usuarios/login
Genera un token JWT para las solicitudes autenticadas.
Parámetros (form-data):
Usuario: admin  
Contraseña: 1234
Respuesta (JSON):
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6..."
}
________________________________________
 Usuarios
 GET /usuarios
Obtiene la lista completa de usuarios registrados en el sistema.
 GET /usuarios/{id}
Recupera los datos de un usuario específico a partir de su identificador (GUID).
 POST /usuarios
Crea un nuevo usuario en el sistema.
Cuerpo (JSON):
{
  "usuario": "jlopez",
  "contraseña": "Segura123*",
  "estatus": "ACTIVO"
}
 PUT /usuarios/{id}
Actualiza los datos de un usuario existente.
Cuerpo (JSON):
{
  "usuario": "jlopez",
  "estatus": "CHANGEPASSWORD",
  "horainicio": "08:00:00",
  "horafin": "17:00:00"
}
 DELETE /usuarios/{id}
Elimina un usuario de forma lógica o permanente según configuración.
________________________________________
📂 Documentos
📄 GET /documentos
Recupera la lista de todos los documentos disponibles en el sistema.
 GET /documentos/{id}
Obtiene los metadatos de un documento por su identificador.
 POST /documentos/subir
Permite subir un nuevo documento asociado a un usuario.
Cuerpo (form-data):
archivo: [PDF o DOCX]
usuarioId: GUID_DEL_USUARIO
descripcion: "Informe trimestral"
 GET /documentos/descargar/{id}
Descarga el archivo físico del documento.
 PUT /documentos/{id}
Actualiza los metadatos de un documento (nombre, descripción, etiquetas, etc.).
 DELETE /documentos/{id}
Elimina un documento del repositorio.
________________________________________
 POST /documentos/ConsultarDocumentos
Consulta dinámica de documentos, compatible con DataTables o cualquier sistema de paginación.
Parámetros (form-data):
search, columns, order, start, length
Permiten filtrar, ordenar y paginar los resultados de manera eficiente.
________________________________________
 Errores comunes
•	401 Unauthorized: Token JWT ausente o inválido.
•	404 Not Found: Documento o usuario no existente.
•	409 Conflict: Ya existe un documento con el mismo nombre o hash.
•	500 Internal Server Error: Error inesperado durante el procesamiento del archivo.
________________________________________
🛠️ Tecnologías Utilizadas
•	ASP.NET Core 8.0
•	Entity Framework Core
•	Autenticación JWT
•	Swagger (Swashbuckle)
•	SQL Server
•	IFormFile para carga de archivos
________________________________________
 Swagger
La documentación interactiva de la API se encuentra disponible en:
 /swagger
cuando la opción está habilitada en el entorno de desarrollo.


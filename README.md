# EComerce - Plataforma de Comercio Electronico

Sistema de comercio electronico desarrollado sobre .NET 10 bajo los principios de Clean Architecture (Arquitectura Limpia). Incorpora autenticacion y autorizacion basada en roles con ASP.NET Core Identity, comunicacion bidireccional en tiempo real con SignalR para mensajeria entre usuarios, persistencia en SQL Server mediante Entity Framework Core, y un middleware centralizado para el tratamiento estandarizado y seguro de excepciones.

---

## Tabla de Contenidos

1. Descripcion General
2. Arquitectura y Tecnologias
3. Estructura de la Solucion
4. Flujo de Trabajo por Capas (Ejemplo: Modulo de Productos)
5. Politica de Manejo de Errores y Seguridad
6. Requisitos del Sistema
7. Guia de Instalacion y Uso
8. Credenciales Iniciales de Prueba
9. Gestion de Datos Sensibles y Configuracion Segura

---

## 1. Descripcion General

El proyecto proporciona la base arquitectonica para una tienda virtual modular, escalable y mantenible. Sus pilares principales son:

- Separacion estricta de responsabilidades mediante cuatro capas bien definidas.
- Gestion de identidades, cuentas y roles (Administrador y Cliente) con politicas de bloqueo de cuenta y requisitos de seguridad de contrasenas.
- Chat punto a punto en tiempo real utilizando SignalR, permitiendo la comunicacion fluida entre usuarios autenticados.
- Respuestas de error uniformes en formato JSON/Problem Details, impidiendo la exposicion accidental de cadenas de conexion o trazas de depuracion hacia clientes externos.

---

## 2. Arquitectura y Tecnologias

- Framework Principal: .NET 10 (C# 13 / C# 14)
- Presentacion Web: ASP.NET Core MVC con Vistas Razor
- Tiempo Real: ASP.NET Core SignalR
- Seguridad e Identidad: ASP.NET Core Identity con Entity Framework Stores
- Acceso a Datos y ORM: Entity Framework Core 10
- Motor de Base de Datos: Microsoft SQL Server (compatible con LocalDB, Express y versiones completas)
- Formato de Solucion: Visual Studio Solution File (.slnx)

---

## 3. Estructura de la Solucion

El repositorio se organiza bajo el patron Clean Architecture dentro de la carpeta `src/`:

```text
LogisticaCentralPais/
|-- .gitignore
|-- EComerce.slnx
|-- README.md
`-- src/
    |-- EComerce.Domain/
    |   |-- Common/
    |   |   `-- BaseEntity.cs
    |   |-- Entities/
    |   |   `-- ChatMessage.cs
    |   |-- Enums/
    |   |   `-- UserRole.cs
    |   `-- Interfaces/
    |       `-- IChatRepository.cs
    |
    |-- EComerce.Application/
    |   |-- DTOs/
    |   |   `-- Chat/
    |   |       |-- ChatMessageDto.cs
    |   |       `-- SendMessageDto.cs
    |   |-- Interfaces/
    |   |   |-- IChatService.cs
    |   |   `-- ICurrentUserService.cs
    |   |-- Services/
    |   |   `-- ChatService.cs
    |   `-- DependencyInjection.cs
    |
    |-- EComerce.Infrastructure/
    |   |-- Identity/
    |   |   `-- ApplicationUser.cs
    |   |-- Persistence/
    |   |   |-- ApplicationDbContext.cs
    |   |   |-- Configurations/
    |   |   `-- Migrations/
    |   |-- Repositories/
    |   |   `-- ChatRepository.cs
    |   `-- DependencyInjection.cs
    |
    `-- EComerce.Web/
        |-- Controllers/
        |   |-- HomeController.cs
        |   `-- ChatController.cs
        |-- Data/
        |   `-- DatabaseSeeder.cs
        |-- Hubs/
        |   `-- ChatHub.cs
        |-- Middleware/
        |   |-- ApiErrorResponse.cs
        |   |-- ErrorCodes.cs
        |   `-- GlobalExceptionHandlerMiddleware.cs
        |-- Services/
        |   `-- CurrentUserService.cs
        |-- Views/
        |   |-- Chat/
        |   |   `-- Index.cshtml
        |   |-- Home/
        |   `-- Shared/
        |       |-- _Layout.cshtml
        |       `-- _LoginPartial.cshtml
        |-- appsettings.json
        |-- appsettings.Example.json
        `-- Program.cs
```

### Detalle de Responsabilidades por Capa

1. EComerce.Domain:
   Representa el nucleo del sistema. No posee dependencias hacia ninguna otra capa ni frameworks externos (excepto bibliotecas base de .NET).
   - Entidades de negocio (ChatMessage, etc.) con sus propiedades e invariantes.
   - Enumeraciones del dominio (UserRole: Admin, Customer).
   - Contratos e interfaces de repositorios (IChatRepository).

2. EComerce.Application:
   Coordina los casos de uso y la logica de negocio de la aplicacion. Depende unicamente de EComerce.Domain.
   - Objetos de Transferencia de Datos (DTOs) para entrada y salida.
   - Interfaces de servicios y abstracciones de aplicacion (IChatService, ICurrentUserService).
   - Implementaciones de casos de uso y validaciones de negocio.

3. EComerce.Infrastructure:
   Implementa las dependencias tecnicas, acceso a datos y proveedores externos. Depende de Domain y Application.
   - Configuración de Entity Framework Core (`ApplicationDbContext`).
   - Mapeo de entidades y migraciones de base de datos.
   - Implementaciones de repositorios (`ChatRepository`).
   - Extension del modelo de identidad (`ApplicationUser`).

4. EComerce.Web:
   Punto de entrada de la aplicacion (Presentation Layer). Depende de Application e Infrastructure para componer la solucion.
   - Controladores MVC y vistas Razor.
   - Hubs de SignalR para eventos en tiempo real.
   - Filtros, middleware global de manejo de excepciones y configuracion de cookies.
   - Configuracion del contenedor de inyeccion de dependencias en `Program.cs`.
   - Inicializacion automatica y siembra de datos (`DatabaseSeeder`).

---

## 4. Flujo de Trabajo por Capas (Ejemplo: Modulo de Productos)

Cuando se requiera anadir una nueva entidad o caracteristica de negocio (por ejemplo, el catalogo de productos), se debe respetar la siguiente secuencia:

1. Capa Domain:
   - Crear la entidad `Product.cs` en `Entities/` heredando de `BaseEntity`.
   - Declarar la interfaz `IProductRepository.cs` en `Interfaces/`.

2. Capa Application:
   - Definir los DTOs: `ProductDto.cs`, `CreateProductDto.cs`, `UpdateProductDto.cs`.
   - Declarar la interfaz del servicio: `IProductService.cs`.
   - Implementar la logica de negocio en `ProductService.cs`.

3. Capa Infrastructure:
   - Registrar `DbSet<Product> Products { get; set; }` en `ApplicationDbContext.cs`.
   - Crear la configuracion Fluent API en `Configurations/ProductConfiguration.cs`.
   - Implementar `ProductRepository.cs` en `Repositories/`.
   - Generar la migracion correspondiente con `dotnet ef migrations add AddProducts`.

4. Capa Web:
   - Crear el controlador `ProductController.cs` inyectando `IProductService`.
   - Crear las vistas Razor correspondientes en `Views/Product/` (Listado, Detalle, Crear, Editar).
   - Aplicar autorizacion mediante `[Authorize(Roles = "Admin")]` para operaciones de administracion de inventario.

---

## 5. Politica de Manejo de Errores y Seguridad

El middleware `GlobalExceptionHandlerMiddleware` intercepta cualquier fallo no controlado durante el ciclo de vida de la peticion HTTP:

- Formato Estandarizado: Respuestas con estructura fija (`ApiErrorResponse`) que incluyen `Success: false`, `ErrorCode`, `Message`, `CorrelationId` y la marca temporal en formato UTC.
- Enmascaramiento de Datos Sensibles: En entornos de Produccion, nunca se devuelven trazas de pila (stack traces), rutas de archivos internos ni datos de conexion.
- Catalogo de Codigos de Error: Documentados en `ErrorCodes.cs` (por ejemplo: `AUTH_UNAUTHORIZED`, `VAL_INVALID_INPUT`, `DB_QUERY_FAILED`, `SRV_INTERNAL_ERROR`).

---

## 6. Requisitos del Sistema

Antes de comenzar, asegurese de disponer de los siguientes componentes instalados en su estacion de trabajo:

- .NET 10 SDK (Version 10.0 o superior):
  Verificable ejecutando: `dotnet --version`
- Motor de Base de Datos SQL Server:
  - Opcion 1 (Por defecto en Windows): SQL Server LocalDB (`(localdb)\MSSQLLocalDB`), incluido con Visual Studio.
  - Opcion 2: SQL Server Express o Developer Edition.
  - Opcion 3: Contenedor Docker de SQL Server para Linux o macOS.
- Cliente Git:
  Verificable ejecutando: `git --version`
- Entorno de Desarrollo Recomendado:
  - Visual Studio 2026 (con la carga de trabajo de desarrollo ASP.NET y web instalada).
  - Visual Studio Code con las extensiones C# Dev Kit.
  - JetBrains Rider 2025 o superior.

---

## 7. Guia de Instalacion y Uso

### Paso 1: Clonar el Repositorio

Abra una terminal y ejecute:

```bash
git clone <URL_DEL_REPOSITORIO>
cd LogisticaCentralPais
```

### Paso 2: Configurar la Cadena de Conexion a Base de Datos

Por defecto, la aplicacion busca una instancia de SQL Server LocalDB en la clave `DefaultConnection`.

Para personalizar la cadena de conexion sin alterar archivos controlados por Git:

Opcion A - Usar `dotnet user-secrets` (Recomendada para Desarrollo):

```bash
cd src/EComerce.Web
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=EComerceDb;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"
cd ../..
```

Opcion B - Crear archivo local `appsettings.Development.json`:

Copie el archivo de ejemplo `src/EComerce.Web/appsettings.Example.json` a `src/EComerce.Web/appsettings.Development.json` y ajuste su contenido:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EComerceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

Nota: El archivo `appsettings.Development.json` se encuentra ignorado en el `.gitignore` para salvaguardar sus datos confidenciales.

### Paso 3: Restaurar Paquetes y Compilar

Desde la raiz del proyecto:

```bash
dotnet restore
dotnet build
```

### Paso 4: Base de Datos y Migraciones

La aplicacion incluye migracion automatica al arrancar. Cada vez que se ejecuta el proyecto, `Program.cs` aplica de manera automatica las migraciones pendientes sobre la base de datos configurada y ejecuta el sembrador de datos iniciales.

Si desea aplicar las migraciones manualmente mediante la linea de comandos:

```bash
dotnet tool install --global dotnet-ef  # Solo si no esta instalado previamente

dotnet ef database update \
  --project src/EComerce.Infrastructure/EComerce.Infrastructure.csproj \
  --startup-project src/EComerce.Web/EComerce.Web.csproj
```

### Paso 5: Ejecutar la Aplicacion

Para levantar el servidor web de desarrollo:

```bash
dotnet run --project src/EComerce.Web/EComerce.Web.csproj
```

La consola mostrara la direccion local de escucha, por ejemplo:
`http://localhost:5104`

Abra dicha URL en su navegador web preferido.

### Paso 6: Verificacion del Chat en Tiempo Real

Para probar la comunicacion bidireccional mediante SignalR:

1. Inicie sesion con la cuenta de administrador en una ventana normal de su navegador.
2. Abra una ventana de navegacion privada o un navegador alternativo.
3. Registre una nueva cuenta de cliente desde la opcion "Register".
4. Acceda a la seccion "Chat" desde la barra de navegacion en ambas ventanas.
5. Seleccione al otro usuario de la lista de contactos e intercambie mensajes en tiempo real.

---

## 8. Credenciales Iniciales de Prueba

El seeder inicial (`DatabaseSeeder.cs`) crea de forma automatica los roles de sistema (`Admin`, `Customer`) y el siguiente usuario con privilegios de administracion:

- Correo Electronico: `admin@ecomerce.local`
- Contrasena: `Admin@1234!`
- Rol Asignado: `Admin`

Aviso de Seguridad: Cambie esta contrasena inmediatamente antes de desplegar en entornos compartidos o productivos.

---

## 9. Gestion de Datos Sensibles y Configuracion Segura

Este repositorio implementa practicas estrictas para prevenir fugas de secretos:

1. Archivos Ignorados:
   - Archivos de entorno especifico (`appsettings.Development.json`, `appsettings.Production.json`, etc.) estan excluidos en el archivo `.gitignore`.
   - Archivos de certificados (`*.pfx`, `*.pem`, `*.key`) y bases de datos locales (`*.mdf`, `*.ldf`, `*.sqlite`) estan excluidos.
   - Se ignora la carpeta de almacenamiento de secretos de usuario de ASP.NET (`secrets.json`).

2. Recomendaciones para Entornos Productivos:
   - Nunca almacene contrasenas ni cadenas de conexion reales en archivos `appsettings.json` commiteados.
   - Utilice variables de entorno del sistema operativo o gestores de secretos en la nube (como Azure Key Vault, AWS Secrets Manager o HashiCorp Vault) mediante los proveedores de configuracion nativos de ASP.NET Core.

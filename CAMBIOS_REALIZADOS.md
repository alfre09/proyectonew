# Cambios realizados al proyecto SIV

## 1. Conexión a base de datos
`appsettings.json` ahora apunta a `Server=localhost` (antes usaba LocalDB, que no es lo
que tienes instalado con SSMS). Si tu instancia tiene otro nombre (ej. `localhost\SQLEXPRESS`),
cámbialo ahí.

## 2. La app ya no requiere pasos manuales para tener datos de prueba
En `Program.cs` se agregó:
- `context.Database.Migrate()` → aplica automáticamente las 3 migraciones existentes al
  arrancar (crea las tablas si no existen).
- `SeedData.InicializarAsync(context)` (nuevo archivo `Data/SeedData.cs`) → siembra, si
  no existen ya:
  - **Estados de vuelo**: Programado, Embarcando, En Vuelo, Aterrizado, Cancelado.
  - **Aerolíneas** de ejemplo: American Airlines, LATAM, Copa, JetBlue.
  - **Aeropuertos** de ejemplo: SDQ, PUJ, MIA, JFK, MAD.

No tuve que crear ninguna migración nueva ni tocar ningún modelo — tu base de datos
actual sigue siendo compatible tal cual.

## 3. Bug corregido: `AerolineaController`
Era el scaffolding por defecto de Visual Studio (`return View()`), pero el proyecto no
tiene soporte de Vistas MVC (`AddControllersWithViews`) ni carpeta `Views/`. Si alguien
entraba a `/Aerolinea` esto tronaba con error 500. Se reemplazó por un controlador API
igual al patrón del resto del proyecto (`api/Aerolinea`), respaldado por un servicio real
que sí guarda en base de datos.

## 4. Módulo nuevo: Aeropuertos
No existía ningún CRUD para aeropuertos (solo el modelo). Se agregó:
- `Application/Dtos/AeropuertoDto.cs`
- `Application/interfaces/IAeropuertoService.cs`
- `Application/services/AeropuertoService.cs`
- `Controllers/AeropuertoController.cs` (`api/Aeropuerto`)

## 5. Auditoría (regla de negocio que no se cumplía)
La tabla `Auditorias` existía pero ningún servicio escribía ahí, violando la regla
"Toda acción relevante debe quedar registrada". Se agregó:
- `Application/interfaces/IAuditoriaService.cs` / `Application/services/AuditoriaService.cs`
- Se inyectó en `VueloService`, `SeguimientoService`, `CambioOperativoService` y
  `NotificacionService`, y ahora cada Crear/Actualizar/Eliminar/Cambio/Notificación deja
  un registro de auditoría.
- `Controllers/AuditoriaController.cs` para poder consultarla (`api/Auditoria`).

## 6. Máquina de estados del vuelo (regla que no se validaba)
Antes solo se bloqueaban cambios sobre un vuelo ya **Cancelado**; no existía ninguna
validación de que los estados avanzaran en orden. Se agregó
`Application/base/CicloEstadosVuelo.cs`, compartida por `VueloService` y
`CambioOperativoService`, que define la secuencia:

```
Programado → Embarcando → En Vuelo → Aterrizado
```

con `Cancelado` como estado final alcanzable desde cualquier punto (excepto desde
Aterrizado o desde el propio Cancelado). Reglas aplicadas:
- No se puede saltar estados (ej. de Programado a Aterrizado directo).
- No se pueden registrar más cambios sobre un vuelo en estado final (Aterrizado o Cancelado).
- Todo vuelo **nuevo** se crea automáticamente en estado "Programado" (ya no se elige
  manualmente al crear, para no violar la regla "no se permite la existencia de vuelos
  activos sin una programación válida").

Se agregó un nuevo tipo de cambio operativo, **`CambioEstado`**, para avanzar el vuelo
por la secuencia (además de `Retraso`, `Adelanto`, `CambioPuerta`, `Cancelacion`).

## 7. Validaciones que faltaban (para que no truene con datos inválidos)
- `VueloService`: valida que la aerolínea, el aeropuerto de origen y el de destino
  existan antes de guardar (antes esto solo lo hubiera detectado la restricción de
  llave foránea de SQL Server con un error feo). También valida que origen ≠ destino.
- `SeguimientoService`: valida que el vuelo exista y que el mismo usuario no siga el
  mismo vuelo dos veces.
- Todos los controladores relevantes ahora capturan `InvalidOperationException` y
  devuelven `400 BadRequest` con un mensaje claro, en vez de un error 500 genérico.

## 8. Páginas Razor ajustadas
`Pages/Vuelos/Crear.cshtml` y `Editar.cshtml` ya no dejan elegir el estado libremente
desde un `<select>` (eso rompía la máquina de estados). El estado se asigna
automáticamente al crear, y para cambiarlo después hay que usar el módulo de Cambios
Operativos (`api/CambiosOperativos`), que sí valida todo.

---

# Cómo probarlo (Swagger)

Corre el proyecto y abre `/swagger`. Como la app siembra datos automáticamente, ya vas a
tener aerolíneas, aeropuertos y estados listos para usar (revisa sus IDs con
`GET /api/Aerolinea` y `GET /api/Aeropuerto`, y los estados con una consulta directa a
`EstadosVuelo` o viendo el historial luego de crear un vuelo).

**1. Crear un vuelo** (queda en estado "Programado" automáticamente)
```
POST /api/vuelos
{
  "numeroVuelo": "AA1234",
  "aerolineaId": 1,
  "aeropuertoOrigenId": 1,
  "aeropuertoDestinoId": 3,
  "horarioProgramado": "2026-07-20T14:30:00",
  "puerta": "B12",
  "estadoVueloId": 1
}
```
(el `estadoVueloId` que mandes se ignora si no es el de "Programado"... en realidad para
la API REST directa sí se respeta lo que envíes, pero se valida que sea el estado
"Programado"; usa el id que te devuelva `GET /api/vuelos` la primera vez que consultes
los estados, o revisa la tabla `EstadosVuelo` en SSMS).

**2. Seguir el vuelo**
```
POST /api/Seguimientos
{ "usuario": "juan@correo.com", "vueloId": 1 }
```

**3. Avanzar el estado del vuelo (dispara notificación al que lo sigue)**
```
POST /api/CambiosOperativos
{
  "vueloId": 1,
  "tipoCambio": "CambioEstado",
  "causa": "Inicio de abordaje",
  "nuevoEstadoVueloId": 2
}
```
Si intentas saltar a "Aterrizado" directamente desde "Programado", te va a devolver 400
con el mensaje de la regla violada — así se comprueba que la máquina de estados funciona.

**4. Ver las notificaciones generadas**
```
GET /api/Notificaciones/usuario/juan@correo.com
```

**5. Ver la trazabilidad**
```
GET /api/Auditoria
GET /api/HistorialEstados/vuelo/1
```

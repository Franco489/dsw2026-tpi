# COMO COLABORAR (v1)
A continuación, presentamos las convenciones para colaborar en este proyecto.

## Commits
Adoptamos la siguiente convención para realizar commits en este proyecto:
- Add: Adición de archivos relevantes (p ej: JSON con datos, clases, métodos/funcionalidades, etc)
- Upd: Adición de nuevas funcionalidades o mejoras a la aplicación.
- Fix: Solución de errores en la aplicación. 
- Refactor: Mejora de piezas de código que ya funcionaban (uso de mejores prácticas, aplicación de patrones o principios, mejor )

Ejemplo de uso: "Add: Clase PersistenceEf para la implementación de persistencia con EntityFramework."  
Ejemplo 2: "Add: CONTRIBUTING.md"

## APIs
- No nombramos los endpoints con el nombre del verbo (PostDoctor, PutSpeciality, etc) sino con el nombre de la accion (CreateDoctor, UpdateSpeciality)

## Preferencias adicionales
Respecto a las estructuras de código como while, if, try-catch, foearch, etc. Preferimos la siguiente notación:

```csharp
if (doctor == null || !doctor.IsActive)
{
    return NotFound();
}
```
Sobre esto:
```csharp
if (doctor == null || !doctor.IsActive) return NotFound();
```

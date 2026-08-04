# Trabajo Práctico Integrador
## Desarrollo de Software 2026


## Integrantes:
### Ferreyra Bernabe - 53018
### Reyes Franco Exequiel - 58223
### Ruiz Coronel Mariano Agustin - 58286
### Sosa Ariana - 58309

## Descripción de Endpoints

### Módulo de Autenticación
* **POST:** `"/auth/admin/register"` : Nos permite registrar un nuevo administrador. Se ingresa el email y contraseña deseados para el administrador, los cuales deben cumplir un determinado formato: Tiene que estar compuesta por lo menos de 8 dígitos en donde debe contener por lo menos un número, una letra mayúscula y minúscula y un carácter especial (por ejemplo: @, !, ?, #, $, %, etc.).
* **POST:** `"/auth/admin/login"` : Permite autenticar un administrador, donde simplemente se ingresa el email y contraseña.

### Módulo Paciente
* **POST:** `"/auth/patient/login"` : Permite autenticar o registrar a un paciente mediante el ingreso de su DNI y Email.

### Módulo Especialidades
* **GET:** `"/api/specialties?pageSize=number&pageIndex=number&name=string"` : Permite obtener el listado de las especialidades activas, mediante el ingreso del nombre. Este método contiene paginación, el cual permite establecer el tamaño de las páginas que queremos ver y el índice desde donde queremos visualizar.
* **POST:** `"/specialities"` : Permite al administrador crear una nueva especialidad médica, en donde ingresa su nombre y su descripción.
* **PUT:** `"/specialities/{id}"` : Permite al administrador actualizar los datos de una especialidad existente, mediante el ingreso de su ID.
* **DELETE:** `"/specialities/{id}"` : Permite al administrador eliminar una especialidad existente, mediante el ingreso de su ID.

### Módulo Médicos
* **GET:** `"/doctors"` : Permite al administrador listar los médicos cargados. El retorno contiene paginación, el cual permite establecer el tamaño de las páginas que queremos ver y el índice desde donde queremos visualizar.
* **POST:** `"/doctors"` : Permite al administrador registrar un nuevo doctor, mediante el ingreso de su nombre, su número de licencia y el ID de la especialidad asociada al mismo.
* **PUT:** `"/doctors/{id}"` : Permite al administrador modificar la información de un doctor mediante el ingreso del ID, modificando los campos que desea.
* **DELETE:** `"/doctors/{id}"` : Permite al administrador realizar un borrado lógico del doctor mediante el ingreso de su ID.
* **GET:** `"/doctors/{id}/availabilities"` : Permite al paciente y al administrador listar los horarios y turnos disponibles para un doctor en específico, mediante el ingreso del ID del doctor. Este método contiene paginación, el cual permite establecer el tamaño de las páginas que queremos ver y el índice desde donde queremos visualizar.

### Módulo Disponibilidades
* **POST:** `"/api/availabilities"` : Permite al administrador generar las reglas de disponibilidad y los slots (turnos) para un mes, indicando el ID del doctor y el día junto con su horario de comienzo y finalización.
* **PUT:** `"/api/availabilities"` : Permite al administrador actualizar la disponibilidad de los doctores (solo si no hay turnos ya reservados).

### Módulo Turnos "Appointments"
* **POST:** `"/api/appointments"` : Permite al paciente registrar la reserva de un turno, mediante el ingreso del ID del doctor, el ID del turno, el DNI del paciente y el motivo/razón del turno.
* **GET:** `"/api/appointments/patient"` : Permite al administrador obtener el historial de turnos activos de un determinado paciente, ingresando su DNI.
* **DELETE:** `"/api/appointments/{id}"` : Permite al paciente cancelar un turno reservado, mediante el ingreso del ID del turno.
* **GET:** `"/api/appointments/search"` : Permite al administrador realizar búsquedas combinadas avanzadas de turnos, ingresando según lo que necesite: DNI, fecha, médico o especialidad.

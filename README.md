# Analizador.Console

Analizador.Console es una herramienta de línea de comandos diseñada para resolver las instrucciones correspondientes a diferencias detectadas entre dos binarios. Utiliza la dirección de las diferencias y el archivo lst correspondiente para identificar y analizar los cambios.

## Características
- Detecta y procesa diferencias entre dos binarios a partir de un archivo de direcciones (diff) y un archivo lst.
- Identifica la instrucción asociada a cada diferencia usando el archivo lst.
- Genera un archivo de salida con las instrucciones relevantes para cada diferencia.
- Publicación como ejecutable single file para Windows x64.
- Incluye pruebas unitarias con xUnit.

## Requisitos
- .NET 8.0 SDK
- Windows x64 (para el ejecutable single file)

## Compilación y publicación

Para compilar y publicar el ejecutable single file:

```sh
# Desde la raíz del proyecto
 dotnet publish Analizador.Console/Analizador.Console.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

El ejecutable estará en:
```
Analizador.Console/bin/Release/net8.0/win-x64/publish/Analizador.Console.exe
```

## Ejecución

Ejecuta el programa desde la terminal o desde una tarea de VS Code:

```sh
Analizador.Console.exe <diffFile> <listFile> <outputFile>
```
- `diffFile`: Archivo con las direcciones de las diferencias detectadas entre los binarios (formato hexadecimal).
- `listFile`: Archivo lst generado por el ensamblador, que contiene el mapeo de direcciones a instrucciones.
- `outputFile`: Archivo donde se guardarán las instrucciones asociadas a cada diferencia.

## Testing

Para ejecutar los tests unitarios:

```sh
dotnet test Analizador.Tests/Analizador.Tests.csproj
```

## Estructura del proyecto
- `Analizador.Console/` - Código fuente principal
- `Analizador.Tests/` - Pruebas unitarias (xUnit)
- `.vscode/tasks.json` - Tareas para build y ejecución en VS Code

## Licencia
Este proyecto se distribuye bajo la licencia MIT.

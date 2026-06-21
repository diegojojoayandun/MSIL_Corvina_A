# MSIL Corvina A — Prototipo de Ransomware con Fines Académicos

> **Trabajo de Tesis — Carrera de Informática**
> Prototipo desarrollado con fines estrictamente educativos para el análisis de técnicas de amenazas persistentes avanzadas (APT) y el estudio de mecanismos de defensa en entornos Windows.

---

## Descripción General

MSIL Corvina A es un prototipo funcional de ransomware desarrollado en **VB.NET** sobre el framework **.NET Framework 4.x** como objeto de investigación académica. El proyecto implementa los componentes técnicos característicos de familias reales de ransomware, incluyendo cifrado de archivos, ofuscación de cadenas en tiempo de ejecución, exfiltración por FTP, comunicación con servidor de comando y control (C2), y mecanismos de evasión de análisis estático y dinámico.

El binario generado adopta el nombre `ctfmon.exe`, emulando el proceso legítimo del Input Method Editor (IME) de Windows, como técnica de camuflaje a nivel de nombre de proceso.

---

## Arquitectura del Sistema

```
┌─────────────────────────────────────────────────────────┐
│                        mMain.vb                         │
│           Módulo principal — orquestación               │
│   CreatePass · GetHwId · Obf · RunPS · MutualExclusion  │
└────────┬────────────┬───────────────┬───────────────────┘
         │            │               │
    ┌────▼────┐  ┌────▼────┐   ┌─────▼─────┐
    │clsSearch│  │clsCrypto│   │  clsPhp   │
    │ScanDrives│ │Encrypt  │   │SendToC2   │
    │ScanDirs  │ │GetKey   │   └─────┬─────┘
    └────┬─────┘ └─────────┘         │
         │                     ┌─────▼─────┐
    ┌────▼─────┐               │  clsSet   │
    │ clsFTP   │               │ (Config)  │
    │UploadFile│               └─────┬─────┘
    └──────────┘                     │
                          ┌──────────┼──────────┐
                     ┌────▼───┐ ┌───▼───┐ ┌────▼────┐
                     │cls3DES │ │clsGeo │ │clsNetwr │
                     │Obf/Des │ │IP     │ │Shares   │
                     └────────┘ └───────┘ └─────────┘
```

---

## Módulos y Responsabilidades

| Archivo | Clase / Módulo | Responsabilidad |
|---|---|---|
| `mMain.vb` | `Module mMain` | Orquestación general: inicialización, derivación de claves, ejecución PowerShell, mutex de instancia única, punto de entrada `Sub main()` |
| `clsSet.vb` | `clsS` | Clase de configuración centralizada: rutas, extensiones objetivo, carpetas excluidas, parámetros de cifrado, cadena de consulta al C2 |
| `clsSearch.vb` | `clsSearch` | Escaneo iterativo de unidades lógicas y directorios mediante pila explícita (evita recursión profunda); filtrado por extensión y tamaño |
| `clsCrypto.vb` | `clsCrypto` | Cifrado Rijndael-256 de archivos; derivación de clave mediante SHA-512; renombrado con identificador de víctima |
| `cls3DES.vb` | `cls3DES` | Cifrado/descifrado 3DES-CBC para ofuscación de literales de cadena en tiempo de compilación |
| `clsFTP.vb` | `clsFTP` | Exfiltración de archivos cifrados al servidor FTP del atacante; creación de directorio por víctima |
| `clsPhp.vb` | `clsPhp` | Comunicación HTTP POST con el servidor C2; envío de metadatos de la víctima (máquina, usuario, HDD, geolocalización, claves) |
| `clsGeoIP.vb` | `clsGeoIP` | Obtención de IP externa y resolución de país/ISP mediante API de geolocalización (HTTPS) |
| `clsNetwork.vb` | `clsNetwork` | Enumeración de carpetas compartidas locales vía `NetShareEnum` (Win32 API); copia del ejecutable para propagación lateral |

---

## Diseño Criptográfico

### Generación de Claves

```
HDD Serial (C:)
     │
     └─► MD5 ──────────────────────────────► clsS.hdd  (identificador persistente)
     │
     └─► MD5(serial) + RNG(24 chars) ──────► SHA-512 ──► bytPK[0..31]  (clave primaria, 256 bits)

RNGCryptoServiceProvider(30 chars) ────────► SHA-512 ──► bytSK[0..15]  (clave secundaria / IV, 128 bits)
```

- **Clave primaria (`sPrimaryKey`):** derivada del serial del disco duro mezclado con 24 caracteres generados por `RNGCryptoServiceProvider`. Es única por máquina y no reproducible sin el valor aleatorio original.
- **Clave secundaria (`sSecondaryKey`):** 30 caracteres generados con `RNGCryptoServiceProvider`. Actúa como vector de inicialización (IV) del cifrado Rijndael.
- **Derivación final:** ambas claves pasan por `SHA-512` y se truncan al tamaño requerido por el algoritmo (32 y 16 bytes respectivamente) mediante `clsCrypto.GetKey`.

### Cifrado de Archivos (`clsCrypto.Encrypt`)

```
Archivo original
     │
     ▼
File.ReadAllBytes()
     │
     ▼
RijndaelManaged(Key=bytPK, IV=bytSK) [modo CBC]
     │
     ▼
Archivo cifrado  →  sobreescribe el original
     │
     ▼
Copia renombrada:  nombre_original[ID=<hdd_md5>].extensión_cifrada
     │
     ▼
File.Delete(original)
```

### Ofuscación de Cadenas (`cls3DES` + `Obf`)

Todas las cadenas sensibles (URLs, extensiones, nombres de carpetas excluidas, comandos PowerShell) están cifradas con **3DES-CBC** en tiempo de compilación. La función `Obf(base64)` las descifra en tiempo de ejecución usando una clave derivada de una cadena codificada en Base64 interna al binario, dificultando el análisis estático del ejecutable.

---

## Técnicas de Evasión y Persistencia

### Detección de Entorno de Análisis
El módulo verifica si el nombre de máquina (`$env:COMPUTERNAME`) pertenece a una lista de hosts conocidos de sandboxes y servicios de análisis automático (VirusTotal, etc.). Si se detecta un entorno controlado, el ejecutable finaliza sin realizar ninguna acción.

### Instancia Única (Mutex)
Se crea un mutex nombrado con el identificador único de la víctima (`GetHwId()`). Si el mutex ya existe, el proceso termina, evitando cifrados dobles.

### Persistencia en Inicio de Windows
Se crea un acceso directo (`.lnk`) en la carpeta `%AppData%\Microsoft\Windows\Start Menu\Programs\Startup` apuntando al ejecutable, garantizando la ejecución en cada inicio de sesión hasta completar el cifrado.

### Auto-eliminación por PowerShell
Tras completar el cifrado, se ejecuta un script PowerShell que sobreescribe el binario byte a byte con ceros antes de eliminarlo, dificultando la recuperación forense del ejecutable.

### Eliminación de Puntos de Restauración
Mediante WMI (`SystemRestore.SRRemoveRestorePoint`) se eliminan los puntos de restauración del sistema, bloqueando la recuperación del sistema operativo a un estado previo a la infección.

---

## Propagación en Red

`clsNetwork.ScanNetwork()` enumera las carpetas compartidas de la máquina local mediante la API nativa `NetShareEnum` de `netapi32.dll` con nivel de información 2 (SHARE\_INFO\_2), con fallback a nivel 1 si el acceso es denegado. Por cada carpeta compartida de tipo disco (`STYPE_DISKTREE`), copia el ejecutable con un nombre aleatorio compuesto por nombre + extensión ofuscados, facilitando la infección de máquinas con acceso a dichos recursos.

---

## Comunicación con el Servidor C2

El módulo `clsPhp` realiza un HTTP POST al servidor de control con la siguiente cadena de parámetros:

```
usuario=<MachineName>|<UserName>|<HDD_MD5>|<CountryCode>|<IP>|<Fecha>
&llave1=<PrimaryKey>
&llave2=<SecondaryKey>
```

Las dos URLs de C2 configuradas en `clsS.sUrl` son redundantes (failover): se intenta la primera y, si no responde, se usa la segunda. Las claves de descifrado se almacenan en el servidor del atacante, siendo el único medio de recuperación de los archivos cifrados.

---

## Requisitos de Compilación

| Componente | Versión |
|---|---|
| IDE | Visual Studio 2013 / 2015 |
| Lenguaje | VB.NET |
| Framework objetivo | .NET Framework 4.0+ |
| Sistema operativo objetivo | Windows Vista / 7 / 8 / 10 (x86/x64) |
| Referencias adicionales | `System.Management`, `System.Web`, `System.Runtime.InteropServices` |

### Compilación (Release)
```
msbuild winexec\ctfmon.vbproj /p:Configuration=Release
```

En modo `DEBUG`, la lista de extensiones objetivo se reemplaza por `{".nocry"}` (definición de compilación condicional en `clsS`), limitando la acción del prototipo a archivos con esa extensión ficticia para pruebas seguras.

---

## Consideraciones de Seguridad Identificadas

Durante el desarrollo se documentaron las siguientes limitaciones criptográficas y de diseño:

1. **Clave derivada de hardware:** la clave primaria incluye el serial del disco duro, lo que la hace determinista si el serial es conocido por el atacante. Un esquema más robusto emplearía intercambio de claves asimétrico (RSA / ECDH) con la clave pública del atacante embebida en el binario.
2. **Transmisión de claves en claro:** las claves se envían al C2 por HTTP. En un escenario real, el canal debería estar protegido por TLS y las claves deberían cifrarse asimétricamente antes de la transmisión.
3. **MD5 como identificador:** el hash MD5 del serial del disco se usa como ID de víctima. MD5 es criptográficamente roto; SHA-256 sería el mínimo aceptable.
4. **Geolocalización sobre HTTP:** corregido en esta versión; las peticiones a las APIs de IP se realizan ahora sobre HTTPS.

---

## Declaración Ética y Legal

Este software fue desarrollado **exclusivamente con fines académicos e investigativos** en el contexto de un trabajo de tesis universitaria. Su propósito es demostrar el funcionamiento interno de una clase de malware para contribuir a la comprensión de las técnicas de ataque y al diseño de contramedidas efectivas.

**Queda estrictamente prohibido:**
- Ejecutar este software sobre sistemas sin autorización expresa y por escrito de su propietario.
- Utilizarlo con fines delictivos, extorsivos o de cualquier naturaleza que cause perjuicio a terceros.
- Distribuirlo fuera de contextos académicos o de investigación de seguridad controlada.

El uso indebido de este software puede constituir un delito tipificado en las legislaciones nacionales sobre delitos informáticos y el Convenio de Budapest sobre Ciberdelincuencia.

> **El autor y la institución académica no se responsabilizan del uso que terceros puedan hacer de este código.**

---

## Licencia

Uso libre para fines académicos y de investigación en seguridad informática. Redistribución bajo responsabilidad exclusiva del receptor.

# 🔨 Build & Deployment Guide

Anleitung zum Bauen und Bereitstellen des Troybin Editors.

## ✅ Voraussetzungen

- **Betriebssystem**: Windows 7 oder höher
- **.NET SDK**: .NET 10 oder höher
- **IDE** (optional): Visual Studio, Rider, oder VS Code
- **Git**: Für Versionskontrolle

### Installation von .NET 10

```bash
# Besuche https://dotnet.microsoft.com/en-us/download/dotnet/10.0
# Laden Sie den Windows Installer herunter und installieren Sie
```

### Überprüfung der Installation

```bash
dotnet --version
# Sollte .NET 10.x.x anzeigen
```

---

## 🏗️ Projekt Bauen

### Option 1: Debug Build (für Entwicklung)

```bash
# Navigation
cd C:\Users\theki\RiderProjects\TroybinEditor\TroybinEditor

# Abhängigkeiten wiederherstellen
dotnet restore

# Debug Build erstellen
dotnet build

# Ausgabe:
# ...
# Erstellen von Erfolgreich in X.Xs
```

Die ausführbare Datei befindet sich dann hier:
```
TroybinEditor\bin\Debug\net10.0-windows\TroybinEditor.exe
```

### Option 2: Release Build (für Produktion)

```bash
# Release Build mit Optimierungen
dotnet build --configuration Release

# Ausgabe:
# ...
# Erstellen von Erfolgreich in X.Xs
```

Die optimierte ausführbare Datei befindet sich hier:
```
TroybinEditor\bin\Release\net10.0-windows\TroybinEditor.exe
```

### Unterschiede Debug vs Release

| Aspekt | Debug | Release |
|--------|-------|---------|
| Optimierung | Keine | Vollständig |
| Dateigröße | Größer | Kleinerer |
| Debug-Info | Ja | Nein |
| Performance | Langsamer | Schneller |
| Startzeit | Normal | Schneller |
| Speichernutzung | Höher | Niedriger |

---

## 🚀 Anwendung Ausführen

### Option 1: Mit dotnet CLI

```bash
cd TroybinEditor
dotnet run
```

### Option 2: Mit IDE

#### Visual Studio
1. Öffne `TroybinEditor.sln`
2. Drücke `F5` oder klicke auf "Run"

#### JetBrains Rider
1. Öffne `TroybinEditor.sln`
2. Drücke `F5` oder klicke auf "Run"

#### VS Code
1. Öffne den Projektordner
2. Installiere C# Dev Kit Extension
3. Drücke `F5` zum Starten

### Option 3: Direkte EXE-Ausführung

```bash
# Debug
.\TroybinEditor\bin\Debug\net10.0-windows\TroybinEditor.exe

# Release
.\TroybinEditor\bin\Release\net10.0-windows\TroybinEditor.exe
```

---

## 📦 Veröffentlichung (Publishing)

### Self-Contained Executable (Empfohlen)

Diese Methode erstellt eine eigenständige EXE, die keine .NET-Installation benötigt.

```bash
# 64-bit Version (empfohlen)
dotnet publish -c Release -r win-x64 --self-contained -p:SelfContainedSingleFile=true

# 32-bit Version (falls benötigt)
dotnet publish -c Release -r win-x86 --self-contained -p:SelfContainedSingleFile=true
```

**Ausgabepfad:**
```
TroybinEditor\bin\Release\net10.0-windows\win-x64\TroybinEditor.exe
```

**Größe:** ~150-200 MB (inkl. .NET Runtime)
**Vorteil:** Funktioniert auf jedem Windows-System ohne .NET-Installation

### Framework-Dependent Executable

Diese Methode erfordert, dass .NET 10 auf dem Zielcomputer installiert ist.

```bash
# Abhängig vom installierten Framework
dotnet publish -c Release -r win-x64
```

**Ausgabepfad:**
```
TroybinEditor\bin\Release\net10.0-windows\win-x64\TroybinEditor.exe
```

**Größe:** ~5-10 MB
**Vorteil:** Kleinere Größe
**Nachteil:** Benötigt .NET 10 Installation

---

## 🐛 Clean Build (bei Problemen)

Falls das Projekt nicht kompiliert:

```bash
# Alle erzeugten Dateien löschen
dotnet clean

# Abhängigkeiten neu laden
dotnet restore

# Neu bauen
dotnet build
```

Oder manuell:
```bash
# Verzeichnisse löschen
rmdir /s bin
rmdir /s obj

# Abhängigkeiten neu laden
dotnet restore

# Bauen
dotnet build
```

---

## 🧪 Testen nach dem Build

### 1. Applikation starten

```bash
dotnet run
```

### 2. Funktionen testen

- [ ] App startet ohne Fehler
- [ ] Fenster wird angezeigt
- [ ] Datei > Öffnen funktioniert
- [ ] Sample.troybin lädt
- [ ] Partikel werden angezeigt
- [ ] Properties können bearbeitet werden
- [ ] Speichern funktioniert
- [ ] Status-Meldung wird angezeigt

### 3. Fehlerprüfung

Achte auf:
- Konsolen-Fehler
- Exception-Meldungen
- Performance-Probleme

---

## 📊 Build-Konfigurationen

### Debug Configuration

```xml
<PropertyGroup>
  <Configuration>Debug</Configuration>
  <DebugType>full</DebugType>
  <Optimize>false</Optimize>
</PropertyGroup>
```

**Verwenden für:**
- Entwicklung
- Debugging
- Testing

### Release Configuration

```xml
<PropertyGroup>
  <Configuration>Release</Configuration>
  <DebugType>embedded</DebugType>
  <Optimize>true</Optimize>
</PropertyGroup>
```

**Verwenden für:**
- Produktion
- Distribution
- Performance-Tests

---

## 🔄 Automatisierte Builds (CI/CD)

### GitHub Actions Beispiel

```yaml
name: Build & Test

on: [push, pull_request]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '10.0.x'
    
    - name: Restore
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release
    
    - name: Publish
      run: |
        dotnet publish -c Release -r win-x64 \
          --self-contained -p:SelfContainedSingleFile=true
```

---

## 📋 Troubleshooting

### Problem: "dotnet command not found"

**Lösung:**
```bash
# .NET installieren
# https://dotnet.microsoft.com/download/dotnet/10.0

# Oder PATH überprüfen
echo $env:PATH
```

### Problem: "The project file was not found"

**Lösung:**
```bash
# Zum richtigen Verzeichnis navigieren
cd C:\Users\theki\RiderProjects\TroybinEditor\TroybinEditor

# oder
cd TroybinEditor
```

### Problem: "error NU1101: Unable to find package"

**Lösung:**
```bash
# NuGet Cache löschen
dotnet nuget locals all --clear

# Wieder versuchen
dotnet restore
```

### Problem: "error MSB4025: The project file could not be loaded"

**Lösung:**
```bash
# Clean Build
dotnet clean
dotnet restore
dotnet build
```

### Problem: "Build fails with XAML errors"

**Lösung:**
```bash
# IDE neustarten oder
# Manual XAML validation
# Überprüfen Sie MainWindow.xaml auf Syntaxfehler
```

---

## 📈 Performance Optimization

### Build-Speedup

```bash
# Multi-threaded build
dotnet build -m

# Skip restore
dotnet build --no-restore

# Increment
dotnet build --configuration Release -m
```

### Runtime Optimization

```xml
<!-- Im .csproj hinzufügen -->
<PropertyGroup>
  <PublishReadyToRun>true</PublishReadyToRun>
  <PublishTrimmed>true</PublishTrimmed>
</PropertyGroup>
```

---

## 🎯 Release Checklist

Vor der Veröffentlichung:

- [ ] Alle Tests bestanden
- [ ] Dokumentation aktuell
- [ ] Version in CHANGELOG.md aktualisiert
- [ ] README.md überprüft
- [ ] Build ohne Warnings
- [ ] Release Build erstellt
- [ ] Self-contained EXE getestet
- [ ] Performance geprüft
- [ ] Security reviewed
- [ ] Tag in Git erstellt

```bash
# Tag erstellen (z.B. für Version 1.0.0)
git tag -a v1.0.0 -m "Release version 1.0.0"
git push origin v1.0.0
```

---

## 🚀 Distribution

### Methode 1: Einzelne EXE

```bash
# Self-contained Publish
dotnet publish -c Release -r win-x64 \
  --self-contained -p:SelfContainedSingleFile=true

# Kopiere
copy bin\Release\net10.0-windows\win-x64\TroybinEditor.exe \
  ...\Release\TroybinEditor.exe
```

### Methode 2: ZIP-Archiv

```bash
# Publish in Verzeichnis
dotnet publish -c Release -r win-x64 --self-contained

# ZIP erstellen
Compress-Archive -Path bin\Release\net10.0-windows\win-x64\* \
  -DestinationPath TroybinEditor-v1.0.0.zip
```

### Methode 3: Installer (NSIS/WiX)

Für professionelle Installation (weiterführend).

---

## 📝 Build-Dokumentation

### Generiere Dokumentation

```bash
# XML-Dokumentation erzeugen
dotnet build /p:GenerateDocumentationFile=true
```

Die Dokumentation befindet sich in:
```
TroybinEditor\bin\Debug\net10.0-windows\TroybinEditor.xml
```

---

## 🔗 Weiterführende Ressourcen

- [.NET CLI Documentation](https://learn.microsoft.com/en-us/dotnet/core/tools/)
- [Publishing .NET Apps](https://learn.microsoft.com/en-us/dotnet/core/deploying/)
- [WPF Deployment](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/deployment/)

---

## 📞 Hilfe bei Build-Problemen

1. Lies das [TROUBLESHOOTING](#-troubleshooting) Kapitel
2. Versuche einen [Clean Build](#-clean-build-bei-problemen)
3. Überprüfe die [RESOURCES.md](./RESOURCES.md)
4. Erstelle ein GitHub Issue

---

**Version**: 1.0.0
**Datum**: 2026-02-19
**Status**: ✅ Aktuell


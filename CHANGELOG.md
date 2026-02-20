# Changelog

Alle wichtigen Änderungen an diesem Projekt werden in dieser Datei dokumentiert.

## [1.0.0] - 2026-02-19

### ✨ Features (Initiale Version)

#### Benutzeroberfläche
- ✅ Modernes Dark Theme mit Blau-Akzenten
- ✅ Responsive 3-spalten Layout (Partikelliste | Vorschau | Properties)
- ✅ Intuitive Navigation und Bedienung
- ✅ Status-Leiste mit Live-Feedback
- ✅ Custom WPF Styling (Buttons, TextBoxes, CheckBoxes, ListBoxes)

#### Dateioperationen
- ✅ Öffnen von .troybin Dateien
- ✅ Speichern von Änderungen
- ✅ "Speichern unter" Funktionalität
- ✅ Automatische Backup-Erstellung
- ✅ Datei-Validierung

#### Partikel-Management
- ✅ Partikel-Liste im linken Bereich
- ✅ Schnelle Vorschau der Eigenschaften
- ✅ Erstellen neuer Partikel
- ✅ Löschen von Partikeln
- ✅ Auswahl und Anzeige von Partikel-Informationen

#### Properties-Editor
- ✅ Name-Bearbeitung
- ✅ Enabled/Disabled Status
- ✅ Duration-Einstellung
- ✅ Position (X, Y, Z)
- ✅ Skalierung (X, Y, Z)
- ✅ Farbe (RGBA Kanäle)
- ✅ Echtzeit-Updates

#### Services & Utilities
- ✅ TroybinFileService - Datei I/O
- ✅ FileDialogService - Datei-Dialoge
- ✅ MessageService - Benutzer-Benachrichtigungen
- ✅ SettingsService - Anwendungs-Einstellungen
- ✅ RecentFilesService - Zuletzt geöffnete Dateien
- ✅ ValidationHelper - Validierungs-Utilities
- ✅ ColorHelper - Farb-Konvertierungen

#### Architektur
- ✅ MVVM Pattern mit CommunityToolkit.Mvvm
- ✅ Saubere Separation of Concerns
- ✅ Erweiterbare Service-Architektur
- ✅ Typsicherheit mit Nullable-Referenzen

### 🎨 Design

- Dunkles Theme für längere Arbeitssitzungen
- Primärfarbe: Blau (#3B82F6)
- Hintergrund: Sehr dunkles Blau (#0F172A)
- Text: Helles Blau-Weiß (#F1F5F9)
- Abgerundete Ecken und moderne Styling

### 🛠️ Technologie

- **Framework**: WPF (Windows Presentation Foundation)
- **Language**: C# (.NET 10)
- **MVVM Toolkit**: CommunityToolkit.Mvvm 8.2.2
- **IDE**: JetBrains Rider / Visual Studio

### 📖 Dokumentation

- ✅ README.md - Überblick
- ✅ GETTING_STARTED.md - Anleitung
- ✅ CHANGELOG.md - Diese Datei
- ✅ Inline-Kommentare im Code
- ✅ XML-Dokumentation

### 🚀 Geplante Features (Zukünftig)

- [ ] Undo/Redo Funktionalität
- [ ] 3D-Vorschau der Partikel
- [ ] Eigenschafts-Datentyp-Validierung
- [ ] Animation Preview
- [ ] Batch-Operationen
- [ ] Mehrsprachige UI (DE, EN, FR)
- [ ] Keyboard-Shortcuts
- [ ] Themes (Light/Dark Mode)
- [ ] Exportfunktion (JSON, XML)
- [ ] Drag & Drop für Dateien

### 🐛 Bekannte Probleme

- Keine bekannten Probleme in Version 1.0.0

### 📋 Kompatibilität

- **Betriebssystem**: Windows 7, 8, 10, 11+
- **.NET**: .NET 10+
- **League of Legends**: Alle Versionen (für Dateien kompatibel)

---

## Versionshistorie

### Versionierungsschema

Dieses Projekt folgt [Semantic Versioning](https://semver.org/):
- **MAJOR** Version für inkompatible API-Änderungen
- **MINOR** Version für neue Funktionalität (rückwärts kompatibel)
- **PATCH** Version für Bugfixes

### Versionsformat

`[VERSION] - YYYY-MM-DD`

---

## Kontribuieren

Wenn Sie einen Bug gefunden haben oder ein Feature vorschlagen möchten:
1. Erstellen Sie ein GitHub Issue
2. Geben Sie eine klare Beschreibung
3. Geben Sie Schritte zur Reproduktion an
4. Schreiben Sie Code-Änderungen

---

**Letzte Aktualisierung**: 2026-02-19


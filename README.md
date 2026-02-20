# Troybin Editor - League of Legends Particle File Editor

Ein moderner, schöner Editor für League of Legends Troybin Partikeldateien (`.troybin`). Der Editor bietet eine intuitive Oberfläche mit Echtzeit-Bearbeitung von Partikel-Eigenschaften.

## Features

✨ **Moderne, dunkle Benutzeroberfläche**
- Sleek Dark Theme mit Blau-Akzenten
- Responsive 3-spalten Layout
- Modern WPF mit MVVM-Architektur

📂 **Dateioperationen**
- Öffnen von `.troybin` Dateien
- Speichern von Änderungen
- "Speichern unter" Funktionalität
- Automatische Backup-Erstellung beim Speichern

🎨 **Partikel-Management**
- Übersichtliche Partikelliste im linken Bereich
- Schnelle Vorschau der Partikel-Eigenschaften
- Erstellen neuer Partikel
- Löschen von Partikeln

⚙️ **Eigenschaften-Editor**
- **Allgemein**: Aktivierter Status, Dauer
- **Position**: X, Y, Z Koordinaten
- **Skalierung**: X, Y, Z Skalen-Werte
- **Farbe**: RGBA-Kanäle mit visueller Vorschau
- Echtzeit-Updates während der Bearbeitung

## Architektur

```
TroybinEditor/
├── Models/                 # Datenmodelle
│   ├── ParticleData.cs    # Partikel-Struktur
│   └── TroybinDocument.cs # Dokument-Container
├── ViewModels/            # MVVM ViewModels
│   ├── MainWindowViewModel.cs
│   └── ParticleViewModel.cs
├── Services/              # Business Logic
│   ├── ITroybinFileService.cs
│   ├── TroybinFileService.cs
│   ├── IFileDialogService.cs
│   └── FileDialogService.cs
├── Views/                 # UI Components
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
├── Themes/               # UI Styling
│   ├── Colors.xaml       # Farb-Palette
│   └── Styles.xaml       # Control Styles
└── Converters/           # Value Converters
    ├── NullToVisibilityConverter.cs
    └── RgbToColorConverter.cs
```

## Installation & Nutzung

### Voraussetzungen
- .NET 10 oder höher
- Windows 7 oder später

### Ausführen

```bash
# Repository klonen
git clone https://github.com/yourusername/TroybinEditor.git
cd TroybinEditor

# Mit dotnet ausführen
dotnet run

# Oder bauen und ausführen
dotnet build
dotnet run
```

### Bedienung

1. **Datei öffnen**: `Datei > Öffnen` oder Button "📁 Open"
2. **Partikel wählen**: Klick auf einen Partikel in der linken Liste
3. **Eigenschaften bearbeiten**: Werte in den rechten Property-Feldern ändern
4. **Neuen Partikel erstellen**: Button "➕ New Particle" oder `Bearbeiten > Neuer Partikel`
5. **Änderungen speichern**: Ctrl+S oder `Datei > Speichern`

## Technologie-Stack

- **Framework**: WPF (Windows Presentation Foundation)
- **Language**: C# (.NET 10)
- **MVVM Toolkit**: CommunityToolkit.Mvvm 8.2.2
- **Pattern**: MVVM (Model-View-ViewModel)
- **UI Theme**: Modern Dark Mode mit Custom Styles

## Geplante Features

- 🔄 Undo/Redo Funktionalität
- 👁️ 3D-Vorschau der Partikel
- 📊 Eigenschafts-Datentyp-Validierung
- 🎞️ Animation Preview
- 📋 Batch-Operationen
- 🌍 Mehrsprachige UI

## Styling & Customization

Das Styling basiert auf modernem Design mit einem dunkelen Theme:

- **Primärfarbe**: Blau (#3B82F6)
- **Hintergrund**: Sehr dunkles Blau (#0F172A)
- **Oberflächen**: Dunkles Schieferblau (#1E293B)
- **Text**: Helles Blau-Weiß (#F1F5F9)

Um das Theme zu ändern, bearbeite `Themes/Colors.xaml`.

## Lizenz

MIT License - Frei nutzbar für eigene Projekte

## Kontakt & Support

Bei Fragen oder Problemen, bitte ein Issue erstellen.

---

**Hinweis**: Dieses Projekt ist ein Community-Editor und ist nicht offiziell mit Riot Games oder League of Legends verbunden.


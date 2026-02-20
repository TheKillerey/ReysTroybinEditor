# Developer Guide - Troybin Editor

## Projektstruktur

```
TroybinEditor/
├── Models/                    # Datenmodelle
│   ├── ParticleData.cs       # Partikel-Struktur
│   └── TroybinDocument.cs    # Dokument-Container
│
├── ViewModels/               # MVVM ViewModels
│   ├── MainWindowViewModel.cs
│   └── ParticleViewModel.cs
│
├── Services/                 # Business Logic Layer
│   ├── ITroybinFileService.cs
│   ├── TroybinFileService.cs
│   ├── IFileDialogService.cs
│   ├── FileDialogService.cs
│   ├── IMessageService.cs
│   ├── MessageService.cs
│   ├── SettingsService.cs
│   └── RecentFilesService.cs
│
├── Views/                    # UI Components (XAML)
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
│
├── Themes/                   # UI Styling
│   ├── Colors.xaml          # Farb-Palette
│   └── Styles.xaml          # Control Styles
│
├── Converters/              # WPF Value Converters
│   ├── NullToVisibilityConverter.cs
│   └── RgbToColorConverter.cs
│
├── Utilities/               # Hilfs-Funktionen
│   ├── ValidationHelper.cs
│   └── ColorHelper.cs
│
├── App.xaml                 # Application XAML
├── App.xaml.cs              # Application Code-Behind
└── TroybinEditor.csproj     # Project File
```

## Architektur-Pattern

### MVVM (Model-View-ViewModel)

```
View (XAML)
    ↓ Bindings
ViewModel (Logic, Commands)
    ↓ References
Model (Data, Services)
```

### Schichten-Architektur

1. **Presentation Layer** (Views, ViewModels)
   - Benutzeroberfläche
   - Benutzereingabe-Handling
   - Daten-Binding

2. **Business Logic Layer** (Services)
   - Datei-Operationen
   - Validierung
   - Konvertierungen

3. **Data Layer** (Models)
   - ParticleData
   - TroybinDocument
   - Datenstrukturen

## Komponenten erklärt

### Models

**ParticleData.cs**
- Repräsentiert ein einzelnes Partikel
- Speichert Eigenschaften als Dictionary
- Unterstützt Clone() für Deep-Copy

**TroybinDocument.cs**
- Container für eine Troybin-Datei
- Enthält Metadata und Particles
- Tracks IsModified Status

### ViewModels

**MainWindowViewModel**
- Hauptlogik der Applikation
- Commands: OpenFile, SaveFile, SaveAs, CreateNewParticle, DeleteSelectedParticle
- Collections: Particles (ObservableCollection)
- Properties: CurrentDocument, SelectedParticle, StatusMessage, IsLoading

**ParticleViewModel**
- Wrapper um ParticleData
- ObservableProperties für Binding
- Automatische Model-Synchronisation

### Services

**TroybinFileService**
- Laden und Speichern von Dateien
- Validierung
- Fehlerbehandlung

**FileDialogService**
- Dateiauswahl-Dialoge
- OpenFileDialog, SaveFileDialog

**MessageService**
- Benutzer-Benachrichtigungen
- ShowInfo, ShowWarning, ShowError, ShowQuestion

**SettingsService**
- Persistente Einstellungen
- Speicherung in AppData/TroybinEditor

**RecentFilesService**
- Track zuletzt geöffneter Dateien
- Max 10 Dateien

## Datenfluss

```
User Action
    ↓
Command in ViewModel
    ↓
Service (TroybinFileService, FileDialogService, etc.)
    ↓
Model (ParticleData, TroybinDocument)
    ↓
Update ViewModel Properties
    ↓
UI Update (via Binding)
```

## Erweiterungen hinzufügen

### Neuen Service hinzufügen

1. Erstelle ein Interface `IMyService.cs`
2. Implementiere `MyService.cs`
3. Registriere in `MainWindowViewModel` Constructor
4. Verwende via Dependency Injection

Beispiel:
```csharp
public interface IMyService 
{
    void DoSomething();
}

public class MyService : IMyService 
{
    public void DoSomething() { }
}

// In MainWindowViewModel:
private readonly IMyService _myService;

public MainWindowViewModel() 
{
    _myService = new MyService();
}
```

### Neues ViewModel hinzufügen

1. Erbe von `ObservableObject`
2. Verwende `[ObservableProperty]` für automatische Eigenschaften
3. Nutze `[RelayCommand]` für Commands
4. Binde im entsprechenden View

### Neues UI-Element hinzufügen

1. Erstelle XAML in `Views/` Verzeichnis
2. Definiere Styles in `Themes/Styles.xaml`
3. Binde ViewModel in XAML: `DataContext="{Binding MyViewModel}"`
4. Verwende Bindings: `Text="{Binding MyProperty}"`

## MVVM Toolkit Features

### ObservableProperty

```csharp
[ObservableProperty]
private string name;

// Wird automatisch zu:
// public string Name { get; set; }
// mit PropertyChanged Events
```

### RelayCommand

```csharp
[RelayCommand]
public async Task MyCommand()
{
    // Wird automatisch zu:
    // public IAsyncRelayCommand MyCommandCommand { get; }
}

// In XAML:
// <Button Command="{Binding MyCommandCommand}" />
```

## Styling & Themes

### Farb-System

Alle Farben sind in `Themes/Colors.xaml` definiert:
- PrimaryBrush (#3B82F6) - Blau
- ErrorBrush (#EF4444) - Rot
- SuccessBrush (#10B981) - Grün

Um Farben zu ändern:
1. Bearbeite `Themes/Colors.xaml`
2. Alle Views aktualisieren sich automatisch

### Custom Styles

In `Themes/Styles.xaml` definiert:
- ModernButtonStyle
- ModernTextBoxStyle
- ModernCheckBoxStyle
- ModernGroupBoxStyle
- ModernListBoxStyle

Verwendung:
```xaml
<Button Style="{StaticResource ModernButtonStyle}" />
```

## Testing

### Unit Tests (zukünftig)

```csharp
[TestClass]
public class TroybinFileServiceTests 
{
    [TestMethod]
    public async Task LoadFile_ValidPath_ReturnsDocument()
    {
        // Arrange
        var service = new TroybinFileService();
        var testFile = "path/to/test.troybin";
        
        // Act
        var result = await service.LoadFileAsync(testFile);
        
        // Assert
        Assert.IsNotNull(result);
    }
}
```

## Build & Deployment

### Debug Build
```bash
dotnet build
```

### Release Build
```bash
dotnet build --configuration Release
```

### Self-Contained Publish
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

## Performance Optimization

1. **UI Virtualisierung**: Nutze VirtualizingStackPanel für große Listen
2. **Async Operations**: Langwierige Operationen async ausführen
3. **Memory**: Nutze WeakReference für große Caches
4. **Rendering**: WPF rendert nur sichtbare Elemente

## Debugging

### Visual Studio
- F5: Start mit Debugging
- F10: Step Over
- F11: Step Into
- Ctrl+Alt+W: Watch Window

### Output Window
```csharp
System.Diagnostics.Debug.WriteLine("Debug message");
```

## Best Practices

1. ✅ MVVM Pattern befolgen
2. ✅ Services abstrahieren
3. ✅ Fehlerbehandlung implementieren
4. ✅ Comments für komplexe Logik
5. ✅ Async/await verwenden
6. ✅ Nullable checks durchführen
7. ✅ Dispose Patterns implementieren

## Bekannte Limitierungen

- MaxRecentFiles = 10
- Max 256 Partikel pro Datei (kann erhöht werden)
- Keine echte 3D-Vorschau
- Keine Undo/Redo

## Kontakt für Entwickler

Für Fragen zum Code:
- Erstelle ein Issue auf GitHub
- Schreibe aussagekräftige Comments
- Verwende aussagekräftige Commit-Messages

---

**Letzte Aktualisierung**: 2026-02-19


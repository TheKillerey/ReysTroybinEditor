# Troybin Editor – League of Legends Particle File Editor

A modern, polished editor for League of Legends Troybin particle files (`.troybin`). The editor provides an intuitive interface with real-time editing of particle properties.

## Features

✨ **Modern, dark user interface**

* Sleek dark theme with blue accents
* Responsive 3-column layout
* Modern WPF with MVVM architecture

📂 **File operations**

* Open `.troybin` files
* Save changes
* “Save As” functionality
* Automatic backup creation when saving

🎨 **Particle management**

* Clear particle list on the left
* Quick preview of particle properties
* Create new particles
* Delete particles

⚙️ **Properties editor**

* **General**: enabled state, duration
* **Position**: X, Y, Z coordinates
* **Scale**: X, Y, Z scale values
* **Color**: RGBA channels with visual preview
* Real-time updates while editing

## Architecture

```
TroybinEditor/
├── Models/                 # Data models
│   ├── ParticleData.cs    # Particle structure
│   └── TroybinDocument.cs # Document container
├── ViewModels/            # MVVM ViewModels
│   ├── MainWindowViewModel.cs
│   └── ParticleViewModel.cs
├── Services/              # Business logic
│   ├── ITroybinFileService.cs
│   ├── TroybinFileService.cs
│   ├── IFileDialogService.cs
│   └── FileDialogService.cs
├── Views/                 # UI components
│   ├── MainWindow.xaml
│   └── MainWindow.xaml.cs
├── Themes/               # UI styling
│   ├── Colors.xaml       # Color palette
│   └── Styles.xaml       # Control styles
└── Converters/           # Value converters
    ├── NullToVisibilityConverter.cs
    └── RgbToColorConverter.cs
```

## Installation & Usage

### Requirements

* .NET 10 or newer
* Windows 7 or later

### Running

```bash
# Clone the repository
git clone https://github.com/yourusername/TroybinEditor.git
cd TroybinEditor

# Run with dotnet
dotnet run

# Or build and run
dotnet build
dotnet run
```

### How to use

1. **Open file**: `File > Open` or the “📁 Open” button
2. **Select particle**: click a particle in the left list
3. **Edit properties**: change values in the property fields on the right
4. **Create new particle**: “➕ New Particle” button or `Edit > New Particle`
5. **Save changes**: Ctrl+S or `File > Save`

## Tech Stack

* **Framework**: WPF (Windows Presentation Foundation)
* **Language**: C# (.NET 10)
* **MVVM Toolkit**: CommunityToolkit.Mvvm 8.2.2
* **Pattern**: MVVM (Model–View–ViewModel)
* **UI Theme**: Modern dark mode with custom styles

## Planned Features

* 🔄 Undo/Redo functionality
* 👁️ 3D particle preview
* 📊 Property data type validation
* 🎞️ Animation preview
* 📋 Batch operations
* 🌍 Multi-language UI

## Styling & Customization

The styling is based on a modern design with a dark theme:

* **Primary color**: Blue (#3B82F6)
* **Background**: Very dark blue (#0F172A)
* **Surfaces**: Dark slate blue (#1E293B)
* **Text**: Light blue-white (#F1F5F9)

To change the theme, edit `Themes/Colors.xaml`.

## License

MIT License – free to use for your own projects.

## Contact & Support

If you have questions or issues, please open an issue.

---

**Note**: This project is a community editor and is not officially affiliated with Riot Games or League of Legends.

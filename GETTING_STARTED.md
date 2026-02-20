# Troybin Editor - Getting Started Guide

## Was ist eine Troybin-Datei?

Troybin ist ein Binärdateiformat, das von League of Legends für Partikeldefinitionen verwendet wird. Diese Dateien definieren visuelle Effekte wie Zauber, Fähigkeiten und andere visuelle Elemente im Spiel.

## Installation

### Anforderungen

- **Betriebssystem**: Windows 7 oder höher
- **.NET Runtime**: .NET 10 oder höher
- **Speicher**: Mindestens 100 MB freier Speicherplatz

### Installation vom Quellcode

```bash
# 1. Repository klonen
git clone https://github.com/yourusername/TroybinEditor.git

# 2. Ins Verzeichnis wechseln
cd TroybinEditor

# 3. Abhängigkeiten wiederherstellen
dotnet restore

# 4. Projekt bauen
dotnet build --configuration Release

# 5. Ausführen
dotnet run
```

### Installation vorkompiliert (wenn verfügbar)

Laden Sie einfach die `.exe` herunter und führen Sie sie aus.

## Erste Schritte

### 1. Eine Troybin-Datei öffnen

```
Datei → Öffnen
```

oder klicken Sie auf den Button "📁 Open" in der Toolbar.

### 2. Die Benutzeroberfläche verstehen

Die Benutzeroberfläche ist in 3 Hauptbereiche unterteilt:

#### 📍 **Linker Bereich - Partikelliste**
- Zeigt alle Partikel in der Datei
- Klicken Sie auf einen Partikel, um ihn zu bearbeiten
- Zeigt kurze Informationen (Name, aktiviert/deaktiviert)

#### 🎨 **Mittlerer Bereich - Vorschau**
- Zeigt eine Zusammenfassung des ausgewählten Partikels
- Wichtige Eigenschaften auf einen Blick
- "Select a particle to see details" wenn nichts ausgewählt ist

#### ⚙️ **Rechter Bereich - Properties-Editor**
- Alle Eigenschaften des ausgewählten Partikels
- Echtzeit-Bearbeitung mit sofortigem Feedback
- Gruppiert nach Kategorie (Name, General, Position, Scale, Color)

### 3. Einen Partikel bearbeiten

1. **Wählen Sie einen Partikel** aus der linken Liste
2. **Bearbeiten Sie die Eigenschaften** im rechten Panel:
   - **Name**: Der Name des Partikels
   - **Enabled**: Ob das Partikel aktiv ist
   - **Duration**: Wie lange das Partikel dauert (in Sekunden)
   - **Position**: X, Y, Z Koordinaten
   - **Scale**: Vergrößerung (X, Y, Z)
   - **Color**: RGBA Werte (0-255)

3. **Änderungen werden automatisch gespeichert** zur aktuellen Datei

### 4. Ein neues Partikel erstellen

```
Bearbeiten → Neuer Partikel
```

oder klicken Sie auf "➕ New Particle"

Ein neues Partikel mit Standard-Werten wird hinzugefügt:
- Name: "Particle_N" (N = Nummer)
- Enabled: true
- Duration: 1.0 Sekunde
- Position: (0, 0, 0)
- Scale: (1, 1, 1)
- Color: Weiß (255, 255, 255, 255)

### 5. Ein Partikel löschen

1. **Wählen Sie das Partikel** aus der Liste
2. Klicken Sie auf "🗑️ Delete" oder wählen Sie `Bearbeiten → Delete Particle`

### 6. Die Datei speichern

```
Datei → Speichern
```

oder klicken Sie auf "💾 Save" oder drücken Sie **Ctrl+S**

Alternativ können Sie auch "Speichern unter" verwenden:
```
Datei → Speichern unter...
```

## Tastenkombinationen

| Tastenkombination | Funktion |
|---|---|
| `Ctrl+O` | Datei öffnen |
| `Ctrl+S` | Datei speichern |
| `Ctrl+N` | Neues Partikel |
| `Ctrl+E` | Datei beenden |

## Tipps & Tricks

### 💡 Schnelle Bearbeitung

- **Tab-Taste**: Zwischen Eingabefeldern navigieren
- **Enter**: Änderung speichern und zum nächsten Feld gehen
- **Doppelklick**: Auf einen Partikel klicken, um schnell zu bearbeiten

### 🎨 Farben verstehen

- **R (Rot)**: 0-255
- **G (Grün)**: 0-255
- **B (Blau)**: 0-255
- **A (Alpha/Transparenz)**: 0 = Unsichtbar, 255 = Volldeckend

Beispiele:
- Rot: R=255, G=0, B=0
- Grün: R=0, G=255, B=0
- Blau: R=0, G=0, B=255
- Weiß: R=255, G=255, B=255
- Schwarz: R=0, G=0, B=0

### 📐 Position und Scale

- **Position**: Bestimmt, wo das Partikel im Raum erscheint
- **Scale**: Bestimmt die Größe des Partikels (1.0 = Normal, 2.0 = 2x größer, 0.5 = halb so groß)

### ⏱️ Duration

- Gibt an, wie lange das Partikel sichtbar/aktiv ist
- In Sekunden (z.B. 2.5 = 2,5 Sekunden)
- 0 = Kein Partikel sichtbar

## Fehlerbehebung

### Die App startet nicht

- Stellen Sie sicher, dass .NET 10 installiert ist
- Versuchen Sie, das Projekt neu zu bauen: `dotnet build`
- Überprüfen Sie die Systemanforderungen

### Datei kann nicht geöffnet werden

- Stellen Sie sicher, dass die Datei vorhanden ist
- Überprüfen Sie, dass es sich um eine gültige Troybin-Datei handelt
- Versuchen Sie, die Datei zu kopieren und umzubenennen

### Änderungen werden nicht gespeichert

- Stellen Sie sicher, dass Sie "Speichern" oder Ctrl+S drücken
- Überprüfen Sie, ob Sie Schreibberechtigung haben
- Versuchen Sie, unter einem anderen Namen zu speichern

## Häufig gestellte Fragen (FAQ)

### F: Kann ich mehrere Dateien gleichzeitig öffnen?
**A**: Derzeit nein, aber Sie können mehrere Fenster öffnen.

### F: Wo werden meine Änderungen gespeichert?
**A**: Direkt in der geöffneten Datei. Verwenden Sie "Speichern unter", um eine Kopie zu erstellen.

### F: Kann ich meine Änderungen rückgängig machen?
**A**: Derzeit gibt es noch keine Undo/Redo-Funktion, aber Sie können die Datei nicht speichern und erneut öffnen.

### F: Wie importiere ich Partikel aus einer anderen Datei?
**A**: Das ist noch nicht implementiert. Sie können Dateien manuell bearbeiten oder die Funktion später hinzufügen.

## Erweiterte Funktionen (geplant)

- 🔄 Undo/Redo
- 👁️ 3D-Vorschau
- 📊 Validierung
- 🎞️ Animation Preview
- 📋 Batch-Operationen
- 🌐 Mehrsprachige UI

## Kontakt & Support

Für Fragen oder Probleme:
1. Überprüfen Sie die FAQ
2. Erstellen Sie ein GitHub Issue
3. Konsultieren Sie die Dokumentation

## Lizenz

MIT License - Frei verwendbar

---

**Hinweis**: Dieses Projekt ist inoffiziell und nicht mit Riot Games verbunden.


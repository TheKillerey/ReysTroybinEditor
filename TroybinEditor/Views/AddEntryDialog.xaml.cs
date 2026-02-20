namespace TroybinEditor.Views;

using System.Windows;
using System.Windows.Controls;

public partial class AddEntryDialog : Window
{
    public string EntryKey   { get; private set; } = string.Empty;
    public string EntryValue { get; private set; } = string.Empty;

    // ── Template definitions ─────────────────────────────────────────
    private record EntryTemplate(string Key, string DefaultValue, string Description);

    private static readonly EntryTemplate[] Templates =
    {
        new("— Custom —",    "",             "Enter any custom key and value manually."),
        // ── Texture / Mesh ──────────────────────────────────────────
        new("p-texture",     "filename.dds", "Main particle texture file (.dds)."),
        new("p-meshtex",     "filename.dds", "Mesh texture override (.dds)."),
        new("p-normal-map",  "filename.dds", "Normal map texture (.dds)."),
        new("p-falloff-texture","filename.dds","Falloff (rim) texture (.dds)."),
        new("p-reflection-map","filename.dds","Reflection/environment map (.dds)."),
        new("p-mesh",        "filename.scb", "3D mesh file (.scb) for this emitter."),
        new("p-skeleton",    "filename.skl", "Skeleton file (.skl) used with mesh."),
        new("p-skin",        "0",            "Skin index used with the mesh."),
        // ── Blend / Render ──────────────────────────────────────────
        new("rendermode",    "Add",          "Blend/render mode: Add, Simple, Complex, Screen."),
        new("pass",          "Add",          "Alias for rendermode."),
        new("p-type",        "Sprite",       "Particle type: Sprite, Beam, Trail, Decal, Mesh."),
        new("p-uvmode",      "0",            "UV mode for the texture (0=normal, 1=strip)."),
        new("p-trailmode",   "0",            "Trail rendering mode."),
        // ── Scale ───────────────────────────────────────────────────
        new("p-scale",       "1.0",          "Uniform scale of the particle."),
        new("p-xscale",      "1.0",          "X-axis scale of the particle."),
        new("uniformscale",  "1.0",          "Uniform scale applied to the whole emitter."),
        // ── Color / Alpha ───────────────────────────────────────────
        new("p-rgba",        "1.0 1.0 1.0 1.0", "RGBA color multiplier (R G B A, each 0-1)."),
        new("p-colorscale",  "1.0",          "Scales the color intensity."),
        new("p-coloroffset", "0.0 0.0 0.0 0.0","Additive color offset (R G B A)."),
        new("p-colortype",   "Constant",     "Color interpolation type: Constant, Linear."),
        new("e-rgba",        "1.0 1.0 1.0 1.0","Emitter-level RGBA multiplier."),
        // ── Life / Rate ─────────────────────────────────────────────
        new("p-life",        "1.0",          "Particle lifetime in seconds."),
        new("e-life",        "1.0",          "Emitter lifetime in seconds."),
        new("e-rate",        "10.0",         "Emission rate (particles per second)."),
        new("e-period",      "0.0",          "Repeat period of the emitter (0 = once)."),
        new("e-timeoffset",  "0.0",          "Time offset before the emitter starts."),
        // ── Velocity / Physics ──────────────────────────────────────
        new("p-vel",         "0.0 0.0 1.0",  "Initial velocity vector (X Y Z)."),
        new("p-accel",       "0.0 0.0 0.0",  "Acceleration vector (X Y Z)."),
        new("p-drag",        "0.0",          "Drag coefficient (slows particles over time)."),
        new("p-offset",      "0.0 0.0 0.0",  "Birth position offset from emitter origin (X Y Z)."),
        new("p-vel",         "0.0 0.0 1.0",  "Birth velocity (X Y Z)."),
        // ── Animation / UV ──────────────────────────────────────────
        new("p-numframes",   "1",            "Number of animation frames in the sprite sheet."),
        new("p-frameRate",   "15",           "Animation playback frame rate (FPS)."),
        new("p-startframe",  "0",            "Starting frame index in the sprite sheet."),
        new("p-randomstartframe","1",        "Randomize starting frame (1=yes, 0=no)."),
        new("p-texdiv",      "1 1",          "Texture division columns × rows for sprite sheets."),
        new("p-uvscroll-rgb","0.0 0.0",      "UV scroll speed for RGB channels (U V per second)."),
        new("e-uvoffset",    "0.0 0.0",      "UV offset for the emitter's texture (U V)."),
        // ── Orientation / Rotation ──────────────────────────────────
        new("p-orientation", "Billboard",    "Orientation type: Billboard, WorldSpace, Velocity."),
        new("p-quadrot",     "0.0",          "Rotation angle in degrees."),
        new("p-rotvel",      "0.0",          "Angular velocity (degrees per second)."),
        new("p-local-orient","0",            "Keep particle orientation local to emitter (0/1)."),
        // ── Flags ───────────────────────────────────────────────────
        new("flag-disable-z","1",            "Disable depth write (1=yes). Useful for additive FX."),
        new("flag-groundlayer","0",          "Render particle on the ground layer (0/1)."),
        new("flag-projected","0",            "Project particle onto terrain (0/1)."),
        new("single-particle","0",           "Only spawn one particle at a time (0/1)."),
        new("p-shadow",      "0",            "Cast shadow (0=no, 1=yes)."),
        // ── Sound ───────────────────────────────────────────────────
        new("SoundOnCreate", "",             "Sound event played when the emitter is created."),
        new("SoundPersistent","0",           "Keep sound playing while emitter is alive (0/1)."),
        // ── Child particles ─────────────────────────────────────────
        new("ChildParticleName","",          "Name of a child particle system to spawn."),
        new("ChildEmitOnDeath","0",          "Spawn child when particle dies (0/1)."),
    };

    public AddEntryDialog()
    {
        InitializeComponent();

        // Populate ComboBox with "key  –  short description"
        foreach (var t in Templates)
            TemplateCombo.Items.Add(t.Key == "— Custom —" ? t.Key : $"{t.Key}  —  {t.Description.Split('.')[0]}");

        TemplateCombo.SelectedIndex = 0;
        Loaded += (_, _) => KeyBox.Focus();
    }

    private void Template_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        int idx = TemplateCombo.SelectedIndex;
        if (idx < 0 || idx >= Templates.Length) return;
        var t = Templates[idx];

        if (t.Key != "— Custom —")
        {
            KeyBox.Text   = t.Key;
            ValueBox.Text = t.DefaultValue;
        }
        DescLabel.Text = t.Description;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        var key = KeyBox.Text.Trim();
        if (string.IsNullOrEmpty(key)) { KeyBox.Focus(); return; }
        EntryKey     = key;
        EntryValue   = ValueBox.Text.Trim();
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e) => DialogResult = false;
}

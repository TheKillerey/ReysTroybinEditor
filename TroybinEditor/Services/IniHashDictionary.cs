namespace TroybinEditor.Services;

/// <summary>
/// Builds the hash→name lookup table for a specific Troybin file.
///
/// KEY INSIGHT from Leischii's Main.jsx getFixdict():
///   1. Read GroupPart0..N values from the [System] section → these are the actual emitter names
///   2. Build hashes for each emitter's fields using the EMITTER NAME as the section
///   3. Also read field-* and fluid-* sub-group names from each emitter
///
/// So fields are hashed as:  SFHash("emitterName", "fieldName")
/// NOT as:                   SFHash("GroupPart0", "fieldName")
/// </summary>
public static class IniHashDictionary
{
    private static uint IHash(string value, uint ret = 0)
    {
        foreach (char c in value)
            ret = (uint)(char.ToLowerInvariant(c) + 65599u * ret);
        return ret;
    }

    public static uint SectionFieldHash(string section, string field)
    {
        uint sh = IHash("*", IHash(section));
        return IHash(field, sh);
    }

    private static uint SectionFieldHashComment(string section, string field)
    {
        uint sh = IHash("*", IHash(section));
        return IHash("'" + field, sh);
    }

    // ── Constants matching dictionary.jsx ────────────────────────────────────
    private const int FIELD_VARS = 10;
    private const int GPART_VARS = 50;
    private const int MAT_VARS   = 5;
    private const int RAND_VARS  = 10;
    private const int COLOR_VARS = 25;
    private const int ROT_VARS   = 10;

    // ── System-level field names ──────────────────────────────────────────────
    private static IEnumerable<string> GetSystemFieldNames()
    {
        // GroupPartN, GroupPartNType, GroupPartNImportance
        for (int i = 0; i < GPART_VARS; i++)
        {
            yield return $"GroupPart{i}";
            yield return $"GroupPart{i}Type";
            yield return $"GroupPart{i}Importance";
            yield return $"Override-Offset{i}";
            yield return $"Override-Rotation{i}";
            yield return $"Override-Scale{i}";
        }
        yield return "AudioFlexValueParameterName";
        yield return "AudioParameterFlexID";
        yield return "build-up-time";
        yield return "group-vis";
        yield return "group-scale-cap";
        yield return "KeepOrientationAfterSpellCast";
        yield return "PersistThruDeath";
        yield return "PersistThruRevive";
        yield return "SelfIllumination";
        yield return "SimulateEveryFrame";
        yield return "SimulateOncePerFrame";
        yield return "SimulateWhileOffScreen";
        yield return "SoundEndsOnEmitterEnd";
        yield return "SoundOnCreate";
        yield return "SoundPersistent";
        yield return "SoundsPlayWhileOffScreen";
        yield return "VoiceOverOnCreate";
        yield return "VoiceOverPersistent";
        for (int i = 0; i < MAT_VARS; i++)
        {
            yield return $"MaterialOverride{i}BlendMode";
            yield return $"MaterialOverride{i}Texture";
            yield return $"MaterialOverride{i}SubMesh";
        }
    }

    // ── All group/emitter field names (from groupNames + flex/rand expansions) ─
    private static IEnumerable<string> GetGroupFieldNames()
    {
        // Base group names from dictionary.jsx groupNames[]
        var groupNames = new[]
        {
            "ExcludeAttachmentType","KeywordsExcluded","KeywordsIncluded","KeywordsRequired",
            "Particle-ScaleAlongMovementVector","SoundOnCreate","SoundPersistent",
            "VoiceOverOnCreate","VoiceOverPersistent","dont-scroll-alpha-UV",
            "e-active","e-alpharef","e-beam-segments","e-censor-policy","e-disabled",
            "e-life","e-life-scale","e-linger","e-local-orient","e-period",
            "e-shape-name","e-shape-scale","e-shape-use-normal-for-birth",
            "e-soft-in-depth","e-soft-out-depth","e-soft-in-depth-delta","e-soft-out-depth-delta",
            "e-timeoffset","e-trail-cutoff","e-trail-smoothing","e-uvscroll","e-uvscroll-mult",
            "flag-brighter-in-fow","flag-disable-z","flag-disable-y","flag-groundlayer",
            "flag-ground-layer","flag-force-animated-mesh-z-write","flag-projected",
            "p-alphaslicerange","p-animation","p-backfaceon","p-beammode","p-bindtoemitter",
            "p-coloroffset","p-colorscale","p-colortype","p-distortion-mode","p-distortion-power",
            "p-falloff-texture","p-fixedorbit","p-fixedorbittype","p-flexoffset","p-flexscale",
            "p-followterrain","p-frameRate","p-frameRate-mult","p-fresnel","p-life-scale",
            "p-life-scale-offset","p-life-scale-symX","p-life-scale-symY","p-life-scale-symZ",
            "p-linger","p-local-orient","p-lockedtoemitter","p-mesh","p-meshtex",
            "p-meshtex-mult","p-normal-map","p-numframes","p-numframes-mult",
            "p-offsetbyheight","p-offsetbyradius","p-orientation","p-projection-fading",
            "p-projection-y-range","p-randomstartframe","p-randomstartframe-mult",
            "p-reflection-fresnel","p-reflection-map","p-reflection-opacity-direct",
            "p-reflection-opacity-glancing","p-rgba","p-scalebias","p-scalebyheight",
            "p-scalebyradius","p-scaleupfromorigin","p-shadow","p-simpleorient",
            "p-skeleton","p-skin","p-startframe","p-startframe-mult","p-texdiv",
            "p-texdiv-mult","p-texture","p-texture-mode","p-texture-mult",
            "p-texture-mult-mode","p-texture-pixelate","p-trailmode","p-type","p-uvmode",
            "p-uvparallax-scale","p-uvscroll-alpha-mult","p-uvscroll-no-alpha","p-uvscroll-rgb",
            "p-uvscroll-rgb-clamp","p-uvscroll-rgb-clamp-mult","p-vec-velocity-minscale",
            "p-vec-velocity-scale","p-vecalign","p-xquadrot-on","pass","rendermode",
            "single-particle","submesh-list","teamcolor-correction","uniformscale",
            "ChildParticleName","ChildSpawnAtBone","ChildEmitOnDeath","p-childProb",
        };
        foreach (var n in groupNames) yield return n;

        // ChildParticleNameN etc.
        for (int i = 0; i < GPART_VARS; i++)
        {
            yield return $"ChildParticleName{i}";
            yield return $"ChildSpawnAtBone{i}";
            yield return $"ChildEmitOnDeath{i}";
        }
        // MaterialOverrideN*
        for (int i = 0; i < MAT_VARS; i++)
        {
            yield return $"MaterialOverride{i}BlendMode";
            yield return $"MaterialOverride{i}GlossTexture";
            yield return $"MaterialOverride{i}EmissiveTexture";
            yield return $"MaterialOverride{i}FixedAlphaScrolling";
            yield return $"MaterialOverride{i}Priority";
            yield return $"MaterialOverride{i}RenderingMode";
            yield return $"MaterialOverride{i}SubMesh";
            yield return $"MaterialOverride{i}Texture";
            yield return $"MaterialOverride{i}UVScroll";
        }

        // e-rgba / p-rgba / p-xrgba color variants
        foreach (var b in new[] { "e-rgba", "p-rgba", "p-xrgba" })
        {
            yield return b;
            for (int i = 0; i < COLOR_VARS; i++) yield return $"{b}{i}";
            foreach (var mod in new[] { "R", "G", "B", "A" })
            {
                yield return $"{b}{mod}P";
                for (int i = 0; i < COLOR_VARS; i++) yield return $"{b}{mod}P{i}";
            }
        }

        // flexFloat: p-scale, p-scaleEmitOffset  → name, name_flex, name_flex0..3
        foreach (var b in new[] { "p-scale", "p-scaleEmitOffset" })
        {
            yield return b; yield return $"{b}_flex";
            for (int j = 0; j < 4; j++) yield return $"{b}_flex{j}";
        }

        // flexRandFloat: e-rate, p-life, p-rotvel
        foreach (var n in FlexRandFloat(new[] { "e-rate", "p-life", "p-rotvel" })) yield return n;

        // flexRandVec2: e-uvoffset
        foreach (var n in FlexRandVec2(new[] { "e-uvoffset" })) yield return n;

        // flexRandVec3: p-offset, p-postoffset, p-vel
        foreach (var n in FlexRandVec3(new[] { "p-offset", "p-postoffset", "p-vel" })) yield return n;

        // randFloat: many fields
        foreach (var n in RandFloat(new[]
        {
            "e-color-modulate","e-framerate","p-bindtoemitter","p-life","p-quadrot",
            "p-rotvel","p-scale","p-xquadrot","p-xscale","e-rate"
        })) yield return n;

        // randVec2
        foreach (var n in RandVec2(new[]
        {
            "e-ratebyvel","e-uvoffset","e-uvoffset-mult","p-uvscroll-rgb","p-uvscroll-rgb-mult"
        })) yield return n;

        // randVec3: many fields
        foreach (var n in RandVec3(new[]
        {
            "Emitter-BirthRotationalAcceleration","Particle-Acceleration","Particle-Drag",
            "Particle-Velocity","e-tilesize","p-accel","p-drag","p-offset","p-orbitvel",
            "p-postoffset","p-quadrot","p-rotvel","p-scale","p-vel","p-worldaccel",
            "p-xquadrot","p-xrgba-beam-bind-distance","p-xscale"
        })) yield return n;

        // e-rotationN rand + axis
        for (int i = 0; i < ROT_VARS; i++)
        {
            foreach (var n in RandFloat(new[] { $"e-rotation{i}" })) yield return n;
            yield return $"e-rotation{i}-axis";
        }

        // field-accel-N .. field-orbit-N (sub-group names stored in group)
        for (int i = 1; i < FIELD_VARS; i++)
        {
            yield return $"field-accel-{i}";
            yield return $"field-attract-{i}";
            yield return $"field-drag-{i}";
            yield return $"field-noise-{i}";
            yield return $"field-orbit-{i}";
        }
        yield return "fluid-params";
    }

    // ── Rand/flex expansion helpers ──────────────────────────────────────────

    private static IEnumerable<string> RandFloat(IEnumerable<string> bases)
    {
        foreach (var b in bases)
        {
            yield return b;
            for (int j = 0; j < RAND_VARS; j++) yield return $"{b}{j}";
            yield return $"{b}XP";
            for (int j = 0; j < RAND_VARS; j++) yield return $"{b}XP{j}";
            yield return $"{b}P";
            for (int j = 0; j < RAND_VARS; j++) yield return $"{b}P{j}";
        }
    }

    private static IEnumerable<string> RandVec2(IEnumerable<string> bases)
    {
        foreach (var b in bases)
        {
            yield return b;
            for (int j = 0; j < RAND_VARS; j++) yield return $"{b}{j}";
            foreach (var ax in new[] { "X", "Y" })
            {
                yield return $"{b}{ax}P";
                for (int j = 0; j < RAND_VARS; j++) yield return $"{b}{ax}P{j}";
            }
        }
    }

    private static IEnumerable<string> RandVec3(IEnumerable<string> bases)
    {
        foreach (var b in bases)
        {
            yield return b;
            for (int j = 0; j < RAND_VARS; j++) yield return $"{b}{j}";
            foreach (var ax in new[] { "X", "Y", "Z" })
            {
                yield return $"{b}{ax}P";
                for (int j = 0; j < RAND_VARS; j++) yield return $"{b}{ax}P{j}";
            }
        }
    }

    private static IEnumerable<string> Flex(IEnumerable<string> bases)
    {
        foreach (var b in bases)
        {
            yield return b;
            yield return $"{b}_flex";
            for (int j = 0; j < 4; j++) yield return $"{b}_flex{j}";
        }
    }

    private static IEnumerable<string> FlexRandFloat(IEnumerable<string> bases) => RandFloat(Flex(bases));
    private static IEnumerable<string> FlexRandVec2(IEnumerable<string> bases)  => RandVec2(Flex(bases));
    private static IEnumerable<string> FlexRandVec3(IEnumerable<string> bases)  => RandVec3(Flex(bases));

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Builds a hash→"[section] field" map for a specific Troybin file.
    /// groupNames = the actual emitter names (values of System[GroupPart0..N]).
    /// </summary>
    public static Dictionary<uint, string> BuildHashMap(IEnumerable<string>? groupNames = null)
    {
        var map = new Dictionary<uint, string>();

        void Add(string section, IEnumerable<string> fieldNames)
        {
            foreach (var field in fieldNames)
            {
                uint h1 = SectionFieldHash(section, field);
                uint h2 = SectionFieldHashComment(section, field);
                string label = $"[{section}] {field}";
                map.TryAdd(h1, label);
                map.TryAdd(h2, label);
            }
        }

        // Always add System section
        Add("System", GetSystemFieldNames());

        // GroupPartN section keys used in dictionary (fallback)
        var gpFields = GetGroupFieldNames().ToList();
        for (int i = 0; i < GPART_VARS; i++)
            Add($"GroupPart{i}", gpFields);

        // If we have actual group names, build hashes using those as sections
        if (groupNames != null)
            foreach (var gn in groupNames.Where(n => !string.IsNullOrEmpty(n)))
                Add(gn, gpFields);

        return map;
    }

    /// <summary>Parse a label string "[section] field" into its two parts.</summary>
    public static (string section, string field) ParseLabel(string label)
    {
        int start = label.IndexOf('[') + 1;
        int end   = label.IndexOf(']');
        if (start > 0 && end > start)
            return (label.Substring(start, end - start), label.Substring(end + 2));
        return ("Unknown", label);
    }

    /// <summary>Resolve a hash to (section, fieldName) pair.</summary>
    public static (string? section, string? field) Resolve(uint hash, IEnumerable<string>? groupNames = null)
    {
        var map = BuildHashMap(groupNames);
        if (!map.TryGetValue(hash, out var label)) return (null, null);
        var (s, f) = ParseLabel(label);
        return (s, f);
    }
}


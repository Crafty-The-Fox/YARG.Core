// Copyright (c) 2016-2020 Alexander Ong
// See LICENSE in project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MoonscraperChartEditor.Song.IO
{
    using static TextEventDefinitions;

    internal static class MidIOHelper
    {
        // Track names
        public const string BEAT_TRACK = "BEAT";
        public const string EVENTS_TRACK = "EVENTS";
        public const string VENUE_TRACK = "VENUE";
        public const string GUITAR_TRACK = "PART GUITAR";
        public const string GH1_GUITAR_TRACK = "T1 GEMS";
        public const string GUITAR_COOP_TRACK = "PART GUITAR COOP";
        public const string BASS_TRACK = "PART BASS";
        public const string RHYTHM_TRACK = "PART RHYTHM";
        public const string KEYS_TRACK = "PART KEYS";
        public const string PRO_GUITAR_17_FRET_TRACK = "PART REAL_GUITAR";
        public const string PRO_GUITAR_22_FRET_TRACK = "PART REAL_GUITAR_22";
        public const string PRO_BASS_17_FRET_TRACK = "PART REAL_BASS";
        public const string PRO_BASS_22_FRET_TRACK = "PART REAL_BASS_22";
        public const string DRUMS_TRACK = "PART DRUMS";
        public const string DRUMS_TRACK_2 = "PART DRUM";
        public const string DRUMS_REAL_TRACK = "PART REAL_DRUMS_PS";
        public const string GHL_GUITAR_TRACK = "PART GUITAR GHL";
        public const string GHL_BASS_TRACK = "PART BASS GHL";
        public const string GHL_RHYTHM_TRACK = "PART RHYTHM GHL";
        public const string GHL_GUITAR_COOP_TRACK = "PART GUITAR COOP GHL";
        public const string VOCALS_TRACK = "PART VOCALS";
        public const string HARMONY_1_TRACK = "HARM1";
        public const string HARMONY_2_TRACK = "HARM2";
        public const string HARMONY_3_TRACK = "HARM3";
        // The Beatles: Rock Band uses these instead for its harmony tracks
        public const string HARMONY_1_TRACK_2 = "PART HARM1";
        public const string HARMONY_2_TRACK_2 = "PART HARM2";
        public const string HARMONY_3_TRACK_2 = "PART HARM3";

        // Regex for text events, matches text inside [brackets] without including the brackets
		// '[end]' -> 'end'
        // '[section Solo] - "Solo"' -> 'section Solo'
		public static readonly Regex TextEventRegex = new(@"\[(.*?)\]", RegexOptions.Compiled | RegexOptions.Singleline);

        // Regex for section events, assumes the brackets have already been stripped out
        // 'section Practice' -> 'Practice'
        // 'prc_intro' -> 'intro'
        // 'section_outro' -> 'outro'
		public static readonly Regex SectionEventRegex = new(@"(?:section|prc)[ _](.*)", RegexOptions.Compiled | RegexOptions.Singleline);

        // Matches venue lighting events and groups the text inside (parentheses), not including the parentheses
        // 'lighting (verse)' -> 'verse'
        // 'lighting (flare_fast)' -> 'flare_fast'
        // 'lighting ()' -> ''
        public static readonly Regex LightingRegex = new(@"lighting\s+\((.*)\)", RegexOptions.Compiled | RegexOptions.Singleline);

        // Note numbers
        public const byte DOUBLE_KICK_NOTE = 95;
        public const byte SOLO_NOTE = 103;                 // http://docs.c3universe.com/rbndocs/index.php?title=Guitar_and_Bass_Authoring#Solo_Sections
        public const byte TAP_NOTE_CH = 104;               // https://github.com/TheNathannator/GuitarGame_ChartFormats/blob/main/doc/FileFormats/.mid/Standard/5-Fret Guitar.md
        public const byte VERSUS_PHRASE_PLAYER_1 = 105;    // Guitar Hero 2 and Rock Band 1/2 use these to mark phrases for face-off
        public const byte VERSUS_PHRASE_PLAYER_2 = 106;    // and other competitive modes where the players trade off phrases of notes
        public const byte LYRICS_PHRASE_1 = VERSUS_PHRASE_PLAYER_1; // These are also used to mark phrases on vocals
        public const byte LYRICS_PHRASE_2 = VERSUS_PHRASE_PLAYER_2; // Rock Band 3 dropped these versus phrases however, and on vocals just uses note 105
        public const byte FLAM_MARKER = 109;
        public const byte STARPOWER_NOTE = 116;            // http://docs.c3universe.com/rbndocs/index.php?title=Overdrive_and_Big_Rock_Endings

        // http://docs.c3universe.com/rbndocs/index.php?title=Drum_Authoring#Drum_Fills
        public const byte DRUM_FILL_NOTE_0 = 120;
        public const byte DRUM_FILL_NOTE_1 = 121;
        public const byte DRUM_FILL_NOTE_2 = 122;
        public const byte DRUM_FILL_NOTE_3 = 123;
        public const byte DRUM_FILL_NOTE_4 = 124;

        // Drum rolls - http://docs.c3universe.com/rbndocs/index.php?title=Drum_Authoring#Drum_Rolls
        public const byte TREMOLO_LANE_NOTE = 126;
        public const byte TRILL_LANE_NOTE = 127;

        // Pro Guitar notes
        public const byte SOLO_NOTE_PRO_GUITAR = 115;

        // Vocals notes
        public const byte LYRIC_SHIFT_NOTE = 0;
        public const byte RANGE_SHIFT_NOTE = 1;
        public const byte VOCALS_RANGE_START = 36;
        public const byte VOCALS_RANGE_END = 84;
        public const byte PERCUSSION_NOTE = 96;
        public const byte NONPLAYED_PERCUSSION_NOTE = 97;

        // Pro Guitar channels
        public const byte PRO_GUITAR_CHANNEL_NORMAL = 0;
        public const byte PRO_GUITAR_CHANNEL_GHOST = 1;
        public const byte PRO_GUITAR_CHANNEL_BEND = 2;
        public const byte PRO_GUITAR_CHANNEL_MUTED = 3;
        public const byte PRO_GUITAR_CHANNEL_TAP = 4;
        public const byte PRO_GUITAR_CHANNEL_HARMONIC = 5;
        public const byte PRO_GUITAR_CHANNEL_PINCH_HARMONIC = 6;

        // Beat track notes
        public const byte BEAT_STRONG = 12;
        public const byte BEAT_WEAK = 13;

        // These events are valid both with and without brackets.
        // The bracketed versions follow the style of other existing .mid text events.
        public const string CHART_DYNAMICS_TEXT = "ENABLE_CHART_DYNAMICS";
        public const string ENHANCED_OPENS_TEXT = "ENHANCED_OPENS";

        // Note velocities
        public const byte VELOCITY = 100;             // default note velocity for exporting
        public const byte VELOCITY_ACCENT = 127;      // fof/ps
        public const byte VELOCITY_GHOST = 1;         // fof/ps

        // Lookup tables
        public static readonly Dictionary<string, MoonSong.MoonInstrument> TrackNameToInstrumentMap = new()
        {
            { GUITAR_TRACK,        MoonSong.MoonInstrument.Guitar },
            { GH1_GUITAR_TRACK,    MoonSong.MoonInstrument.Guitar },
            { GUITAR_COOP_TRACK,   MoonSong.MoonInstrument.GuitarCoop },
            { BASS_TRACK,          MoonSong.MoonInstrument.Bass },
            { RHYTHM_TRACK,        MoonSong.MoonInstrument.Rhythm },
            { KEYS_TRACK,          MoonSong.MoonInstrument.Keys },

            { DRUMS_TRACK,         MoonSong.MoonInstrument.Drums },
            { DRUMS_REAL_TRACK,    MoonSong.MoonInstrument.Drums },

            { GHL_GUITAR_TRACK,    MoonSong.MoonInstrument.GHLiveGuitar },
            { GHL_BASS_TRACK,      MoonSong.MoonInstrument.GHLiveBass },
            { GHL_RHYTHM_TRACK,    MoonSong.MoonInstrument.GHLiveRhythm },
            { GHL_GUITAR_COOP_TRACK, MoonSong.MoonInstrument.GHLiveCoop },

            { PRO_GUITAR_17_FRET_TRACK, MoonSong.MoonInstrument.ProGuitar_17Fret },
            { PRO_GUITAR_22_FRET_TRACK, MoonSong.MoonInstrument.ProGuitar_22Fret },
            { PRO_BASS_17_FRET_TRACK,   MoonSong.MoonInstrument.ProBass_17Fret },
            { PRO_BASS_22_FRET_TRACK,   MoonSong.MoonInstrument.ProBass_22Fret },

            { VOCALS_TRACK,        MoonSong.MoonInstrument.Vocals },
            { HARMONY_1_TRACK,     MoonSong.MoonInstrument.Harmony1 },
            { HARMONY_2_TRACK,     MoonSong.MoonInstrument.Harmony2 },
            { HARMONY_3_TRACK,     MoonSong.MoonInstrument.Harmony3 },
            { HARMONY_1_TRACK_2,   MoonSong.MoonInstrument.Harmony1 },
            { HARMONY_2_TRACK_2,   MoonSong.MoonInstrument.Harmony2 },
            { HARMONY_3_TRACK_2,   MoonSong.MoonInstrument.Harmony3 },
        };

        public static readonly Dictionary<MoonSong.Difficulty, int> GUITAR_DIFF_START_LOOKUP = new()
        {
            { MoonSong.Difficulty.Easy, 60 },
            { MoonSong.Difficulty.Medium, 72 },
            { MoonSong.Difficulty.Hard, 84 },
            { MoonSong.Difficulty.Expert, 96 }
        };

        public static readonly Dictionary<MoonSong.Difficulty, int> GHL_GUITAR_DIFF_START_LOOKUP = new()
        {
            { MoonSong.Difficulty.Easy, 58 },
            { MoonSong.Difficulty.Medium, 70 },
            { MoonSong.Difficulty.Hard, 82 },
            { MoonSong.Difficulty.Expert, 94 }
        };

        public static readonly Dictionary<MoonSong.Difficulty, int> PRO_GUITAR_DIFF_START_LOOKUP = new()
        {
            { MoonSong.Difficulty.Easy, 24 },
            { MoonSong.Difficulty.Medium, 48 },
            { MoonSong.Difficulty.Hard, 72 },
            { MoonSong.Difficulty.Expert, 96 }
        };

        public static readonly Dictionary<MoonSong.Difficulty, int> DRUMS_DIFF_START_LOOKUP = new()
        {
            { MoonSong.Difficulty.Easy, 60 },
            { MoonSong.Difficulty.Medium, 72 },
            { MoonSong.Difficulty.Hard, 84 },
            { MoonSong.Difficulty.Expert, 96 }
        };

        // http://docs.c3universe.com/rbndocs/index.php?title=Drum_Authoring
        public static readonly Dictionary<MoonNote.DrumPad, int> PAD_TO_CYMBAL_LOOKUP = new()
        {
            { MoonNote.DrumPad.Yellow, 110 },
            { MoonNote.DrumPad.Blue, 111 },
            { MoonNote.DrumPad.Orange, 112 },
        };

        public static readonly Dictionary<int, MoonNote.DrumPad> CYMBAL_TO_PAD_LOOKUP = PAD_TO_CYMBAL_LOOKUP.ToDictionary((i) => i.Value, (i) => i.Key);

        public static readonly Dictionary<byte, MoonNote.Flags> PRO_GUITAR_CHANNEL_FLAG_LOOKUP = new()
        {
            // Not all flags are implemented yet
            { PRO_GUITAR_CHANNEL_NORMAL,         MoonNote.Flags.None },
            // { PRO_GUITAR_CHANNEL_GHOST,          MoonNote.Flags. },
            // { PRO_GUITAR_CHANNEL_BEND,           MoonNote.Flags. },
            { PRO_GUITAR_CHANNEL_MUTED,          MoonNote.Flags.ProGuitar_Muted },
            // { PRO_GUITAR_CHANNEL_TAP,            MoonNote.Flags. },
            // { PRO_GUITAR_CHANNEL_HARMONIC,       MoonNote.Flags. },
            // { PRO_GUITAR_CHANNEL_PINCH_HARMONIC, MoonNote.Flags. },
        };

        public static readonly Dictionary<int, (VenueEvent.Type type, string text)> VENUE_NOTE_LOOKUP = new()
        {
            #region Post-processing events
            { 110, (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_TRAILS_LONG) },           // Trails
            { 109, (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SCANLINES_SECURITY) },    // Security camera
            { 108, (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SCANLINES_BLACK_WHITE) }, // Black and white
            { 107, (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SCANLINES) },             // Scanlines
            { 106, (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SCANLINES_BLUE) },        // Blue tint
            { 105, (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_MIRROR) },                // Mirror
            { 104, (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_DESATURATED_RED) },       // Bloom B
            { 103, (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_BLOOM) },                 // Bloom A
            { 102, (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_CHOPPY_BLACK_WHITE) },    // Photocopy
            { 101, (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_PHOTONEGATIVE) },         // Negative
            { 100, (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SILVERTONE) },            // Silvertone
            { 99,  (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SEPIATONE) },             // Sepia
            { 98,  (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_GRAINY_FILM) },           // 16mm
            { 97,  (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_POLARIZED_BLACK_WHITE) }, // Contrast A
            { 96,  (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_DEFAULT) },               // Default
            #endregion

            #region Performer singalongs
            { 87, (VenueEvent.Type.Singalong, VENUE_PERFORMER_GUITAR) },
            { 86, (VenueEvent.Type.Singalong, VENUE_PERFORMER_DRUMS) },
            { 85, (VenueEvent.Type.Singalong, VENUE_PERFORMER_BASS) },
            #endregion

            #region Lighting keyframes
            { 50, (VenueEvent.Type.Lighting, VENUE_LIGHTING_FIRST) },
            { 49, (VenueEvent.Type.Lighting, VENUE_LIGHTING_PREVIOUS) },
            { 48, (VenueEvent.Type.Lighting, VENUE_LIGHTING_NEXT) },
            #endregion

            #region Performer spotlights
            { 41, (VenueEvent.Type.Spotlight, VENUE_PERFORMER_KEYS) },
            { 40, (VenueEvent.Type.Spotlight, VENUE_PERFORMER_VOCALS) },
            { 39, (VenueEvent.Type.Spotlight, VENUE_PERFORMER_GUITAR) },
            { 38, (VenueEvent.Type.Spotlight, VENUE_PERFORMER_DRUMS) },
            { 37, (VenueEvent.Type.Spotlight, VENUE_PERFORMER_BASS) },
            #endregion
        };

        public static readonly Dictionary<string, (VenueEvent.Type type, string text)> VENUE_TEXT_CONVERSION_LOOKUP = new()
        {
            #region Lighting events
            // Keyframe events
            { "first", (VenueEvent.Type.Lighting, VENUE_LIGHTING_FIRST) },
            { "next",  (VenueEvent.Type.Lighting, VENUE_LIGHTING_NEXT) },
            { "prev",  (VenueEvent.Type.Lighting, VENUE_LIGHTING_PREVIOUS) },

            // RBN1 equivalents for `lighting (chorus)` and `lighting (verse)`
            { "verse",  (VenueEvent.Type.Lighting, VENUE_LIGHTING_VERSE) },
            { "chorus", (VenueEvent.Type.Lighting, VENUE_LIGHTING_CHORUS) },
            #endregion

            #region Post-processing events
            { "bloom.pp",                        (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_BLOOM) },
            { "bright.pp",                       (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_BRIGHT) },
            { "clean_trails.pp",                 (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_TRAILS) },
            { "contrast_a.pp",                   (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_POLARIZED_BLACK_WHITE) },
            { "desat_blue.pp",                   (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_DESATURATED_BLUE) },
            { "desat_posterize_trails.pp",       (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_TRAILS_DESATURATED) },
            { "film_contrast.pp",                (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_CONTRAST) },
            { "film_b+w.pp",                     (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_BLACK_WHITE) },
            { "film_sepia_ink.pp",               (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SEPIATONE) },
            { "film_silvertone.pp",              (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SILVERTONE) },
            { "film_contrast_red.pp",            (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_CONTRAST_RED) },
            { "film_contrast_green.pp",          (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_CONTRAST_GREEN) },
            { "film_contrast_blue.pp",           (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_CONTRAST_BLUE) },
            { "film_16mm.pp",                    (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_GRAINY_FILM) },
            { "film_blue_filter.pp",             (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SCANLINES_BLUE) },
            { "flicker_trails.pp",               (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_TRAILS_FLICKERY) },
            { "horror_movie_special.pp",         (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_PHOTONEGATIVE_RED_BLACK) },
            { "photocopy.pp",                    (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_CHOPPY_BLACK_WHITE) },
            { "photo_negative.pp",               (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_PHOTONEGATIVE) },
            { "posterize.pp",                    (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_POSTERIZE) },
            { "ProFilm_a.pp",                    (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_DEFAULT) },
            { "ProFilm_b.pp",                    (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_DESATURATED_RED) },
            { "ProFilm_mirror_a.pp",             (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_MIRROR) },
            { "ProFilm_psychedelic_blue_red.pp", (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_POLARIZED_RED_BLUE) },
            { "shitty_tv.pp",                    (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_GRAINY_CHROMATIC_ABBERATION) },
            { "space_woosh.pp",                  (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_TRAILS_SPACEY) },
            { "video_a.pp",                      (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SCANLINES) },
            { "video_bw.pp",                     (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SCANLINES_BLACK_WHITE) },
            { "video_security.pp",               (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_SCANLINES_SECURITY) },
            { "video_trails.pp",                 (VenueEvent.Type.PostProcessing, VENUE_POSTPROCESS_TRAILS_LONG) },
            #endregion

            #region Miscellaneous
            { "bonusfx",          (VenueEvent.Type.Miscellaneous, VENUE_MISC_BONUS_FX) },
            { "bonusfx_optional", (VenueEvent.Type.Miscellaneous, VENUE_OPTIONAL_EVENT_PREFIX_WITH_SPACE + VENUE_MISC_BONUS_FX) },
            { "FogOn",            (VenueEvent.Type.Miscellaneous, VENUE_MISC_FOG_ON) },
            { "FogOff",           (VenueEvent.Type.Miscellaneous, VENUE_MISC_FOG_OFF) },
            #endregion
        };

        public static readonly Dictionary<Regex, (Dictionary<string, string> lookup, VenueEvent.Type type, string defaultValue)> VENUE_EVENT_REGEX_TO_LOOKUP = new()
        {
            { LightingRegex,    (VENUE_LIGHTING_CONVERSION_LOOKUP, VenueEvent.Type.Lighting, VENUE_LIGHTING_DEFAULT) },
        };

        public static readonly Dictionary<string, string> VENUE_LIGHTING_CONVERSION_LOOKUP = new()
        {
            #region Keyframed
            // { string.Empty,  VENUE_LIGHTING_DEFAULT }, // Handled by the default case
            { "chorus",      VENUE_LIGHTING_CHORUS },
            { "dischord",    VENUE_LIGHTING_DISCHORD },
            { "manual_cool", VENUE_LIGHTING_COOL_MANUAL },
            { "manual_warm", VENUE_LIGHTING_WARM_MANUAL },
            { "stomp",       VENUE_LIGHTING_STOMP },
            { "verse",       VENUE_LIGHTING_VERSE },
            #endregion

            #region Automatic
            { "blackout_fast",    VENUE_LIGHTING_BLACKOUT_FAST },
            { "blackout_slow",    VENUE_LIGHTING_BLACKOUT_SLOW },
            { "blackout_spot",    VENUE_LIGHTING_BLACKOUT_SPOTLIGHT },
            { "bre",              VENUE_LIGHTING_BIG_ROCK_ENDING },
            { "flare_fast",       VENUE_LIGHTING_FLARE_FAST },
            { "flare_slow",       VENUE_LIGHTING_FLARE_SLOW },
            { "frenzy",           VENUE_LIGHTING_FRENZY },
            { "harmony",          VENUE_LIGHTING_HARMONY },
            { "intro",            VENUE_LIGHTING_INTRO },
            { "loop_cool",        VENUE_LIGHTING_COOL_AUTOMATIC },
            { "loop_warm",        VENUE_LIGHTING_WARM_AUTOMATIC },
            { "searchlights",     VENUE_LIGHTING_SEARCHLIGHTS },
            { "silhouettes",      VENUE_LIGHTING_SILHOUETTES },
            { "silhouettes_spot", VENUE_LIGHTING_SILHOUETTES_SPOTLIGHT },
            { "strobe_fast",      VENUE_LIGHTING_STROBE_FAST },
            { "strobe_slow",      VENUE_LIGHTING_STROBE_SLOW },
            { "sweep",            VENUE_LIGHTING_SWEEP },
            #endregion
        };
    }
}

﻿using System;

namespace YARG.Core.Chart
{
    public class ProGuitarNote : Note<ProGuitarNote>
    {
        private readonly ProGuitarNoteFlags _proFlags;

        public ProGuitarNoteFlags ProFlags;

        public int String   { get; }
        public int Fret     { get; }

        public ProGuitarNoteType Type { get; set; }

        public bool IsStrum => Type == ProGuitarNoteType.Strum;
        public bool IsHopo  => Type == ProGuitarNoteType.Hopo;
        public bool IsTap   => Type == ProGuitarNoteType.Tap;

        public bool IsSustain => TickLength > 0;

        public bool IsExtendedSustain => (ProFlags & ProGuitarNoteFlags.ExtendedSustain) != 0;
        public bool IsDisjoint        => (ProFlags & ProGuitarNoteFlags.Disjoint) != 0;

        public bool IsMuted => (ProFlags & ProGuitarNoteFlags.Muted) != 0;

        public ProGuitarNote(ProGuitarString proString, int proFret, ProGuitarNoteType type, ProGuitarNoteFlags proFlags,
            NoteFlags flags, double time, double timeLength, uint tick, uint tickLength)
            : this((int) proString, proFret, type, proFlags, flags, time, timeLength, tick, tickLength)
        {
        }

        public ProGuitarNote(int proString, int proFret, ProGuitarNoteType type, ProGuitarNoteFlags proFlags,
            NoteFlags flags, double time, double timeLength, uint tick, uint tickLength)
            : base(flags, time, timeLength, tick, tickLength)
        {
            String = proString;
            Fret = proFret;
            Type = type;

            _proFlags = proFlags;
            ProFlags = proFlags;
        }

        public override void ResetNoteState()
        {
            base.ResetNoteState();
            ProFlags = _proFlags;
        }
    }

    public enum ProGuitarString
    {
        Red,
        Green,
        Orange,
        Blue,
        Yellow,
        Purple,
    }

    public enum ProGuitarNoteType
    {
        Strum,
        Hopo,
        Tap,
    }

    [Flags]
    public enum ProGuitarNoteFlags
    {
        None = 0,

        ExtendedSustain = 1 << 0,
        Disjoint        = 1 << 1,

        Muted = 1 << 2, // TODO: would this make more sense as its own note type? physically, only strums can be muted
    }
}
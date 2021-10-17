﻿using RLEngine.Boards;
using RLEngine.Utils;

namespace RLEngine.Logs
{
    public class ModificationLog : Log
    {
        public ModificationLog(TileType newType, TileType previousType, Coords at)
        {
            NewType = newType;
            PreviousType = previousType;
            At = at;
        }

        public TileType NewType { get; }
        public TileType PreviousType { get; }
        public Coords At { get; }
    }
}
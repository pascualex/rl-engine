﻿namespace RLEngine.Abilities
{
    public interface IAreaTargetEffect : IEffect
    {
        string Source { get; }
        int Radius { get; }
        string NewGroup { get; }
    }
}
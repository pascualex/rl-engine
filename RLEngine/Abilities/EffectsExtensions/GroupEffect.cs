﻿using RLEngine.Logs;
using RLEngine.State;

namespace RLEngine.Abilities
{
    public static class GroupEffect
    {
        public static Log? CastGroup(this IGroupEffect effect,
        TargetDB targetDB, GameState state)
        {
            var log = new CombinedLog(effect.IsParallel);
            foreach (var newTarget in targetDB.GetEntityGroup(effect.Group))
            {
                targetDB.Add(effect.NewTarget, newTarget);
                var iterationLog = new CombinedLog(false);
                foreach (var nestedEffect in effect.Effects)
                {
                    iterationLog.Add(nestedEffect.Cast(targetDB, state));
                }
                log.Add(iterationLog);
            }
            return log;
        }
    }
}

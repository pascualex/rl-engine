﻿using RLEngine.Core.Logs;
using RLEngine.Core.Abilities;

using NRE = System.NullReferenceException;

namespace RLEngine.Core.Events
{
    internal class DestructionEffectEvent : EffectEvent<IDestructionEffect>
    {
        public DestructionEffectEvent(IDestructionEffect effect, TargetDB targetDB)
        : base(effect, targetDB) { }

        protected override ILog? InternalInvoke(EventContext ctx)
        {
            if (!targetDB.TryGetEntity(effect.Target, out var target)) throw new NRE();
            return ctx.ActionExecutor.Destroy(target);
        }
    }
}
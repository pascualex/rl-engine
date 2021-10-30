﻿using RLEngine.Core.Logs;
using RLEngine.Core.Abilities;
using RLEngine.Core.Boards;
using RLEngine.Core.Entities;
using RLEngine.Core.Utils;

using System;

namespace RLEngine.CLI
{
    public class GameView
    {
        private readonly int delayMS;

        public GameView(int delayMS)
        {
            this.delayMS = delayMS;
        }

        public void Process(ILog log)
        {
            if      (log is      AbilityLog      abilityLog) Write(     abilityLog);
            else if (log is        SpawnLog        spawnLog) Write(       spawnLog);
            else if (log is     MovementLog     movementLog) Write(    movementLog);
            else if (log is  DestructionLog  destructionLog) Write( destructionLog);
            else if (log is ModificationLog modificationLog) Write(modificationLog);
            else if (log is       DamageLog       damageLog) Write(      damageLog);
            else if (log is      HealingLog      healingLog) Write(     healingLog);
            else if (log is   ProjectileLog   projectileLog) Write(  projectileLog);
            else WriteUnsupported(log);

            if (delayMS > 0) System.Threading.Thread.Sleep(delayMS);
        }

        private static void Write(AbilityLog log)
        {
            Write(log.Caster);
            Console.Write(" casts ");
            Write(log.Ability);
            if (log.Target is IEntity or Coords) Console.Write(" targeted at ");
            if (log.Target is IEntity eTarget) Write(eTarget);
            if (log.Target is Coords cTarget) Write(cTarget);
            Console.WriteLine(".");
        }

        private static void Write(SpawnLog log)
        {
            Write(log.Entity);
            Console.Write(" spawns at ");
            Write(log.At);
            Console.WriteLine(".");
        }

        private static void Write(MovementLog log)
        {
            Write(log.Entity);
            Console.Write(" moves to ");
            Write(log.To);
            Console.WriteLine(".");
        }

        private static void Write(DestructionLog log)
        {
            Write(log.Entity);
            var verb = log.Entity.IsAgent ? "dies" : "is destroyed";
            Console.WriteLine($" {verb}.");
        }

        private static void Write(ModificationLog log)
        {
            Write(log.PreviousType);
            Console.Write(" at ");
            Write(log.At);
            Console.Write(" is modified to ");
            Write(log.NewType);
            Console.WriteLine(".");
        }

        private static void Write(DamageLog log)
        {
            if (log.Attacker != null)
            {
                Write(log.Attacker);
                Console.Write(" attacks. ");
            }
            Write(log.Target);
            Console.Write(" loses ");
            Write(log.ActualDamage, false);
            if (log.Damage > log.ActualDamage)
            {
                Console.Write(" (");
                Write(log.Damage, false);
                Console.Write(")");
            }
            Console.WriteLine(" health.");
        }

        private static void Write(HealingLog log)
        {
            if (log.Healer != null)
            {
                Write(log.Healer);
                Console.Write(" heals. ");
            }
            Write(log.Target);
            Console.Write(" recovers ");
            Write(log.ActualHealing, true);
            if (log.Healing > log.ActualHealing)
            {
                Console.Write(" (");
                Write(log.Healing, true);
                Console.Write(")");
            }
            Console.WriteLine(" health.");
        }

        private static void Write(ProjectileLog log)
        {
            Console.Write("A projectile goes from ");
            if (log.Source is IEntity eSource) Write(eSource);
            else if (log.Source is Coords cSource) Write(cSource);
            else WriteNull();
            Console.Write(" to ");
            if (log.Target is IEntity eTarget) Write(eTarget);
            else if (log.Target is Coords cTarget) Write(cTarget);
            else WriteNull();
            Console.WriteLine(".");
        }

        private static void WriteUnsupported(ILog log)
        {
            Console.Write("[");
            Write("error", ConsoleColor.Red);
            Console.WriteLine($"] {log.GetType().Name} is not supported.");
        }

        private static void Write(Ability ability)
        {
            Write(ability.Name, ConsoleColor.Yellow);
        }

        private static void Write(TileType tileType)
        {
            Write(tileType.Name, ConsoleColor.Yellow);
        }

        private static void Write(IEntity entity)
        {
            Write(entity.Name, ConsoleColor.Yellow);
        }

        private static void Write(Coords coords)
        {
            Write(coords, ConsoleColor.Blue);
        }

        private static void Write(int amount, bool isHeal)
        {
            Write(amount, isHeal ? ConsoleColor.Green : ConsoleColor.Red);
        }

        private static void WriteNull()
        {
            Write("null", ConsoleColor.Red);
        }

        private static void Write(object obj, ConsoleColor color)
        {
            var text = obj.ToString();
            if (text != null) Write(text, color);
        }

        private static void Write(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}
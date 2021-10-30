﻿namespace RLEngine.CLI
{
    public static class ExitCommand
    {
        private static readonly string[] aliases = { "exit", "e" };

        public static bool Execute(string command)
        {
            return AliasesUtils.Accepts(command, aliases);
        }
    }
}
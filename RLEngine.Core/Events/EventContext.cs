﻿using RLEngine.Core.Actions;
using RLEngine.Core.Turns;
using RLEngine.Core.Boards;

namespace RLEngine.Core.Events
{
    internal class EventContext
    {
        public EventContext(EventStack eventStack, ActionExecutor actionExecutor,
        ITurnManager turnManager, IBoard board)
        {
            ActionExecutor = actionExecutor;
            EventStack = eventStack;
            TurnManager = turnManager;
            Board = board;
        }

        public ActionExecutor ActionExecutor { get; }
        public EventStack EventStack { get; }
        public ITurnManager TurnManager { get; }
        public IBoard Board { get; }
    }
}
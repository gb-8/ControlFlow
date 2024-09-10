﻿namespace ControlFlow.Framework
{
    public interface IBlock
    {
        Task<BlockExecutionResult> Execute();
        Task<BlockExecutionResult> Handle(IMessage message);
    }
}
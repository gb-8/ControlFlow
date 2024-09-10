using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFlow.Framework
{
    public class StartSideEffect : IMessage
    {
        public StartSideEffect(Guid id) => Id = id;

        public Guid Id { get; }
    }
}

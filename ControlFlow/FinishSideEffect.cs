using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFlow
{
    public class FinishSideEffect : IMessage
    {
        public FinishSideEffect(Guid id) => this.Id = id;

        public Guid Id { get; }
    }
}

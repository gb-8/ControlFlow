using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlFlow
{
    public class StartSideEffect : IMessage
    {
        public StartSideEffect(Guid id) => this.Id = id;

        public Guid Id { get; }
    }
}

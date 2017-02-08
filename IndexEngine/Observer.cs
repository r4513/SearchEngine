using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IndexEngine
{
    public interface Observer
    {
        void Notify(object message);
    }
}

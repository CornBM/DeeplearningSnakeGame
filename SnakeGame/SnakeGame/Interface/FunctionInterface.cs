using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeGame.Interface
{
    public interface FunctionInterface
    {
        Task Start();
        void Stop();
        bool IsRunning();
    }
}

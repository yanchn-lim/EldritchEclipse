using UnityEngine;

namespace Debugger
{
    public interface IConsoleCommand
    {
        public string identifier { get; }
        public string description { get; }

        void Execute(string[] args, System.Action<string> Log);
    }
}
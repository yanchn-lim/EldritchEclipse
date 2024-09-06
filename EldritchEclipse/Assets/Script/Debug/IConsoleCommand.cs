using UnityEngine;

namespace Debugger
{
    public interface IConsoleCommand
    {
        public string identifier { get; }
        public string description { get; }
        public string argsDescription { get; }
        void Execute(string[] args, System.Action<string> Log);
    }
}
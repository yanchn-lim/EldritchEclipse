using System;
using TMPro;
using UnityEngine;

namespace Debugger
{
    public class TestCommand : IConsoleCommand
    {
        public string identifier => "test";
        public string description => "this is a test command";
        public string argsDescription => "int[optional]";

        public void Execute(string[] args, Action<string> Log)
        {
            if(args.Length == 0){
                Log("Test Message");
                return;
            }

            if(int.TryParse(args[0],out int x))
            {
                for (int i = 0; i < x; i++)
                    Log("Test Message " + i);


            }
            else
            {
                Log("Argument is invalid " + args[0]);
            }

        }
    }

    public class ClearCommand : IConsoleCommand
    {
        public string identifier => "clear";
        public string description => "clears the console";
        public string argsDescription => "";
        public TMP_Text consoleText;

        public void Execute(string[] args, Action<string> Log)
        {
            consoleText.text = "";
        }

        public ClearCommand(TMP_Text consoleText)
        {
            this.consoleText = consoleText;
        }
    }

    public class HelpCommand : IConsoleCommand
    {
        public string identifier => "help";
        public string description => "displays all available commands";
        public string argsDescription => "";

        public void Execute(string[] args, Action<string> Log)
        {
            Log("Available Commands:");
            foreach (var cmd in Console.Instance.commandRegistry)
            {
                Log(cmd.Value.identifier + " - " + cmd.Value.description);
            }
        }
    }
}

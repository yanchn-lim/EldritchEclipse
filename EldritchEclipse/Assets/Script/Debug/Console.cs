using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Debugger
{
    public class Console : MonoBehaviour
    {
        public Console Instance;
        //references
        [SerializeField] TMP_Text consoleText;
        [SerializeField] TMP_InputField inputField;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] Canvas canvas;

        //utility strings
        string startOfLine = "\n> ";

        //commands
        //[SerializeField] List<ConsoleCommand> commandList;
        Dictionary<string, IConsoleCommand> commandRegistry = new();
        List<string> commandHistory = new();
        int historyIndex;

        #region Initialize Console
        void Awake()
        {
            //singleton
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            RegisterCommands();
        }

        void RegisterCommands()
        {
            AddCommand(new TestCommand());
            AddCommand(new ClearCommand(consoleText));
        }

        void AddCommand(IConsoleCommand command)
        {
            if (!commandRegistry.ContainsKey(command.identifier))
            {
                commandRegistry.Add(command.identifier, command);
            }
        }

        #endregion

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                EvaluateCommand();
            }

            HistoryNavigation();
        }

        public void Log(string msg)
        {
            consoleText.text += startOfLine + msg;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0;
        }
        
        void ClearConsole()
        {
            consoleText.text = string.Empty;
        }

        public void EvaluateCommand()
        {
            string[] input = inputField.text.Split(' ');
            string id = input[0].ToLower();
            string[] args = new string[input.Length - 1];

            for (int i = 1; i < input.Length; i++)
            {
                args[i - 1] = input[i];
            }
            
            if(commandRegistry.TryGetValue(id,out IConsoleCommand cmd))
            {
                cmd.Execute(args,Log);
                commandHistory.Add(inputField.text);
                historyIndex = commandHistory.Count;
            }
            else
            {
                Log($"Command not found : {id}");
            }

            //moves the scroll rect to the bottom            
            inputField.text = string.Empty;
            inputField.ActivateInputField();
        }

        void HistoryNavigation()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (historyIndex > 0)
                {
                    historyIndex--;
                    inputField.text = commandHistory[historyIndex];                    
                }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (historyIndex < commandHistory.Count - 1)
                {
                    historyIndex++;
                    inputField.text = commandHistory[historyIndex];
                }
            }
        }
    }
}

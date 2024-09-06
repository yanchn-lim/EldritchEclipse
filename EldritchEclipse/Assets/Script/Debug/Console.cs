using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace Debugger
{
    public class Console : MonoBehaviour
    {
        public static Console Instance;
        //references
        [SerializeField] TMP_Text consoleText;
        [SerializeField] TMP_InputField inputField;
        [SerializeField] ScrollRect scrollRect;
        [SerializeField] Canvas canvas;
        [Header("AutoComplete")]
        [SerializeField] GameObject autoCompleteCommandPrefab;
        [SerializeField] Transform autoCompleteCommandParent;
        [SerializeField] GameObject autoCompleteCommandPanel;
        //utility strings
        string startOfLine = "\n> ";

        //commands
        //[SerializeField] List<ConsoleCommand> commandList;
        public Dictionary<string, IConsoleCommand> commandRegistry = new();
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
            autoCompleteCommandPanel.SetActive(false);
        }

        void RegisterCommands()
        {
            AddCommand(new TestCommand());
            AddCommand(new ClearCommand(consoleText));
            AddCommand(new HelpCommand());
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

            if (autoCompleteCommandPanel.activeSelf && Input.GetKeyDown(KeyCode.Tab))
            {
                inputField.text = autoCompleteCommandParent.GetChild(0).GetComponentInChildren<TMP_Text>().text;
            }
            
            HistoryNavigation();
        }

        public void Log(string msg)
        {
            consoleText.text += startOfLine + msg;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0;
        }

        void EvaluateCommand()
        {
            if(string.IsNullOrWhiteSpace(inputField.text))
            {
                inputField.text = string.Empty;
                inputField.ActivateInputField();
                return;
            }

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

        public void AutoCompleteCommand()
        {
            if(inputField.text == string.Empty)
            {
                //if there is children
                if(autoCompleteCommandParent.childCount > 0)
                {
                    //clear the previous autocomplete commands
                    foreach (Transform child in autoCompleteCommandParent)
                    {
                        Destroy(child.gameObject);
                    }
                }
                autoCompleteCommandPanel.SetActive(false);
                return;
            }

            autoCompleteCommandPanel.SetActive(false);
            List<IConsoleCommand> possibleCommands = new();

            //clear the previous autocomplete commands
            foreach (Transform child in autoCompleteCommandParent)
            {
                Destroy(child.gameObject);
            }

            commandRegistry.Values.ToList().ForEach(c =>
            {
                if (c.identifier.StartsWith(inputField.text))
                {
                    possibleCommands.Add(c);
                    if(autoCompleteCommandPanel.activeSelf == false)
                        autoCompleteCommandPanel.SetActive(true);
                }
            });

            possibleCommands.ForEach(c =>
            {
                GameObject go = Instantiate(autoCompleteCommandPrefab, autoCompleteCommandParent);
                go.GetComponentInChildren<TMP_Text>().text = c.identifier;
            });
        }
    }
}

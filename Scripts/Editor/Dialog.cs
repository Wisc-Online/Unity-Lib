using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FVTC.LearningInnovations.Unity.Editor
{
    public static class Dialog
    {

        public static void PromptMultiLine(string title, string message, Action<string> acceptAction)
        {
            PromptMultiLine(title, message, null, acceptAction);
        }

        public static void PromptMultiLine(string title, string message, string defaultValue, Action<string> acceptAction)
        {
            PromptMultiLine(title, message, defaultValue, acceptAction, null);
        }

        public static void PromptMultiLine(string title, string message, string defaultValue, Action<string> acceptAction, Action<string> cancelAction)
        {
            Prompt(title, message, defaultValue, acceptAction, cancelAction, true);
        }


        public static void Prompt(string title, string message, Action<string> acceptAction)
        {
            Prompt(title, message, null, acceptAction);
        }

        public static void Prompt(string title, string message, string defaultValue, Action<string> acceptAction)
        {
            Prompt(title, message, defaultValue, acceptAction, null);
        }

        public static void Prompt(string title, string message, string defaultValue, Action<string> acceptAction, Action<string> cancelAction)
        {
            Prompt(title, message, defaultValue, acceptAction, cancelAction, false);
        }

        static void Prompt(string title, string message, string defaultValue, Action<string> acceptAction, Action<string> cancelAction, bool multiLine)
        {
            PromptWindow window = null;

            Action ok = (acceptAction == null) ? null : (Action)delegate ()
            {
                acceptAction(window.Value);
            };

            Action cancel = (cancelAction == null) ? null : (Action)delegate ()
            {
                cancelAction(window.Value);
            };

            window = ScriptableObject.CreateInstance<PromptWindow>();
            window.MultiLine = multiLine;
            window.Message = message;
            window.Value = defaultValue;
            window.titleContent = new GUIContent(title);
            window.OkCallback = ok;
            window.CancelCallback = cancel;
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 150);
            window.ShowPopup();
        }


        class PromptWindow : EditorWindow
        {
            public string Message { get; set; }
            public string Value { get; set; }
            public Action OkCallback;
            public Action CancelCallback;
            public bool MultiLine;

            private void OnGUI()
            {
                GUILayout.Label(Message, EditorStyles.boldLabel);

                Value = MultiLine ? GUILayout.TextArea(Value, GUILayout.MinHeight(50f)) : GUILayout.TextField(Value);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("OK"))
                {
                    this.Close();
                    Okay();
                }

                if (GUILayout.Button("Cancel"))
                {
                    this.Close();
                    Cancel();
                }
                GUILayout.EndHorizontal();
            }

            private void Cancel()
            {
                if (CancelCallback != null)
                {
                    CancelCallback();
                }
            }

            private void Okay()
            {
                if (OkCallback != null)
                {
                    OkCallback();
                }
            }
        }

        public static bool YesNo(string title, string message)
        {
            return EditorUtility.DisplayDialog(title, message, "Yes", "No");
        }

        public static void OK(string title, string message)
        {
            EditorUtility.DisplayDialog(title, message, "OK");
        }

        public static bool OkCancel(string title, string message)
        {
            return EditorUtility.DisplayDialog(title, message, "OK", "Cancel");
        }

        public static void Close(string title, string message)
        {
            EditorUtility.DisplayDialog(title, message, "Close");
        }

        public static void ErrorSeeOutput()
        {
            EditorUtility.DisplayDialog("Error", "See console for error details.", "OK");
        }


        public static void PromptSelection(string title, string message, IEnumerable<string> options, Action<int?> callback)
        {
            DropdownSelectionWindow window = null;

            window = ScriptableObject.CreateInstance<DropdownSelectionWindow>();
            window.Message = message;
            window.titleContent = new GUIContent(title);

            window.Callback = callback;
            window.Options = options.ToArray();
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 150);
            window.ShowModal();
        }

        class DropdownSelectionWindow : EditorWindow
        {
            public string Message { get; set; }
            public bool MultiLine;

            public string[] Options { get; set; }
            public int SelectedIndex { get; set; }
            public Action<int?> Callback { get; internal set; }

            int? _selectedIndex = null;

            private void OnGUI()
            {

                SelectedIndex = EditorGUILayout.IntPopup(Message, SelectedIndex, Options, Options.Select((x, i) => i).ToArray());

                GUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(SelectedIndex < 0);
                if (GUILayout.Button("OK"))
                {
                    _selectedIndex = SelectedIndex;
                    this.Close();
                }
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("Cancel"))
                {
                    _selectedIndex = null;
                    this.Close();
                }
                GUILayout.EndHorizontal();

                GUIUtility.ExitGUI();
            }


            private void OnDestroy()
            {
                Callback(_selectedIndex);
            }
        }

    }
}

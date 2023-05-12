using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace AICommand
{
    public sealed class AICommandWindow : EditorWindow
    {
        static string WrapPrompt(string input)
            => "Xanet to team leader, bardzo sarkastyczny skurwysyn, ale czasem miły, " +
               "zawsze wesprze Cię w trudnej sytuacji, ale na codzień jednak lubi Ci dojebać. " +
               "Napisz proszę jak Xanet odpowiedziałby na następujące słowa. " +
               "nie dodawaj żadnego opisu, przejdź od razu do słów Xaneta. " +
               "Słowa na które ma odpowiedzieć to: " +
               input;

        void RunGenerator()
        {
            var code = OpenAIUtil.InvokeChat(WrapPrompt(_prompt));
            Debug.Log("AI command script:" + code);
        }


        #region Editor GUI

        string _prompt = "Create 100 cubes at random points.";

        const string ApiKeyErrorText =
            "API Key hasn't been set. Please check the project settings " +
            "(Edit > Project Settings > AI Command > API Key).";

        bool IsApiKeyOk
            => !string.IsNullOrEmpty(AICommandSettings.instance.apiKey);

        [MenuItem("Window/AI Command")]
        static void Init() => GetWindow<AICommandWindow>(true, "AI Command");

        void OnGUI()
        {
            if (IsApiKeyOk)
            {
                _prompt = EditorGUILayout.TextArea(_prompt, GUILayout.ExpandHeight(true));
                if (GUILayout.Button("Run")) RunGenerator();
            }
            else
            {
                EditorGUILayout.HelpBox(ApiKeyErrorText, MessageType.Error);
            }
        }

        #endregion
    }
} // namespace AICommand
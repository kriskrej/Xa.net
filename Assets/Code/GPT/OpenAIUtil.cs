using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

namespace AICommand
{
    static class OpenAIUtil
    {
        public static event Action<string> OnGptAnswerReady; 

        static string CreateChatRequestBody(string prompt)
        {
            var msg = new OpenAI.RequestMessage();
            msg.role = "user";
            msg.content = prompt;

            var req = new OpenAI.Request();
            req.model = "gpt-3.5-turbo";
            req.messages = new[] { msg };

            return JsonUtility.ToJson(req);
        }

        public static string InvokeChatInEditor(string prompt)
        {
            var settings = AICommandSettings.instance;

            // POST
            using var post = UnityWebRequest.Post
                (OpenAI.Api.Url, CreateChatRequestBody(prompt), "application/json");

            // Request timeout setting
            post.timeout = settings.timeout;

            // API key authorization
            post.SetRequestHeader("Authorization", "Bearer " + settings.apiKey);

            // Request start
            var req = post.SendWebRequest();

            // Progress bar (Totally fake! Don't try this at home.)
            for (var progress = 0.0f; !req.isDone; progress += 0.01f)
            {
                EditorUtility.DisplayProgressBar
                    ("AI Command", "Generating...", progress);
                System.Threading.Thread.Sleep(100);
                progress += 0.01f;
            }

            EditorUtility.ClearProgressBar();

            // Response extraction
            var json = post.downloadHandler.text;
            var data = JsonUtility.FromJson<OpenAI.Response>(json);
            return data.choices[0].message.content;
        }

        public static IEnumerator AskGptCoroutine(string prompt)
        {
            var settings = AICommandSettings.instance;
            // POST
            using var post = UnityWebRequest.Post
                (OpenAI.Api.Url, CreateChatRequestBody(prompt), "application/json");
            // Request timeout setting
            post.timeout = settings.timeout;
            // API key authorization
            post.SetRequestHeader("Authorization", "Bearer " + settings.apiKey);
            // Request start
            var req = post.SendWebRequest();
            
            var progress = 0.0f;
            while (!req.isDone) {
                yield return new WaitForSeconds(0.1f);
                progress += 0.01f;
            }

            // Response extraction
            var json = post.downloadHandler.text;
            var data = JsonUtility.FromJson<OpenAI.Response>(json);
            OnOnGptAnswerReady(data.choices[0].message.content);
        }

        private static void OnOnGptAnswerReady(string answer)
        {
            answer = answer.Replace("\"", "");
            Debug.Log(answer);
            OnGptAnswerReady?.Invoke(answer);
        }
    }
} // namespace AICommand
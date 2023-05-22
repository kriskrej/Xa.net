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

        public static IEnumerator AskGptCoroutine(string prompt)
        {
            // POST
            using var post = UnityWebRequest.Post
                (OpenAI.Api.Url, CreateChatRequestBody(prompt), "application/json");
            // Request timeout setting
            post.timeout = Credentials.timeout;
            // API key authorization
            post.SetRequestHeader("Authorization", "Bearer " + Credentials.GptApiKey);
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
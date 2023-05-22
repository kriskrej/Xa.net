using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AICommand;
using UnityEngine;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

public class XanetReader : MonoBehaviour
{
    [SerializeField] private GameObject photo1;
    [SerializeField] private GameObject photo2;
    public IEnumerator DownloadTtsFromElevenLabsCoroutine(TtsInput ttsInput)
    {
        Debug.Log("using voice");
        var apiUrl = $"https://api.elevenlabs.io/v1/text-to-speech/{ttsInput.reader}";
        var requestData = new ElevenLabsTtsRequest(ttsInput.text, 0.1f, 0.7f);
        var json = JsonUtility.ToJson(requestData);
        Debug.Log(json);
        var request = new UnityWebRequest(apiUrl, "POST");
        //var request = UnityWebRequestMultimedia.GetAudioClip(apiUrl, AudioType.MPEG);
        var bodyRaw = Encoding.UTF8.GetBytes(json);
        request.SetRequestHeader("accept", "audio/mpeg");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("xi-api-key", Credentials.elevenApiKey);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {request.error}");
        }
        else
        {
            Debug.Log("Request completed successfully.");
            var binaryResponse = request.downloadHandler.data;
            Debug.Log(binaryResponse.Length);
            string tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".mp3");
            File.WriteAllBytes(tempPath, binaryResponse);
            using (var www = UnityWebRequestMultimedia.GetAudioClip(tempPath, AudioType.MPEG))
            {
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    var clip = DownloadHandlerAudioClip.GetContent(www);
                    var src = GetComponent<AudioSource>();
                    src.clip = clip;
                    src.Play();
                    while (src.isPlaying)
                    {
                        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));
                        MoveMouth();
                    }

                    CloseMouth();
                }
            }
            File.Delete(tempPath);
        }
    }

    private void CloseMouth()
    {
        photo1.SetActive(true);
        photo2.SetActive(false);
    }

    private void MoveMouth()
    {
        photo1.SetActive(!photo1.activeSelf);
        photo2.SetActive(!photo2.activeSelf);
    }

    [Serializable]
    public class TtsInput
    {
        public string text;
        public string reader;
    }

    [Serializable]
    public class ElevenLabsTtsRequest
    {
        public string text;
        public string model_id = "eleven_multilingual_v1"; 
        public VoiceSettings voice_settings;

        public ElevenLabsTtsRequest(string txt, float stability, float similarity_boost)
        {
            this.text = txt;
            voice_settings = new VoiceSettings(stability, similarity_boost);
        }
    }

    [Serializable]
    public sealed class VoiceSettings
    {
        public float stability;
        public float similarity_boost;

        public VoiceSettings(float stability, float similarity_boost)
        {
            this.stability = stability;
            this.similarity_boost = similarity_boost;
        }
    }
}
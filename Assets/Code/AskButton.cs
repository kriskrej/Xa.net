using AICommand;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AskButton : MonoBehaviour
{
    
    [SerializeField] TMP_Text question;
    [SerializeField] AnswerTextBox answer;
    [SerializeField] Button button;
    [SerializeField] private XanetReader xanetReader;

    private void Start()
    {
        OpenAIUtil.OnGptAnswerReady += OnGptAnswerReadyHandler;
        answer.Clear();
    }

    private void OnGptAnswerReadyHandler(string gptAnswer)
    {
        answer.Type(gptAnswer);
        button.interactable = true;
        var ttsInput = new XanetReader.TtsInput()
        {
            text = gptAnswer,
            reader = "sgHWxRCmUjKb2gnTa39T" 
        };
        StartCoroutine(xanetReader.DownloadTtsFromElevenLabsCoroutine(ttsInput));
    }

    public void OnAskPressed()
    {
        StartCoroutine(OpenAIUtil.AskGptCoroutine(WrapPrompt(question.text)));
        button.interactable = false;
    }
    
    public static string WrapPrompt(string input)
        => "Xanet to team leader, bardzo sarkastyczny skurwysyn, ale czasem miły, " +
           "zawsze wesprze Cię w trudnej sytuacji, ale na codzień jednak lubi Ci dojebać. " +
           "Napisz proszę jak Xanet odpowiedziałby na następujące słowa. " +
           "nie dodawaj żadnego opisu, przejdź od razu do słów Xaneta. " +
           "Słowa na które ma odpowiedzieć to: " +
           input;

}

using System;
using System.Collections;
using System.Collections.Generic;
using AICommand;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AskButton : MonoBehaviour
{
    
    [SerializeField] TMP_Text question;
    [SerializeField] TMP_Text answer;
    [SerializeField] Button button;
    [SerializeField] private XanetReader xanetReader;

    private void Start()
    {
        OpenAIUtil.OnGptAnswerReady += OnGptAnswerReadyHandler;
    }

    private void OnGptAnswerReadyHandler(string gptAnswer)
    {
        answer.text = gptAnswer;
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
        StartCoroutine(OpenAIUtil.AskGptCoroutine(AICommandWindow.WrapPrompt(question.text)));
        button.interactable = false;
    }

}

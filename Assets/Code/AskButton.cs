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

    private void Start()
    {
        OpenAIUtil.OnGptAnswerReady += OnGptAnswerReadyHandler;
    }

    private void OnGptAnswerReadyHandler(string gptAnswer)
    {
        answer.text = gptAnswer;
        button.interactable = true;
    }

    public void OnAskPressed()
    {
        StartCoroutine(OpenAIUtil.AskGptCoroutine(AICommandWindow.WrapPrompt(question.text)));
        button.interactable = false;
    }

}

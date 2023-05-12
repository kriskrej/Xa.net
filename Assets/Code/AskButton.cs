using System;
using System.Collections;
using System.Collections.Generic;
using AICommand;
using TMPro;
using UnityEngine;

public class AskButton : MonoBehaviour
{
    [SerializeField] TMP_Text question;
    [SerializeField] TMP_Text answer;

    private void Start()
    {
        OpenAIUtil.OnGptAnswerReady += OnGptAnswerReadyHandler;
    }

    private void OnGptAnswerReadyHandler(string gptAnswer)
    {
        answer.text = gptAnswer;
    }

    public void OnAskPressed()
    {
        StartCoroutine(OpenAIUtil.AskGptCoroutine(AICommandWindow.WrapPrompt(question.text)));
    }

}

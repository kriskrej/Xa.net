using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnswerTextBox : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    public void Type(string txt)
    {
        Clear();
        StartCoroutine(TypeSentence(txt));
    }

    public void Clear()
    {
        label.text = "";
    }

    IEnumerator TypeSentence(string sentence)
    {
        for (int i = 0; i < sentence.Length; ++i)
        {
            yield return new WaitForSeconds(0.05f);
            label.text += sentence[i];
        }
    }
}
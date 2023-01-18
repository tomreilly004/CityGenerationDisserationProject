using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystemCreator : MonoBehaviour
{
    public Rule[] rules;

    public string rootSentence;

    [Range(0,10)]
    public int numOfIteration = 1;

    public string GenerateSentence(string word = null)
    {
        if(word == null)
        {
            word = rootSentence;
        }
        return GrowRecursive(word);
    }

    private string GrowRecursive(string word, int iterationIndex = 0)
    {
        if (iterationIndex >= numOfIteration)
        {
            return word;
        }
        StringBuilder resultantWord = new StringBuilder();

        foreach(var a in word)
        {
            resultantWord.Append(a);
            ProcessRulesRecursively(resultantWord, a, iterationIndex);
        }

        return resultantWord.ToString();
    }

    private void ProcessRulesRecursively(StringBuilder resultantWord, char a, int iterarionIndex)
    {
        foreach (var rule in rules)
        {
            if(rule.letter == a.ToString())
            {
                resultantWord.Append(GrowRecursive(rule.GetResult(), iterarionIndex+1));
            }
        }
    }
}

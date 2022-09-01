using Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Voxul;

public class TextScroller : ExtendedMonoBehaviour
{
    public TextMeshProUGUI Text => GetComponent<TextMeshProUGUI>();

    [Multiline]
    public List<string> Content;
    public string Seperator;
    public int MinLength = 10;
    public float Speed = 1;
    public bool Translate = true;

    private void Awake()
    {
        StartCoroutine(Tick());
    }

    private IEnumerator Tick()
    {
        var counter = 0;
        var currentText = "";
        while (true)
        {
            for (var i = 0; i < Content.Count; i++)
            {
                var line = Content[i];
                foreach (var c in line)
                {
                    counter++;
                    if (currentText.Length < MinLength)
                    {
                        currentText += c;
                        continue;
                    }
                    currentText = currentText.SafeSubstring(1) + c;
                    if (Translate)
                    {
                        Text.text = LanguageUtility.GetStringForTextMesh(currentText);
                    }
                    else
                    {
                        Text.text = currentText;
                    }
                    yield return new WaitForSeconds(Speed);
                }
                currentText += Seperator;
            }
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOutHighlight : MonoBehaviour
{
    [SerializeField] Image highlight;
    void OnEnable()
    {
        StartCoroutine(FadeInOut());       
    }

    IEnumerator FadeInOut()
    {
        float highlightAlpha = 1;
        Color newHighlightColor;
        while (true)
        {
            for (float i = 0; i < .5f; i += .01f)
            {
                newHighlightColor = highlight.color;
                highlightAlpha += i;
                newHighlightColor.a = i;
                highlight.color = newHighlightColor;
                yield return new WaitForSecondsRealtime(.01f);
            }

            for (float i = .5f; i > 0; i -= .01f)
            {
                newHighlightColor = highlight.color;
                highlightAlpha -= i;
                newHighlightColor.a = i;
                highlight.color = newHighlightColor;
                yield return new WaitForSecondsRealtime(.01f);
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

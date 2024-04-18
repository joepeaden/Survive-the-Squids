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
            for (float i = 1; i > 0; i -= .1f)
            {
                newHighlightColor = highlight.color;
                highlightAlpha -= i;
                newHighlightColor.a = i;
                highlight.color = newHighlightColor;
                yield return new WaitForSeconds(.05f);
            }

            for (float i = 0; i < 1; i += .1f)
            {
                newHighlightColor = highlight.color;
                highlightAlpha += i;
                newHighlightColor.a = i;
                highlight.color = newHighlightColor;
                yield return new WaitForSeconds(.05f);
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

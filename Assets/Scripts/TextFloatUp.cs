using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MyGame
{

    public class TextFloatUp : MonoBehaviour
    {
        public float floatSpeed;
        public float fadeIncrement;
        public float fadeSpeed;
        public TMP_Text textElement;

        public void SetData(Vector3 position, string text, Color color)
        {
            StopAllCoroutines();
            //Vector3 objScreenPos = Camera.main.WorldToScreenPoint(position);
            //GetComponent<RectTransform>().position = ;
            transform.position = new Vector3(position.x + Random.Range(-.5f, .5f), position.y + .5f, position.z);
            textElement.text = text;
            textElement.color = color;
            StartCoroutine(Fade());

            startTime = Time.time;
        }

        float startTime;

        private void Update()
        {
            transform.Translate(Vector2.up * floatSpeed * Time.deltaTime);

            //for (float i = 1; i > 0; i -= fadeIncrement)
            //{
            textElement.alpha = (1 - (Time.time - startTime));//i;
                //yield return new WaitForSecondsRealtime(fadeSpeed);
            //}

            if (Time.time - startTime < -1)
                gameObject.SetActive(false);
        }

        IEnumerator Fade()
        {
            for (float i = 1; i > 0; i -= fadeIncrement)
            {
                textElement.alpha = i;
                yield return new WaitForSecondsRealtime(fadeSpeed);
            }

            gameObject.SetActive(false);
        }
    }
}
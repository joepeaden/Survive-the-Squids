using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MyGame
{

    public class TextFloatUp : MonoBehaviour
    {
        public float floatSpeed;
        public float fadeSpeed;
        public TMP_Text textElement;

        public void SetData(Vector3 position, string text, Color color)
        {
            //Vector3 objScreenPos = Camera.main.WorldToScreenPoint(position);
            //GetComponent<RectTransform>().position = ;
            transform.position = new Vector3(position.x + Random.Range(-.5f, .5f), position.y + .5f, position.z);
            textElement.text = text;
            textElement.color = color;
            StartCoroutine(Fade());
        }

        private void Update()
        {
            transform.Translate(Vector2.up * floatSpeed * Time.deltaTime);
        }

        IEnumerator Fade()
        {
            for (float i = 1; i > 0; i -= fadeSpeed)
            {
                textElement.alpha = i;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
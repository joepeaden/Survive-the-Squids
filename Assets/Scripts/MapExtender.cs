using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class MapExtender : MonoBehaviour
    {
        float width;
        float height;
        Vector3 startPos;

        // Start is called before the first frame update
        void Start()
        {
            width = GetComponent<SpriteRenderer>().bounds.size.x;
            height = GetComponent<SpriteRenderer>().bounds.size.y;
            startPos = transform.position;

            GameManager.instance.OnGameStart.AddListener(Reset);
        }

        // Update is called once per frame
        void Update()
        {
            float xPositionDiff = transform.position.x - Camera.main.transform.position.x;
            float yPositionDiff = transform.position.y - Camera.main.transform.position.y;
            if (Mathf.Abs(xPositionDiff) > width)
            {
                transform.position = new Vector3(transform.position.x + width * (xPositionDiff < 0 ? 2 : -2), transform.position.y, transform.position.z); //);
            }

            if (Mathf.Abs(yPositionDiff) > height)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + height * (yPositionDiff < 0 ? 2 : -2), transform.position.z);//);
            }
        }

        private void Reset()
        {
            transform.position = startPos;
        }
    }
}

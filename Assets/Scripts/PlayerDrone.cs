using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class PlayerDrone : MonoBehaviour
    {
        public Transform playerT;
        public float radius = 2f;
        public float speed = 2f;

        private float angle = 0f;

        private void Start()
        {
            playerT = Player.instance.transform;
            GameplayManager.OnGameStart.AddListener(Reset);
        }

        void Reset()
        {
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(Reset);
        }

        void Update()
        {
            // Increment angle based on speed
            angle += speed * Time.deltaTime;

            // Calculate the position on the circle based on the angle
            float x = playerT.position.x + Mathf.Cos(angle) * radius;
            float y = playerT.position.y + Mathf.Sin(angle) * radius;

            // Update the position of the GameObject
            transform.position = new Vector3(x, y, playerT.position.z);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class HealthBar : MonoBehaviour
    {
        public GameObject healthPip;
        public Transform FollowTransform;
        public float heightOffset;
        Enemy theEnemy;

        Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public void SetupHealthBar(Enemy e)
        {
            for (int i = 0; i < e.HitPoints; i++)
            {
                Instantiate(healthPip, transform);
            }

            FollowTransform = e.transform;
            
            theEnemy = e;
            theEnemy.OnGetHit.AddListener(HandleHit);
        }

        public void HandleHit()
        {
            if (theEnemy.HitPoints <= 0)
            {
                Destroy(gameObject);
                return;
            }

            for (int i = theEnemy.HitPoints; i < transform.childCount; i++)
            {
                Image image = transform.GetChild(i).GetComponent<Image>();
                //if (i <= newHitPoints)
                //{
                //    continue;
                //}
                if (image.color == Color.red)
                {
                    return;
                }

                image.color = Color.red;
            }
        }

        private void FixedUpdate()
        {
            if (FollowTransform.gameObject.activeInHierarchy)
            {
                Vector3 objScreenPos = mainCamera.WorldToScreenPoint(FollowTransform.position);
                objScreenPos.y += heightOffset;

                //if (!(this as EntityMarker))
                //{
                //    // make sure we're not off-screen
                //    objScreenPos = new Vector3(Mathf.Clamp(objScreenPos.x, padding, Screen.width - padding), Mathf.Clamp(objScreenPos.y, padding, (Screen.height - padding)), objScreenPos.z);
                //}

                GetComponent<RectTransform>().position = objScreenPos;
            }
            else
            {
                // whenever the objectiveTransform is destroyed, destroy this marker.
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            theEnemy.OnGetHit.RemoveListener(HandleHit);
        }
    }
}
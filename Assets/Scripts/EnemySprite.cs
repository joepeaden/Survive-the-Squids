using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace MyGame
{
    public class EnemySprite : MonoBehaviour
    {
        public Enemy enemyScript;

        public SpriteRenderer headSprite;
        public SpriteRenderer bodySprite;
        public SpriteRenderer faceSprite;
        public Sprite deadSprite;

        [SerializeField]
        Sprite upFace;
        [SerializeField]
        Sprite downFace;
        [SerializeField]
        Sprite leftFace;
        [SerializeField]
        Sprite rightFace;

        Animator animator;
        bool isMoving;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            float animationVelocity = enemyScript.GetComponent<AIPath>().velocity.magnitude;
            if (animationVelocity <= .5)
            {
                // make sure it's negative so it's "less than zero" so that the anim controller knows we stopped
                animationVelocity = -1f;
                isMoving = false;
            }
            else if (!isMoving)
            {
                isMoving = true;
                animator.SetFloat("AnimOffset", Random.Range(0f, 1f));
            }

            animator.SetFloat("Velocity", animationVelocity);



            transform.position = enemyScript.transform.position;


            headSprite.transform.localPosition = enemyScript.transform.up * .08f;//headFollowTransform.transform.position + bodySprite.transform.localPosition;


            int newFaceSorting = 26;
            // sort body sprites based on facing
            if (enemyScript.transform.rotation.eulerAngles.z > 90 && enemyScript.transform.rotation.eulerAngles.z < 270)
            {
                // weapon pointing down
                bodySprite.sortingOrder = 24;
            }
            else
            {
                // weapon pointing up
                bodySprite.sortingOrder = 26;
            }

            if (enemyScript.transform.rotation.eulerAngles.z >= 45 && enemyScript.transform.rotation.eulerAngles.z < 135)
            {
                faceSprite.sprite = leftFace;
            }
            else if (enemyScript.transform.rotation.eulerAngles.z >= 135 && enemyScript.transform.rotation.eulerAngles.z < 225)
            {
                faceSprite.sprite = downFace;
            }
            else if (enemyScript.transform.rotation.eulerAngles.z >= 225 && enemyScript.transform.rotation.eulerAngles.z < 315)
            {
                faceSprite.sprite = rightFace;
            }
            else
            {
                faceSprite.sprite = upFace;
                newFaceSorting = 25;
            }

            faceSprite.sortingOrder = newFaceSorting;

        }

        public void HandleDeath()
        {
            //bodySprite.sprite = deadSprite;
            //faceSprite.enabled = false;
            //headSprite.enabled = false;
            //animator.enabled = false;

            //enabled = false;
        }
    }
}
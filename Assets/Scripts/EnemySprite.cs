using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

namespace MyGame
{
    public class EnemySprite : MonoBehaviour
    {
        public Enemy enemyScript;

        public SpriteRenderer headSpriteRend;
        public SpriteRenderer bodySpriteRend;
        public SpriteRenderer faceSpriteRend;
        public Sprite deadSprite;

        [SerializeField]
        Sprite upFace;
        [SerializeField]
        Sprite downFace;
        [SerializeField]
        Sprite leftFace;
        [SerializeField]
        Sprite rightFace;
        [SerializeField]
        Sprite headHitSprite;
        [SerializeField]
        Sprite bodyHitSprite;

        Sprite oldBodySprite;
        Sprite oldHeadSprite;

        Animator animator;
        bool isMoving;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void  SetData(EnemyData newData)
        {
            bodySpriteRend.sprite = newData.bodySprite;
            faceSpriteRend.sprite = newData.faceSprite;
            headSpriteRend.sprite = newData.headSprite;
            headHitSprite = newData.headHitSprite;
            bodyHitSprite = newData.bodyHitSprite;

            oldBodySprite = bodySpriteRend.sprite;
            oldHeadSprite = headSpriteRend.sprite;
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


            headSpriteRend.transform.localPosition = enemyScript.transform.up * .08f;//headFollowTransform.transform.position + bodySprite.transform.localPosition;


            int newFaceSorting = 26;
            // sort body sprites based on facing
            if (enemyScript.transform.rotation.eulerAngles.z > 90 && enemyScript.transform.rotation.eulerAngles.z < 270)
            {
                // weapon pointing down
                bodySpriteRend.sortingOrder = 24;
            }
            else
            {
                // weapon pointing up
                bodySpriteRend.sortingOrder = 26;
            }

            if (enemyScript.transform.rotation.eulerAngles.z >= 45 && enemyScript.transform.rotation.eulerAngles.z < 135)
            {
                faceSpriteRend.sprite = leftFace;
            }
            else if (enemyScript.transform.rotation.eulerAngles.z >= 135 && enemyScript.transform.rotation.eulerAngles.z < 225)
            {
                faceSpriteRend.sprite = downFace;
            }
            else if (enemyScript.transform.rotation.eulerAngles.z >= 225 && enemyScript.transform.rotation.eulerAngles.z < 315)
            {
                faceSpriteRend.sprite = rightFace;
            }
            else
            {
                faceSpriteRend.sprite = upFace;
                newFaceSorting = 25;
            }

            faceSpriteRend.sortingOrder = newFaceSorting;

        }

        public void HandleHit()
        {
            StartCoroutine(FlashHit());
        }

        IEnumerator FlashHit()
        {
            animator.SetTrigger("Hit");

            faceSpriteRend.enabled = false;
            bodySpriteRend.sprite = bodyHitSprite;
            headSpriteRend.sprite = headHitSprite;

            yield return new WaitForSeconds(.08f);

            faceSpriteRend.enabled = true;
            bodySpriteRend.sprite = oldBodySprite;
            headSpriteRend.sprite = oldHeadSprite;
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
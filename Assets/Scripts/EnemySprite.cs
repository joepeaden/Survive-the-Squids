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
        Sprite faceUSprite;
        [SerializeField]
        Sprite faceSprite;
        [SerializeField]
        Sprite faceLSprite;
        [SerializeField]
        Sprite faceRSprite;
        Sprite headHitSprite;
        Sprite bodyHitSprite;
        Sprite headHitCritSprite;
        Sprite bodyHitCritSprite;

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
            faceLSprite = newData.faceLSprite;
            faceRSprite = newData.faceRSprite;
            faceUSprite = newData.faceUSprite;
            faceSprite = newData.faceSprite;
            headSpriteRend.sprite = newData.headSprite;
            headHitSprite = newData.headHitSprite;
            bodyHitSprite = newData.bodyHitSprite;
            headHitCritSprite = newData.headCritSprite;
            bodyHitCritSprite = newData.bodyCritSprite;

            oldBodySprite = bodySpriteRend.sprite;
            oldHeadSprite = headSpriteRend.sprite;

            // reset stuff in case it got wonky
            faceSpriteRend.enabled = true;
            transform.localScale = new Vector3(1, 1, 1);
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


            headSpriteRend.transform.localPosition = enemyScript.transform.up * .05f;//headFollowTransform.transform.position + bodySprite.transform.localPosition;


            int newFaceSorting = 26;
            int newHeadSorting = 25;
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
                faceSpriteRend.sprite = faceLSprite;
            }
            else if (enemyScript.transform.rotation.eulerAngles.z >= 135 && enemyScript.transform.rotation.eulerAngles.z < 225)
            {
                faceSpriteRend.sprite = faceSprite;
            }
            else if (enemyScript.transform.rotation.eulerAngles.z >= 225 && enemyScript.transform.rotation.eulerAngles.z < 315)
            {
                faceSpriteRend.sprite = faceRSprite;
            }
            else
            {
                faceSpriteRend.sprite = faceUSprite;
                newHeadSorting = 23;
                newFaceSorting = 25;
            }

            headSpriteRend.sortingOrder = newHeadSorting;
            faceSpriteRend.sortingOrder = newFaceSorting;

        }

        public void HandleHit(bool isCrit)
        {
            StartCoroutine(FlashHit(isCrit));
        }

        IEnumerator FlashHit(bool isCrit)
        {
            animator.SetTrigger("Hit");

            faceSpriteRend.enabled = false;
            bodySpriteRend.sprite = isCrit ? bodyHitCritSprite :  bodyHitSprite;
            headSpriteRend.sprite = isCrit ? headHitCritSprite : headHitSprite;

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

            GameObject corpse = ObjectPool.instance.GetCorpse();
            corpse.transform.position = transform.position;
            corpse.SetActive(true);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    public class CharacterSprite : MonoBehaviour
    {
        [SerializeField]
        SpriteRenderer faceSprite;
        [SerializeField]
        SpriteRenderer headSprite;
        [SerializeField]
        SpriteRenderer bodySprite;
        [SerializeField]
        Transform charBody;

        [SerializeField]
        Sprite upFace;
        [SerializeField]
        Sprite downFace;
        [SerializeField]
        Sprite leftFace;
        [SerializeField]
        Sprite rightFace;

        [SerializeField]
        Transform headFollower;

        Animator animator;
        bool isMoving;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            float animationVelocity = transform.parent.parent.GetComponent<Rigidbody2D>().velocity.magnitude;
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

            int newFaceSorting = 26;
            // sort body sprites based on facing
            if (charBody.rotation.eulerAngles.z > 90 && charBody.rotation.eulerAngles.z < 270)
            {
                // weapon pointing down
                bodySprite.sortingOrder = 24;
            }
            else
            {
                // weapon pointing up
                bodySprite.sortingOrder = 26;
            }

            if (charBody.rotation.eulerAngles.z >= 45 && charBody.rotation.eulerAngles.z < 135)
            {
                faceSprite.sprite = leftFace;
            }
            else if (charBody.rotation.eulerAngles.z >= 135 && charBody.rotation.eulerAngles.z < 225)
            {
                faceSprite.sprite = downFace;
            }
            else if (charBody.rotation.eulerAngles.z >= 225 && charBody.rotation.eulerAngles.z < 315)
            {
                faceSprite.sprite = rightFace;
            }
            else
            {
                faceSprite.sprite = upFace;
                newFaceSorting = 24;
            }

            faceSprite.sortingOrder = newFaceSorting;

            headSprite.gameObject.transform.position = headFollower.position;
        }

    }
}

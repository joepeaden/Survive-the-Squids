using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class WeaponSprite : MonoBehaviour
    {
        [SerializeField]
        Animator gunBlastAnim;

        [SerializeField]
        Animator gunMoveAnim;

        [SerializeField]
        GameObject shellCasing;

        bool isFacingLeft = false;
        SpriteRenderer rend;

        void Awake()
        {
            //gunBlastAnim = GetComponent<Animator>();
            rend = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {

            if (transform.parent.rotation.eulerAngles.z > 90 && transform.parent.rotation.eulerAngles.z < 270)
            {
                // weapon pointing down, put it in front of actor
                rend.sortingOrder = 31;
            }
            else
            {
                // weapon pointing up, put it behind actor
                rend.sortingOrder = 22;
            }

            if (transform.parent.rotation.eulerAngles.z > 180 && transform.parent.rotation.eulerAngles.z < 360)
            {
                Quaternion q = Quaternion.Euler(new Vector3(0, 180, 90));
                transform.localRotation = q;
                gunMoveAnim.SetBool("isFacingLeft", false);
                isFacingLeft = false;
            }
            else
            {
                Quaternion q = Quaternion.Euler(new Vector3(0, 0, 90));
                transform.localRotation = q;
                gunMoveAnim.SetBool("isFacingLeft", true);
                isFacingLeft = true;
            }
        }

        public void SetWeapon(WeaponData newWeapon)
        {
            if (rend == null)
            {
                rend = GetComponent<SpriteRenderer>();
            }

            rend.sprite = newWeapon.weaponSprite;
        }

        public void PlayFireAnim()
        {
            gunBlastAnim.Play("HumanGunBlast", -1, 0);

            PlayShellCasingAnim();

            if (!isFacingLeft)
            {
                gunMoveAnim.Play("GunKicking", -1, 0);
            }
            else
            {
                gunMoveAnim.Play("GunKickingLeft", -1, 0);
            }
        }

        void PlayShellCasingAnim()
        {
            GameObject casing = Instantiate(shellCasing, transform.position, transform.rotation);
            casing.GetComponent<Rigidbody2D>().AddForce(-transform.parent.up * Random.Range(200f, 500f));
            if (isFacingLeft)
            {
                casing.GetComponent<Rigidbody2D>().AddForce(-transform.parent.right * Random.Range(150f, 200f));
            }
            else
            {
                casing.GetComponent<Rigidbody2D>().AddForce(transform.parent.right * Random.Range(150f, 200f));
            }

            casing.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-500f, 500f));
        }
    }
}
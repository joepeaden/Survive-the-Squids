using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MyGame
{
    public class PlayerBar : MonoBehaviour
    {
        public static PlayerBar Instance;
        public GameObject healthPip;
        public Transform healthParent;
        public Transform xpParent;
        public GameObject xpPip;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            for (int i = 0; i < healthParent.childCount; i++)
            {
                Destroy(healthParent.GetChild(i).gameObject);
            }

            for (int i = 0; i < Player.instance.HitPoints; i++)
            {
                Instantiate(healthPip, healthParent);
            }
        }

        public void HandleHit()
        {
            if (Player.instance.HitPoints < 0)
            {
                return;
            }

            for (int i = Player.instance.HitPoints; i < healthParent.childCount; i++)
            {
                Image image = healthParent.GetChild(i).GetComponent<Image>();

                if (image.color == Color.red)
                {
                    return;
                }

                image.color = Color.red;
            }
        }

        public void HandleXP(int xpCount, int xpToLevel)
        {
            // can optimize this later it can just add pips instead of deleting them all
            if (xpToLevel != xpParent.childCount)
            {
                for (int i = 0; i < xpParent.childCount; i++)
                {
                    Destroy(xpParent.GetChild(i).gameObject);
                }

                for (int i = 0; i < xpToLevel; i++)
                {
                    Instantiate(xpPip, xpParent);
                }
            }

            if (xpCount != 0)
            {
                xpParent.GetChild(xpCount - 1).GetComponent<Image>().color = Color.yellow;
            }
            //for (int i = xpCount; i < xpParent.childCount; i++)
            //{
            //    Image image = healthParent.GetChild(i).GetComponent<Image>();

            //    if (image.color == Color.yellow)
            //    {
            //        return;
            //    }

            //    image.color = Color.yellow;
            //}
        }
    }
}

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

            GameplayManager.OnGameStart.AddListener(Reset);
        }

        private void OnDestroy()
        {
            GameplayManager.OnGameStart.RemoveListener(Reset);
        }

        private void Reset()
        {
            for (int i = 0; i < xpParent.childCount; i++)
            {
                Destroy(xpParent.GetChild(i).gameObject);
            }
        }

        public void InitializeHP()
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
            if (xpToLevel > xpParent.childCount)
            {
                for (int i = 0; i < xpToLevel; i++)
                {
                    if (xpParent.childCount > i)
                    {
                        xpParent.GetChild(i).GetComponent<Image>().color = Color.black;
                    }

                    if (i < (xpToLevel - xpParent.childCount))
                    {
                        Instantiate(xpPip, xpParent);
                    }
                }
            }
            //else if (xpToLevel < xpParent.childCount)
            //{

            //}

            for (int i = 0; i < xpToLevel; i++)
            {
                if (xpCount > i)
                {
                    xpParent.GetChild(i).GetComponent<Image>().color = Color.cyan;
                }
            }

            //if (xpCount != 0)
            //{
            //    xpParent.GetChild(xpCount - 1).GetComponent<Image>().color = Color.cyan;
            //}
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

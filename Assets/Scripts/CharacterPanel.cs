using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace MyGame
{

    public class CharacterPanel : MonoBehaviour
    {
        public TMP_Text charName;
        //public TMP_Text weaponName;
        //public TMP_Text reflexSpeed;
        //public TMP_Text defaultTextItem;

        public void SetCharacter(CharacterInfo charInfo)
        {
            charName.text = charInfo.charName + "  (LVL " + charInfo.level.ToString() + ")";
            //weaponName.text = charInfo.weaponData.weaponName;
            //reflexSpeed.text = "Reflex Speed: " + charInfo.ReflexSpeed.ToString();

            //defaultTextItem.gameObject.SetActive(false);

            //foreach (CharacterInfo.CharTraits theTrait in charInfo.traits)
            //{
            //    TMP_Text newTextEntry = Instantiate(defaultTextItem, defaultTextItem.transform.parent).GetComponent<TMP_Text>();
            //    newTextEntry.gameObject.SetActive(true);
            //    newTextEntry.text = Player.instance.traitEnumToData[theTrait].traitName;
            //}
        }
    }
}
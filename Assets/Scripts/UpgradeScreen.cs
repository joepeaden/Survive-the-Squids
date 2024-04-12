using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradeScreen : MonoBehaviour
{
    public GameObject characterPanelPrefab;
    public Player player;

    private void OnEnable()
    {
        if (player == null)
        {
            player = Player.instance;
        }

        // DESTROY THE CHILDREN!
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }

        foreach (CharacterBody charBody in player.CharacterBodies)
        {
            CharacterPanel charPanel = Instantiate(characterPanelPrefab, transform).GetComponent<CharacterPanel>();
            charPanel.SetCharacter(charBody.CharInfo);
        }
    }

}

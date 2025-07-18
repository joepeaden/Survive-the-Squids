using UnityEngine;

namespace MyGame
{
    [CreateAssetMenu(fileName = "PickupData", menuName = "MyScriptables/PickupData")]
    public class PickupData : ScriptableObject
    {
        public Sprite sprite;
        public AudioClip pickupSound;
        public int XPValue;
        public float magnetSpeed;
    }
}

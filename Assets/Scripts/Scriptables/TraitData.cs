using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    [CreateAssetMenu(fileName = "TraitData", menuName = "MyScriptables/TraitData")]
    public class TraitData : ScriptableObject
    {
        public string traitName;
        public string description;
        public float value;
    }
}
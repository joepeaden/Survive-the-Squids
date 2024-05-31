using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{

    [CreateAssetMenu(fileName = "CompanyBuffData", menuName = "MyScriptables/CompanyBuffData")]
    public class CompanyBuffData : ScriptableObject
    {
        public CompanyBuffTypes buffType;
        public float value;
    }

    public enum CompanyBuffTypes
    {
        Speed,
        XPBoost,
        PickupRadius
    }
}
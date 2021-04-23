using System.Collections.Generic;
using UnityEngine;

namespace Knife.Tools
{
    [CreateAssetMenu(fileName = "Decal Templates Kit", menuName = "Knife/Decal Templates Kit", order = 1000)]
    public class DecalTemplatesKit : ScriptableObject
    {
        public List<DecalPlacementTool.DecalTemplate> Templates;
    }
}
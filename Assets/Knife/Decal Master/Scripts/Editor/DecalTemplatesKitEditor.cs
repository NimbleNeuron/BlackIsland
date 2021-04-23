using UnityEditor;

namespace Knife.Tools
{
    [CustomEditor(typeof(DecalTemplatesKit))]
    public class DecalTemplatesKitEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }
    }
}
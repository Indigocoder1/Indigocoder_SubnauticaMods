using UnityEngine;


namespace TextureReplacerEditor.Monobehaviors.ConfigChangeHandlers
{
    internal class VectorChangeHandler : ConfigChangeHandler
    {
        public VectorDisplay oldVector;
        public VectorDisplay newVector;

        public override void SetInfo(object original, object changed)
        {
            oldVector.SetVector((Vector4)original);
            newVector.SetVector((Vector4)changed);
        }
    }
}

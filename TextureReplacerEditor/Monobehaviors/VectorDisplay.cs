using TMPro;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors
{
    internal class VectorDisplay : MonoBehaviour
    {
        public TextMeshProUGUI xText;
        public TextMeshProUGUI yText;
        public TextMeshProUGUI zText;
        public TextMeshProUGUI wText;

        public void SetVector(Vector4 vector)
        {
            xText.text = $"<color=#FF0000>X:</color> {vector.x.ToString("F2")},";
            yText.text = $"<color=#00FF00>Y:</color> {vector.y.ToString("F2")},";
            zText.text = $"<color=#0000FF>Z:</color> {vector.z.ToString("F2")},";
            wText.text = $"<color=#FFFFFF>W:</color> {vector.w.ToString("F2")}";
        }
    }
}

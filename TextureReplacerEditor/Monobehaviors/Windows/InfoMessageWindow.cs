using TMPro;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors.Windows
{
    internal class InfoMessageWindow : DraggableWindow
    {
        public TextMeshProUGUI messageText;

        public void SetMessage(string text, Color color)
        {
            messageText.text = text;
            messageText.color = color;
        }
    }
}

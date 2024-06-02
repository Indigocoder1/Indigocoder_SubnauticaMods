using TMPro;

namespace TextureReplacerEditor.Monobehaviors.ConfigChangeHandlers
{
    internal class FloatChangeHandler : ConfigChangeHandler
    {
        public TextMeshProUGUI oldValueText;
        public TextMeshProUGUI newValueText;

        public override void SetInfo(object original, object changed)
        {
            oldValueText.text = ((float)original).ToString();
            newValueText.text = ((float)changed).ToString();
        }
    }
}

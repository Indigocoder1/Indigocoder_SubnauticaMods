using System;
using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    public class CustomCraftFormatter : MonoBehaviour
    {
        private CustomCraftVersion customCraftVersion = CustomCraftVersion.NotSet;

        public void SetVersion(int version)
        {
            switch (version)
            {
                case -1:
                    customCraftVersion = CustomCraftVersion.NotSet;
                    break;
                case 2:
                    customCraftVersion = CustomCraftVersion.CC2;
                    break;
                case 3:
                    customCraftVersion = CustomCraftVersion.CC3;
                    break;
                default:
                    Debug.LogError($"Invalid version number ({version}) trying to be assigned!");
                    break;
            }
        }
    }

    [Serializable]
    public enum CustomCraftVersion
    {
        NotSet,
        CC2,
        CC3
    }
}

using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    public class Menu : MonoBehaviour
    {
        public Menu subMenuParent;
        public string menuName;
        private bool isOpen;

        private void Start()
        {
            if (gameObject.activeSelf)
            {
                isOpen = true;
            }
        }

        public void Open()
        {
            isOpen = true;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            isOpen = false;
            gameObject.SetActive(false);
        }

        public bool IsOpen()
        {
            return isOpen;
        }
    }
}
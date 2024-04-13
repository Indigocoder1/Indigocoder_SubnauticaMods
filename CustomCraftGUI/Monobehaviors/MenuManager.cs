using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomCraftGUI.Monobehaviors
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;
        [SerializeField] private Menu[] menus;

        private void Awake()
        {
            Instance = this;
        }

        public void OpenMenu(string menuName)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].name == menuName)
                {
                    OpenMenu(menus[i]);
                }
                else if (menus[i].IsOpen())
                {
                    CloseMenu(menus[i]);
                }
            }
        }

        public void OpenMenu(Menu menu)
        {
            for (int i = 0; i < menus.Length; i++)
            {
                if (menus[i].IsOpen())
                {
                    CloseMenu(menus[i]);
                }
            }
            menu.Open();
        }

        public void CloseMenu(Menu menu)
        {
            menu.Close();
        }

        public void QuitApp()
        {
            Application.Quit();
        }
    }
}
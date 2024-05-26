using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TodoList.Monobehaviors
{
    public class TodoItem : MonoBehaviour
    {
        public static List<TodoItem> todoItems = new();

        public TodoInputField todoInputField;
        public Toggle toggle;
        public SaveData saveData
        {
            get
            {
                return new(itemText, isCompleted);
            }
            set
            {
                itemText = value.itemBody;
                isCompleted = value.isCompleted;
            }
        }

        public string itemText
        {
            get
            {
                return todoInputField.inputField.textComponent.text;
            }
            set
            {
                todoInputField.inputField.text = value;
            }
        }

        public bool isCompleted
        {
            get
            {
                return toggle.isOn;
            }
            set
            {
                todoInputField.OnToggleChanged(value);
                toggle.isOn = value;
            }
        }

        private void Start()
        {
            toggle.onValueChanged.AddListener((bool val) => todoInputField.OnToggleChanged(val));
            todoInputField = GetComponentInChildren<TodoInputField>();
            toggle = GetComponentInChildren<Toggle>();
        }

        private void OnEnable()
        {
            if (!todoItems.Contains(this))
            {
                todoItems.Add(this);
            }
        }

        private void OnDisable()
        {
            if (todoItems.Contains(this))
            {
                todoItems.Remove(this);
            }
        }

        private void OnDestroy()
        {
            if (todoItems.Contains(this))
            {
                todoItems.Remove(this);
            }
        }
    }
}

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

        public Color hintCheckmarkColor;
        public Color normalCheckmarkColor;
        public Image backgroundImage;
        public Image checkmarkBox;
        public Image checkmarkCheck;

        public bool isHintItem { get; private set; }

        public ItemSaveData saveData
        {
            get
            {
                return new(itemText, isCompleted, isHintItem);
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

        public void SetIsHintItem(bool isHintItem)
        {
            this.isHintItem = isHintItem;
            Color checkboxColor = isHintItem ? hintCheckmarkColor : normalCheckmarkColor;
            backgroundImage.color = isHintItem ? hintCheckmarkColor : Color.white;
            checkmarkBox.color = checkboxColor;
            checkmarkCheck.color = checkboxColor;
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

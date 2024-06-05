using System;
using System.Collections.Generic;
using UnityEngine;

namespace TextureReplacerEditor.Monobehaviors
{
    internal class TutorialHandler : MonoBehaviour
    {
        public float minItemDelay;

        private Dictionary<string, Action> tutorialItems = new();
        private List<Action> queuedActions = new();
        private List<string> completedActions = new();
        private float currentItemDelay;

        private void Start()
        {
            currentItemDelay = minItemDelay;
        }

        public void AddTutorialItem(string key, Action onComplete)
        {
            if (tutorialItems.ContainsKey(key)) return;

            tutorialItems.Add(key, onComplete);
        }

        public bool TriggerTutorialItem(string trigger)
        {
            if (completedActions.Contains(trigger)) return false;

            if (tutorialItems.TryGetValue(trigger, out Action action))
            {
                queuedActions.Add(action);
                completedActions.Add(trigger);
                tutorialItems.Remove(trigger);
                return true;
            }

            return false;
        }

        public void RemoveAllItems()
        {
            tutorialItems.Clear();
            queuedActions.Clear();
            completedActions.Clear();
        }

        private void Update()
        {
            if (queuedActions.Count <= 0) return;

            if(currentItemDelay < minItemDelay)
            {
                currentItemDelay += Time.unscaledDeltaTime;
                return;
            }

            queuedActions[0]();
            queuedActions.RemoveAt(0);
            currentItemDelay = 0;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnThisCamera : MonoBehaviour
{
    public List<GameObject> objectsToShow = new List<GameObject>();
    public List<Action> preCullActions = new List<Action>();
    public List<Action> postCullActions = new List<Action>();

    private Dictionary<GameObject, bool> previousStates = new Dictionary<GameObject, bool>();

    private void Start()
    {
        objectsToShow.Clear();

        preCullActions.Add(() => Player.main.SetHeadVisible(true));
        postCullActions.Add(() => Player.main.SetHeadVisible(false));
    }

    private void OnPreCull()
    {
        foreach (var item in objectsToShow)
        {
            if(!previousStates.ContainsKey(item))
            {
                previousStates.Add(item, item.activeSelf);
            }

            previousStates[item] = item.activeSelf;
            item?.SetActive(true);
        }

        foreach (var action in preCullActions)
        {
            action.Invoke();
        }
    }

    private void OnPostRender()
    {
        foreach (var item in objectsToShow)
        {
            item?.SetActive(previousStates[item]);

            if (previousStates.ContainsKey(item))
            {
                previousStates.Remove(item);
            }
        }

        foreach (var action in postCullActions)
        {
            action.Invoke();
        }
    }
}

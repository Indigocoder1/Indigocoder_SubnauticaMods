using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HideOnThisCamera : MonoBehaviour
{
    public List<GameObject> objectsToHide;

    private Dictionary<GameObject, bool> previousStates = new Dictionary<GameObject, bool>();

    private void OnPreCull()
    {
        foreach (var item in objectsToHide.ToList())
        {
            if (!previousStates.ContainsKey(item))
            {
                previousStates.Add(item, item.activeSelf);
            }

            previousStates[item] = item.activeSelf;
            item.SetActive(false);
        }
    }

    private void OnPostRender()
    {
        foreach (var item in objectsToHide)
        {
            item.SetActive(previousStates[item]);

            if (previousStates.ContainsKey(item))
            {
                previousStates.Remove(item);
            }
        }
    }
}

using MirrorMod;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisableWhenNotInView : MonoBehaviour
{
    //CURRENTLY DEPRECATED
    public GameObject mirrorRoot;
    public Camera mirrorCamera;

    public List<Renderer> mirrorRenderers;
    public List<GameObject> objectsToDisable = new List<GameObject>();

    private Camera mainCamera;
    private RenderTexture renderTexture;

    private Renderer renderPlane;
    
    private void Start()
    {
        mainCamera = Camera.main;
        renderTexture = new RenderTexture(Main_Plugin.MirrorResolution.Value, Main_Plugin.MirrorResolution.Value, 1);
        mirrorCamera.targetTexture = renderTexture;

        Transform targetPlane = transform.Find("IgnoreOnOtherMirrors/TargetPlane");
        renderPlane = targetPlane.Find("RenderPlane").GetComponent<Renderer>();

        renderPlane.material.SetTexture("_MainTex", renderTexture);
    }

    private void Update()
    {
        CheckIfVisible();
    }

    private void CheckIfVisible()
    {
        int amountNotVisible = 0;

        foreach (Renderer rend in mirrorRenderers)
        {
            if (!IsVisible(rend, mainCamera))
            {
                amountNotVisible++;
            }
        }

        if (amountNotVisible == mirrorRenderers.Count)
        {
            SetObjectsActive(false);
        }
        else
        {
            SetObjectsActive(true);
        }
    }

    private bool IsVisible(Renderer renderer, Camera cam)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

        if (GeometryUtility.TestPlanesAABB(planes, renderer.bounds))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetObjectsActive(bool isActive)
    {
        for (int i = 0; i < objectsToDisable.Count; i++)
        {
            if (objectsToDisable[i].activeSelf != isActive)
            {
                objectsToDisable[i].SetActive(isActive);
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine(nameof(LateAddComponents));
    }

    private void OnDisable()
    {
        HideOnThisCamera[] components = FindObjectsOfType<HideOnThisCamera>();

        foreach (var item in components)
        {
            if (item.objectsToHide.Contains(mirrorRoot))
            {
                item.objectsToHide.Remove(mirrorRoot);
            }
        }
    }

    private IEnumerator LateAddComponents()
    {
        yield return new WaitForSeconds(0.1f);

        HideOnThisCamera[] components = FindObjectsOfType<HideOnThisCamera>();

        foreach (var item in components)
        {
            if(item == GetComponentInChildren<HideOnThisCamera>())
            {
                continue;
            }

            if (!item.objectsToHide.Contains(mirrorRoot))
            {
                item.objectsToHide.Add(mirrorRoot);
            }
        }
    }

    private void OnDestroy()
    {
        renderPlane.material.SetTexture("_MainTex", null);
        mirrorCamera.targetTexture = null;

        renderTexture.Release();
    }
}
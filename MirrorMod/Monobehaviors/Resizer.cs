using MirrorMod;
using UnityEngine;

public class Resizer : MonoBehaviour
{
    public Mirror mirror;
    public Transform top;
    public Transform bottom;
    public Transform left;
    public Transform right;

    private Vector3 initialScale;
    private Vector3 scaleLastFrame;
    private bool handlesVisible = true;

    private void Start()
    {
        initialScale = transform.localScale;
        UpdateScale();
    }

    private void Update()
    {
        if(Input.GetKeyDown(Main_Plugin.ResizeHandleToggleButton.Value))
        {
            handlesVisible = !handlesVisible;

            if (top.gameObject.activeSelf != handlesVisible) { top.gameObject.SetActive(handlesVisible); }
            if (bottom.gameObject.activeSelf != handlesVisible) { bottom.gameObject.SetActive(handlesVisible); }
            if (left.gameObject.activeSelf != handlesVisible) { left.gameObject.SetActive(handlesVisible); }
            if (right.gameObject.activeSelf != handlesVisible) { right.gameObject.SetActive(handlesVisible); }
        }   
    }

    private void LateUpdate()
    {
        if(!handlesVisible)
        {
            return;
        }

        UpdateScale();
    }

    private void UpdateScale()
    {
        float xPos = (left.position.x + right.position.x) / 2;
        float yPos = (top.position.y + bottom.position.y) / 2;
        transform.position = new Vector3(xPos, yPos, transform.position.z);

        float xScale = Mathf.Abs(right.localPosition.x - left.localPosition.x);
        float yScale = Mathf.Abs(top.localPosition.y - bottom.localPosition.y);
        Vector3 newScale = new Vector3(xScale / 2, yScale / 2, initialScale.z);
        transform.localScale = newScale;

        if (newScale != scaleLastFrame)
        {
            //mirror.CreateMirrorBounds();
            //mirror.InitializeRenderTexture();
        }

        scaleLastFrame = newScale;
    }
}

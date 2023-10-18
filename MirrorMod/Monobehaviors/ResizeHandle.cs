using MirrorMod;
using UnityEngine;

public class ResizeHandle : MonoBehaviour, IHandTarget
{
    public Transform root;
    public bool allowX;
    public bool allowY;
    public bool onlyPositive;
    public bool onlyNegative;

    private Vector3 mousePos;
    private float minSize = 0.5f;
    private Transform lastHandPosition;

    public void OnHandClick(GUIHand hand)
    {
        Main_Plugin.logger.LogInfo(hand.transform.position);
    }

    public void OnHandHover(GUIHand hand)
    {
        Main_Plugin.logger.LogInfo(hand.transform.position);
    }

    private Vector3 GetMousePosition()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {
        mousePos = Input.mousePosition - GetMousePosition();
    }

    private void OnMouseDrag()
    {
        Vector3 globalPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z);
        Vector3 pos = Camera.main.ScreenToWorldPoint(globalPos);
        Vector3 localPos = root.InverseTransformPoint(pos);

        float posX = allowX ? -localPos.x : transform.localPosition.x;
        float posY = allowY ? localPos.y : transform.localPosition.y;

        if(onlyPositive)
        {
            posX = allowX ? Mathf.Max(minSize, posX) : posX;
            posY = allowY ? Mathf.Max(minSize, posY) : posY;
        }

        if(onlyNegative)
        {
            posX = allowX ? Mathf.Min(-minSize, posX) : posX;
            posY = allowY ? Mathf.Min(-minSize, posY) : posY;
        }

        Vector3 newPosition = new Vector3(posX, posY, transform.localPosition.z);
        transform.localPosition = newPosition;
    }
}

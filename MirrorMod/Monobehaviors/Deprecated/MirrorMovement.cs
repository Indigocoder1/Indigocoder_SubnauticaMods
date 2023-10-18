using UnityEngine;

public class MirrorMovement : MonoBehaviour
{
    //CURRENTLY DEPRECATED
    public Transform mirrorTarget;
    public Transform mirror;

    private void Start()
    {
        if(mirrorTarget == null)
        {
            mirrorTarget = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        Vector3 localPlayerPos = mirror.InverseTransformPoint(mirrorTarget.position);
        transform.position = mirror.TransformPoint(new Vector3(localPlayerPos.x, localPlayerPos.y, -localPlayerPos.z));

        Vector3 lookAtMirror = mirror.TransformPoint(new Vector3(-localPlayerPos.x, -localPlayerPos.y, localPlayerPos.z));

        transform.LookAt(lookAtMirror);
    }
}

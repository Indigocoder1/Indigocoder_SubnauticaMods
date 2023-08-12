using System;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public static event EventHandler OnMirrorSwitchedState;
    public static Dictionary<Mirror, MirrorInfo> mirrorInfos = new Dictionary<Mirror, MirrorInfo>();
    public static List<Mirror> mirrorList = new List<Mirror>();

    public Camera mirrorCam;
    public Transform mirrorTarget;
    public Transform renderPlane;
    public GameObject mirrorRenderersParent;

    private RenderTexture renderTexture;

    private Transform _pa;
    private Transform _pb;
    private Transform _pc;
    private Transform _pe;
    private float frustumScale = 1f;

    private Bounds mirrorBounds;

    private void Start()
    {
        if (mirrorTarget == null)
        {
            mirrorTarget = Camera.main.transform;
        }

        InitializeMirrorBounds();
        InitializeRenderTexture();
        InitializeNearPlanePoints();
        InitializeStaticVariables();
    }

    private void LateUpdate()
    {
        MirrorMovementAndRotation();
        SetNearPlane();
        CheckIfVisible();
    }

    private void MirrorMovementAndRotation()
    {
        Vector3 localPlayerPos = transform.InverseTransformPoint(mirrorTarget.position);
        mirrorCam.transform.position = transform.TransformPoint(new Vector3(localPlayerPos.x, localPlayerPos.y, -localPlayerPos.z));

        Vector3 lookAtMirror = transform.TransformPoint(new Vector3(-localPlayerPos.x, -localPlayerPos.y, localPlayerPos.z));

        mirrorCam.transform.LookAt(lookAtMirror);
    }

    private void SetNearPlane()
    {
        //Article "followed": https://edom18.medium.com/implementation-of-generalized-perspective-projection-on-the-unity-c9472a94f083

        float n = mirrorCam.nearClipPlane;
        float f = mirrorCam.farClipPlane;

        Vector3 pe = _pe.position;

        Vector3 pa = _pa.position;
        Vector3 pb = _pb.position;
        Vector3 pc = _pc.position;

        // Compute an orthonormal basis for the screen.
        Vector3 vr = (pb - pa).normalized;
        Vector3 vu = (pc - pa).normalized;
        Vector3 vn = Vector3.Cross(vu, vr).normalized;

        // Compute the screen corner vectors.
        Vector3 va = pa - pe;
        Vector3 vb = pb - pe;
        Vector3 vc = pc - pe;

        // Find the distance from the eye to screen plane.
        float d = -Vector3.Dot(va, vn);

        // Find the extent of the perpendicular projection.
        //(How far on each side of the midpoint the plane extends)
        float nd = n / d * frustumScale;
        float l = Vector3.Dot(vr, va) * nd;
        float r = Vector3.Dot(vr, vb) * nd;
        float b = Vector3.Dot(vu, va) * nd;
        float t = Vector3.Dot(vu, vc) * nd;

        // Load the perpendicular projection.
        Matrix4x4 P = Matrix4x4.Frustum(l, r, b, t, n, f);

        mirrorCam.projectionMatrix = P;
        _pe.rotation = Quaternion.LookRotation(-vn, vu);
        mirrorCam.nearClipPlane = d;
    }

    private void CheckIfVisible()
    {
        //Bounds for the renderer of this mirror
        Bounds mirrorBounds = mirrorInfos[this].rendererBounds;

        bool visibleFromMirror = false;
        bool visibleFromCamera = false;
        List<Mirror> mirrorsVisibleFrom = new List<Mirror>();

        for (int i = 0; i < mirrorList.Count; i++)
        {
            Mirror mirror = mirrorList[i];

            if (mirror == this)
            {
                continue;
            }

            MirrorInfo mirrorInfo = mirrorInfos[mirror];

            //Is visible from another mirror's camera?
            if(VisibleFromCamera(mirrorInfo.mirrorCam, mirrorBounds))
            {
                visibleFromMirror = true;
                mirrorsVisibleFrom.Add(mirror);
            }
        }

        //Check if visible from main camera if it's not visible from other mirrors
        bool visibleFromMainCamera = VisibleFromCamera(Camera.main, mirrorBounds);

        MirrorInfo info = mirrorInfos[this];
        info.visibleInfo.visibleFromMirrorCamera = visibleFromMirror;
        info.visibleInfo.visibleFromMainCamera = visibleFromMainCamera;
        mirrorInfos[this] = info;

        //Merge the 2 visibleFroms
        visibleFromCamera = visibleFromMirror || visibleFromMainCamera;

        Debug.Log($"Visible from main = {visibleFromMainCamera}");
        if (visibleFromMainCamera == false)
        {
            bool otherMirrorIsVisible = true;

            for (int i = 0; i < mirrorsVisibleFrom.Count; i++)
            {
                //Check if the mirrors this is visible from are in the main camera
                //This makes it so the mirrors are actually disabled when looking away
                VisibleInfo visibleInfo = mirrorInfos[mirrorsVisibleFrom[i]].visibleInfo;
                if(visibleInfo.visibleFromMainCamera == false)
                {
                    otherMirrorIsVisible = false;
                }
            }

            if(otherMirrorIsVisible == false)
            {
                visibleFromCamera = false;
            }

            Debug.Log($"Other mirror is visible = {otherMirrorIsVisible}");
        }

        mirrorRenderersParent.SetActive(visibleFromCamera);
    }

    private void InitializeMirrorBounds()
    {
        Bounds tempBounds = new Bounds();
        Renderer[] renderers = mirrorRenderersParent.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer each = renderers[i];

            if (i == 0)
            {
                tempBounds = each.bounds;
            }
            else
            {
                tempBounds.Encapsulate(each.bounds);
            }
        }
        mirrorBounds = tempBounds;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(mirrorBounds.center, mirrorBounds.extents * 2);
    }
#endif

    private void InitializeRenderTexture()
    {
        int renderTextureSize = 900;
        renderTexture = new RenderTexture(renderTextureSize, renderTextureSize, 1, UnityEngine.Experimental.Rendering.DefaultFormat.LDR);
        renderTexture.name = name + "_RenderTexture";

        mirrorCam.targetTexture = renderTexture;

        Renderer planeRend = renderPlane.GetComponent<Renderer>();
        planeRend.material.SetTexture("_MainTex", renderTexture);
    }

    private void InitializeNearPlanePoints()
    {
        _pa = renderPlane.Find("PA");
        _pb = renderPlane.Find("PB");
        _pc = renderPlane.Find("PC");
        _pe = mirrorCam.transform;
    }

    private void InitializeStaticVariables()
    {
        mirrorList.Add(this);
        List<Mirror> mirrorsInView = new List<Mirror>();
        MirrorInfo mirrorInfo = new MirrorInfo(mirrorCam, null, mirrorBounds);

        if (!mirrorInfos.ContainsKey(this))
        {
            mirrorInfos.Add(this, mirrorInfo);
        }

        Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(mirrorCam);
        for (int i = 0; i < mirrorList.Count; i++)
        {
            if (GeometryUtility.TestPlanesAABB(camPlanes, mirrorInfos[mirrorList[i]].rendererBounds))
            {
                //This mirror is visible from this mirrorCam
                mirrorsInView.Add(mirrorList[i]);
            }
            else if (mirrorsInView.Contains(mirrorList[i]))
            {
                mirrorsInView.Remove(mirrorList[i]);
            }
        }

        mirrorInfo.mirrorsInView = mirrorsInView;
        mirrorInfos[this] = mirrorInfo;

        Debug.Log(mirrorInfos);
        Debug.Log(mirrorList);
    }

    private bool VisibleFromCamera(Camera camera, Bounds bounds)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, bounds);
    }

    private void OnDestroy()
    {
        Renderer planeRend = renderPlane.GetComponent<Renderer>();
        planeRend.material.SetTexture("_MainTex", null);
        renderTexture.Release();
    }

    public struct MirrorInfo
    {
        public Camera mirrorCam;
        public List<Mirror> mirrorsInView;
        public Bounds rendererBounds;
        public VisibleInfo visibleInfo;

        public MirrorInfo(Camera mirrorCam, List<Mirror> mirrorsInView, Bounds rendererBounds)
        {
            this.mirrorCam = mirrorCam;
            this.mirrorsInView = mirrorsInView;
            this.rendererBounds = rendererBounds;
            visibleInfo = new VisibleInfo();
        }
    }

    public struct VisibleInfo
    {
        public bool visibleFromMirrorCamera;
        public bool visibleFromMainCamera;
    }
}

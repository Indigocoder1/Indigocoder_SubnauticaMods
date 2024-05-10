using Chameleon.Interfaces;
using Chameleon.Monobehaviors.Abstract;
using UnityEngine;

namespace Chameleon.Monobehaviors
{
    internal class LoadDistanceFieldIntoWaterClipProxy : PrefabModifier, ICyclopsReferencer
    {
        //This is pretty much copied from https://github.com/LeeTwentyThree/SealSub/blob/main/SealSubMod/MonoBehaviours/Prefab/LoadDistanceFieldIntoWaterClipProxy.cs

        public WaterClipProxy waterClipProxy;
        public DistanceField distanceField;

        private void OnValidate()
        {
            //Disable so its initialization isn't called
            waterClipProxy.enabled = false;
        }

        public void OnCyclopsReferenceFinished(GameObject cyclops)
        {
            waterClipProxy.clipMaterial = cyclops.transform.Find("WaterClipProxy").GetComponent<WaterClipProxy>().clipMaterial;
        }

        private void Start()
        {
            waterClipProxy.initialized = true;
            if (waterClipProxy.waterSurface == null) waterClipProxy.waterSurface = WaterSurface.Get();

            ApplyDistanceField();

            waterClipProxy.enabled = true;
            gameObject.layer = 28;
        }

        private void ApplyDistanceField()
        {
            waterClipProxy.UnloadDistanceField();
            var borderSizeScaled = default(Vector3);
            borderSizeScaled.x = waterClipProxy.waterSurface.foamDistance / transform.lossyScale.x;
            borderSizeScaled.y = waterClipProxy.waterSurface.foamDistance / transform.lossyScale.y;
            borderSizeScaled.z = waterClipProxy.waterSurface.foamDistance / transform.lossyScale.z;
            if(distanceField != null)
            {
                //Creates a bounds which the 3d texture will be in
                //This will be sent to the water shader to clip it
                waterClipProxy.distanceFieldMin = distanceField.min;
                waterClipProxy.distanceFieldMax = distanceField.max;
                waterClipProxy.distanceFieldSize = waterClipProxy.distanceFieldMax - waterClipProxy.distanceFieldMin;
                waterClipProxy.distanceFieldTexture = distanceField.texture;
                var extents = waterClipProxy.distanceFieldSize * .5f + borderSizeScaled;
                var center = (waterClipProxy.distanceFieldMin + waterClipProxy.distanceFieldMax) * .5f;

                waterClipProxy.CreateBoxMesh(center, extents);
            }
            else
            {
                Main_Plugin.logger.LogWarning($"No distance field found on {this}! Using a box instead");
                var extents = Vector3.one * .5f + borderSizeScaled;
                var center = Vector3.zero;

                waterClipProxy.CreateBoxMesh(center, extents);
            }

            var renderer = gameObject.EnsureComponent<Renderer>();
            //Gets the clip material from the cyclops reference earlier, assigns it the the renderer, then gets a reference to the version
            //on this gameobject's renderer
            renderer.material = waterClipProxy.clipMaterial;
            waterClipProxy.clipMaterial = renderer.material;
            waterClipProxy.UpdateMaterial();
        }
    }
}

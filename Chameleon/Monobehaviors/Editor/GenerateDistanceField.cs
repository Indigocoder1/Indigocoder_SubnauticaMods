﻿using UnityEngine;

namespace Chameleon.Monobehaviors.Editor
{
    internal class GenerateDistanceField : MonoBehaviour
    {
        public Bounds bounds;
        public DistanceField distanceField;
        public float pixelsPerUnit;

        //Too lazy to make a custom editor for this
        [Header("Bake")] public bool generateTexture;

        [Header("Editor visualization")] public bool visualizeInEditor;
        [Tooltip("Percentage value representing the depth through the volume at which the cross section is visualized.")]
        [Range(0f, 1f)] public float crossSectionVisualizationDepth;
        public Axis crossSectionAxis;
        [Tooltip("If false (default value), the interior pixels will be rendered. If true, exterior pixels are rendered.")]
        public bool inverseVisualization;

        private Vector3 Resolution => bounds.size * pixelsPerUnit;
        private Texture3D Texture3D => distanceField.texture;

        private void OnValidate()
        {
            if(generateTexture)
            {
                generateTexture = false;
                GenerateTexture();
                visualizeInEditor = false;
            }
        }

        public void GenerateTexture()
        {
            if(distanceField == null)
            {
                Debug.LogError($"No distance field found on object {this}! Cannot generate a texture");
                return;
            }

            if (distanceField.texture == null || distanceField.texture.width != (int)Resolution.x ||
                distanceField.texture.height != (int)Resolution.y || distanceField.texture.depth != (int)Resolution.z)
            {
                Debug.Log("Creating texture!");
                distanceField.texture = new((int)Resolution.x, (int)Resolution.y, (int)Resolution.z, TextureFormat.Alpha8, 1)
                {
                    //Idk why this is needed but Lee said so in the Seal commits ¯\_(ツ)_/¯
                    wrapMode = TextureWrapMode.Clamp
                };
            }

            var checkBoxExtents = Vector3.Scale(bounds.extents, VectorReciprocal(Resolution));
            for (int z = 0; z < Resolution.z; z++)
            {
                for (int y = 0; y < Resolution.y; y++)
                {
                    int interiorStartPixel = -1;
                    int interiorEndPixel = -1;
                    bool lastPixelOccupied = false;
                    for (int x = 0; x < Resolution.x; x++)
                    {
                        var occupied = Physics.CheckBox(PixelToWorldCoordinate(new Vector3(x, y, z)), checkBoxExtents);
                        if(occupied && interiorStartPixel == -1)
                        {
                            interiorStartPixel = x;
                        }
                        else if(lastPixelOccupied && !occupied)
                        {
                            interiorEndPixel = x;
                        }

                        lastPixelOccupied = occupied;
                    }

                    if(interiorStartPixel == -1)
                    {
                        //Didn't hit anything
                        for (int x = 0; x < Resolution.x; x++)
                        {
                            Texture3D.SetPixel(x, y, z, Color.white);
                        }
                    }
                    else
                    {
                        for (int x = 0; x < Resolution.x; x++)
                        {
                            Texture3D.SetPixel(x, y, z, x >= interiorStartPixel && x < interiorEndPixel ? Color.clear : Color.white);
                        }
                    }
                }
            }

            Texture3D.Apply();
        }

        private void OnDrawGizmos()
        {
            if(!visualizeInEditor)
            {
                return;
            }

            Gizmos.color = new Color(0.3f, 0.3f, 0f, 0.2f);

            Gizmos.DrawCube(bounds.center, bounds.size);

            if (distanceField == null)
            {
                Debug.LogError("Cannot visualize an object with no distance field!");
                return;
            }

            if (Texture3D == null)
            {
                Debug.LogError("Cannot visualize a distance field with no a texture!");
                return;
            }

            Gizmos.color = new Color(0.3f, 0.4f, 1f, 0.2f);

            int d = (int)(Resolution[(int)crossSectionAxis] * crossSectionVisualizationDepth);
            for (int l = 0; l < Resolution[((int)crossSectionAxis + 1) % 3]; l++)
            {
                for (int w = 0; w < Resolution[((int)crossSectionAxis + 2) % 3]; w++)
                {
                    var location = GetCrossSectionLocation(l, w, d, crossSectionAxis);
                    var transparent = Mathf.Approximately(Texture3D.GetPixel(location.x, location.y, location.z).a, inverseVisualization ? 1f : 0f);
                    if(transparent)
                    {
                        Gizmos.DrawCube(PixelToWorldCoordinate(location), Vector3.Scale(bounds.size, VectorReciprocal(Resolution)));
                    }
                }
            }
        }

        private static Vector3Int GetCrossSectionLocation(int length, int width, int depth, Axis axis)
        {
            return axis switch
            {
                Axis.Right => new(depth, length, width),
                Axis.Up => new(width, depth, length),
                Axis.Forward => new(length, width, depth),
                _ => new()
            };
        }

        private Vector3 PixelToWorldCoordinate(Vector3 pixel)
        {
            return bounds.center - bounds.extents + Vector3.Scale(pixel, Vector3.one * (1 / pixelsPerUnit));
        }

        private Vector3 VectorReciprocal(Vector3 v) => new(1 / v.x, 1 / v.y, 1 / v.z);

        public enum Axis
        {
            Right,
            Up,
            Forward
        }
    }
}

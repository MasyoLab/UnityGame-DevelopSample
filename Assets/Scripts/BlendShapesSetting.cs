using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasyoLab.Game.DevelopSample
{
    public class BlendShapesSetting : MonoBehaviour
    {
        public struct BlendShapesData
        {
            public SkinnedMeshRenderer SkinnedMeshRenderer { get; private set; }
            public string BlendShapeName { get; private set; }
            public int ShapeIndex { get; private set; }
            public float BlendShapeWeight { get; private set; }

            public BlendShapesData(SkinnedMeshRenderer skinnedMeshRenderer, string blendShapeName, int shapeIndex, float blendShapeWeight)
            {
                this.SkinnedMeshRenderer = skinnedMeshRenderer;
                this.BlendShapeName = blendShapeName;
                this.ShapeIndex = shapeIndex;
                this.BlendShapeWeight = blendShapeWeight;
            }
        }

        private List<BlendShapesData> _blendShapesDataList = new List<BlendShapesData>();

        private void Awake()
        {
            var skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                for (int shapeIndex = 0; shapeIndex < skinnedMeshRenderer.sharedMesh.blendShapeCount; shapeIndex++)
                {
                    var blendShapeWeight = skinnedMeshRenderer.GetBlendShapeWeight(shapeIndex);
                    var blendShapeName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(shapeIndex);
                    var blendShapesData = new BlendShapesData(skinnedMeshRenderer, blendShapeName, shapeIndex, blendShapeWeight);
                    _blendShapesDataList.Add(blendShapesData);
                }
            }
        }
    }
}

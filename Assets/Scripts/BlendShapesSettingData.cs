using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MasyoLab.Game.DevelopSample
{
    public class BlendShapesSettingData : ScriptableObject
    {
        [System.Serializable]
        public class BlendShapesData
        {
            public SkinnedMeshRenderer SkinnedMeshRenderer;
            public string BlendShapeName;
            public int ShapeIndex;
            public float BlendShapeWeight;

            public BlendShapesData(SkinnedMeshRenderer skinnedMeshRenderer, string blendShapeName, int shapeIndex, float blendShapeWeight)
            {
                this.SkinnedMeshRenderer = skinnedMeshRenderer;
                this.BlendShapeName = blendShapeName;
                this.ShapeIndex = shapeIndex;
                this.BlendShapeWeight = blendShapeWeight;
            }
        }

        public List<BlendShapesData> BlendShapesDataList = new List<BlendShapesData>();
    }
}

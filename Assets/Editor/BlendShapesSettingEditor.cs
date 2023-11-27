using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static MasyoLab.Game.DevelopSample.BlendShapesSettingData;

namespace MasyoLab.Game.DevelopSample
{
    [CustomEditor(typeof(BlendShapesSetting))]
    public class BlendShapesSettingEditor : UnityEditor.Editor
    {
        private BlendShapesSetting _blendShapesSetting => (BlendShapesSetting)target;
        private Dictionary<string, BlendShapesData> _blendShapesSettingDataDict = new Dictionary<string, BlendShapesData>();
        private float _breastsBigValue = 0;
        private float _breastsSmallValue = 0;

        private void OnEnable()
        {
            if (!_blendShapesSettingDataDict.Any())
            {
                var skinnedMeshRenderers = _blendShapesSetting.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
                {
                    for (int shapeIndex = 0; shapeIndex < skinnedMeshRenderer.sharedMesh.blendShapeCount; shapeIndex++)
                    {
                        var blendShapeWeight = skinnedMeshRenderer.GetBlendShapeWeight(shapeIndex);
                        var blendShapeName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(shapeIndex);
                        var blendShapesData = new BlendShapesData(skinnedMeshRenderer, blendShapeName, shapeIndex, blendShapeWeight);
                        if (!_blendShapesSettingDataDict.ContainsKey(blendShapeName))
                        {
                            _blendShapesSettingDataDict.Add(blendShapeName, blendShapesData);
                        }
                    }
                }
            }

            var blendShapesDatas = GetBlendShapesData("Breasts_big");
            if (blendShapesDatas.Any())
            {
                var blendShapesData = blendShapesDatas[0];
                _breastsBigValue = blendShapesData.SkinnedMeshRenderer.GetBlendShapeWeight(blendShapesData.ShapeIndex);
            }
            blendShapesDatas = GetBlendShapesData("Breasts_small");
            if (blendShapesDatas.Any())
            {
                var blendShapesData = blendShapesDatas[0];
                _breastsSmallValue = blendShapesData.SkinnedMeshRenderer.GetBlendShapeWeight(blendShapesData.ShapeIndex);
            }
        }

        private IReadOnlyList<BlendShapesData> GetBlendShapesData(params string[] blendShapeNames)
        {
            var blendShapesDatas = new List<BlendShapesData>();
            if (!_blendShapesSettingDataDict.Any())
            {
                return blendShapesDatas;
            }
            foreach (var item in blendShapeNames)
            {
                if (!_blendShapesSettingDataDict.ContainsKey(item))
                {
                    continue;
                }
                var blendShapesData = _blendShapesSettingDataDict[item];
                blendShapesDatas.Add(blendShapesData);
            }
            return blendShapesDatas;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // ターゲットのオブジェクトを更新
            serializedObject.Update();

            OnSetDefault();
            BreastsBig();
            BreastsSmall();
            CommonSlider();

            // ターゲットのオブジェクトに変更を適用
            serializedObject.ApplyModifiedProperties();

            // 変更があったら即座にエディタに反映
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void OnSetDefault()
        {
            if (!GUILayout.Button("SetDefault"))
            {
                return;
            }

            _blendShapesSetting.BlendShapesSettingData.BlendShapesDataList.Clear();
            var skinnedMeshRenderers = _blendShapesSetting.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
            {
                for (int shapeIndex = 0; shapeIndex < skinnedMeshRenderer.sharedMesh.blendShapeCount; shapeIndex++)
                {
                    var blendShapeWeight = skinnedMeshRenderer.GetBlendShapeWeight(shapeIndex);
                    var blendShapeName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(shapeIndex);
                    var blendShapesData = new BlendShapesSettingData.BlendShapesData(skinnedMeshRenderer, blendShapeName, shapeIndex, blendShapeWeight);
                    _blendShapesSetting.BlendShapesSettingData.BlendShapesDataList.Add(blendShapesData);
                }
            }
            // データの変更をUnityに通知し、変更を保存
            EditorUtility.SetDirty(_blendShapesSetting.BlendShapesSettingData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
        }

        private void BreastsBig()
        {
            EditorGUI.BeginChangeCheck();
            _breastsBigValue = EditorGUILayout.Slider("バスト(大)", _breastsBigValue, 0, 100f);
            if (EditorGUI.EndChangeCheck())
            {
                var blendShapesDatas = GetBlendShapesData("Breasts_big", "Blouse_breasts_big");
                foreach (var blendShapesData in blendShapesDatas)
                {
                    blendShapesData.BlendShapeWeight = _breastsBigValue;
                    blendShapesData.SkinnedMeshRenderer.SetBlendShapeWeight(blendShapesData.ShapeIndex, _breastsBigValue);
                }
            }
        }

        private void BreastsSmall()
        {
            EditorGUI.BeginChangeCheck();
            _breastsSmallValue = EditorGUILayout.Slider("バスト(小)", _breastsSmallValue, 0, 100f);
            if (EditorGUI.EndChangeCheck())
            {
                var blendShapesDatas = GetBlendShapesData("Breasts_small", "Blouse_breasts_small");
                foreach (var blendShapesData in blendShapesDatas)
                {
                    blendShapesData.BlendShapeWeight = _breastsSmallValue;
                    blendShapesData.SkinnedMeshRenderer.SetBlendShapeWeight(blendShapesData.ShapeIndex, _breastsSmallValue);
                }
            }
        }

        private void CommonSlider()
        {
            foreach (var item in _blendShapesSettingDataDict)
            {
                EditorGUI.BeginChangeCheck();
                item.Value.BlendShapeWeight = EditorGUILayout.Slider(item.Value.BlendShapeName, item.Value.BlendShapeWeight, 0, 100f);
                if (EditorGUI.EndChangeCheck())
                {
                    item.Value.SkinnedMeshRenderer.SetBlendShapeWeight(item.Value.ShapeIndex, item.Value.BlendShapeWeight);
                }
            }
        }
    }
}

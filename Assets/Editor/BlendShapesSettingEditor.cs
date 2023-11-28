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
        private List<Dictionary<string, BlendShapesData>> _blendShapesSettingDataList = new List<Dictionary<string, BlendShapesData>>();
        private Dictionary<string, List<BlendShapesData>> _customBlendShapesSettingDict = new Dictionary<string, List<BlendShapesData>>();

        private void OnEnable()
        {
            if (!_blendShapesSettingDataList.Any())
            {
                var skinnedMeshRenderers = _blendShapesSetting.GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
                {
                    var blendShapesSettingDataDict = new Dictionary<string, BlendShapesData>(skinnedMeshRenderer.sharedMesh.blendShapeCount);
                    _blendShapesSettingDataList.Add(blendShapesSettingDataDict);
                    for (int shapeIndex = 0; shapeIndex < skinnedMeshRenderer.sharedMesh.blendShapeCount; shapeIndex++)
                    {
                        var blendShapeWeight = skinnedMeshRenderer.GetBlendShapeWeight(shapeIndex);
                        var blendShapeName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(shapeIndex);
                        var blendShapesData = new BlendShapesData(skinnedMeshRenderer, blendShapeName, shapeIndex, blendShapeWeight);
                        blendShapesSettingDataDict.Add(blendShapeName, blendShapesData);
                    }
                }

                _customBlendShapesSettingDict.Add("バスト(大)", GetBlendShapesData("Breasts_big", "Blouse_breasts_big"));
                _customBlendShapesSettingDict.Add("バスト(小)", GetBlendShapesData("Breasts_small", "Blouse_breasts_small"));
            }
        }

        private List<BlendShapesData> GetBlendShapesData(params string[] blendShapeNames)
        {
            var blendShapesDatas = new List<BlendShapesData>();
            if (!_blendShapesSettingDataList.Any())
            {
                return blendShapesDatas;
            }
            foreach (var blendShapeName in blendShapeNames)
            {
                foreach (var item in _blendShapesSettingDataList)
                {
                    if (!item.ContainsKey(blendShapeName))
                    {
                        continue;
                    }
                    var blendShapesData = item[blendShapeName];
                    blendShapesDatas.Add(blendShapesData);
                }
            }
            return blendShapesDatas;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // ターゲットのオブジェクトを更新
            serializedObject.Update();

            OnSetDefault();
            CustomSlider();
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

        private void CustomSlider()
        {
            foreach (var keyValuePair in _customBlendShapesSettingDict)
            {
                if (!keyValuePair.Value.Any())
                {
                    continue;
                }
                var baseValue = keyValuePair.Value[0];
                var blendShapeWeight = baseValue.BlendShapeWeight;
                EditorGUI.BeginChangeCheck();
                blendShapeWeight = EditorGUILayout.Slider(keyValuePair.Key, blendShapeWeight, 0, 100f);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (var item in keyValuePair.Value)
                    {
                        item.BlendShapeWeight = blendShapeWeight;
                        item.SkinnedMeshRenderer.SetBlendShapeWeight(item.ShapeIndex, blendShapeWeight);
                    }
                }
            }
        }

        private void CommonSlider()
        {
            foreach (var keyValuePairs in _blendShapesSettingDataList)
            {
                foreach (var keyValuePair in keyValuePairs)
                {
                    EditorGUI.BeginChangeCheck();
                    keyValuePair.Value.BlendShapeWeight = EditorGUILayout.Slider(keyValuePair.Value.BlendShapeName, keyValuePair.Value.BlendShapeWeight, 0, 100f);
                    if (EditorGUI.EndChangeCheck())
                    {
                        keyValuePair.Value.SkinnedMeshRenderer.SetBlendShapeWeight(keyValuePair.Value.ShapeIndex, keyValuePair.Value.BlendShapeWeight);
                    }
                }
            }
        }
    }
}

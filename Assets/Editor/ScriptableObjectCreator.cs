using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MasyoLab.Game.DevelopSample
{
    public class ScriptableObjectCreator : MonoBehaviour
    {
        [MenuItem("Assets/Create/Scriptable Object", true)]
        static bool ValidateCreateCustomMenu()
        {
            // 右クリックしたアセットがプレハブであるかを確認
            if (Selection.activeObject != null && PrefabUtility.GetPrefabAssetType(Selection.activeObject) == PrefabAssetType.Regular)
            {
                // プレハブのルートオブジェクトを取得
                GameObject prefabRoot = PrefabUtility.LoadPrefabContents(AssetDatabase.GetAssetPath(Selection.activeObject));
                // 特定のコンポーネントがアタッチされているかを確認
                if (prefabRoot.GetComponent<BlendShapesSetting>() != null)
                {
                    return true; // メニューを表示
                }
            }
            return false; // メニューを表示しない
        }

        [MenuItem("Assets/Create/Scriptable Object")]
        public static void CreateScriptableObject()
        {
            // 選択されたアセットを取得
            Object selectedObject = Selection.activeObject;

            // 選択されたものがプレハブであればスクリプタブルオブジェクトを生成または更新
            if (selectedObject != null && PrefabUtility.GetPrefabAssetType(selectedObject) == PrefabAssetType.Regular)
            {
                // スクリプタブルオブジェクトの型に合わせて適切なクラスを指定
                var scriptableObject = ScriptableObject.CreateInstance<BlendShapesSettingData>();

                // プレハブのあるフォルダ内にアセットを保存
                string prefabPath = AssetDatabase.GetAssetPath(selectedObject.GetInstanceID());
                var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                string folderPath = System.IO.Path.GetDirectoryName(prefabPath);
                string assetPath = $"{folderPath}/{prefabAsset.name}Setting.asset";

                // 既存のアセットがあれば更新、なければ新規作成
                var existingScriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);
                if (existingScriptableObject != null)
                {
                    // 既存のアセットが存在する場合は内容を更新
                    EditorUtility.CopySerialized(scriptableObject, existingScriptableObject);
                }
                else
                {
                    // 新しいアセットを作成
                    AssetDatabase.CreateAsset(scriptableObject, assetPath);
                }

                // データの変更をUnityに通知し、変更を保存
                EditorUtility.SetDirty(scriptableObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = scriptableObject;

                // PrefabにScriptableObjectをアタッチ
                if (prefabAsset != null)
                {
                    // プレハブのインスタンスを取得
                    var prefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabAsset);
                    var scriptableObjectComponent = prefabInstance.GetComponent<BlendShapesSetting>();
                    scriptableObjectComponent.BlendShapesSettingData = AssetDatabase.LoadAssetAtPath<BlendShapesSettingData>(assetPath);

                    // 更新が完了したらプレハブを保存
                    PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);

                    // プレハブのインスタンスを破棄
                    DestroyImmediate(prefabInstance);
                }
            }
        }
    }
}

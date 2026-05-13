using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetViewer : EditorWindow
{
    private Object selectedAsset;
    private string selectedPath = "";
    private Vector2 scrollPosition;

    [MenuItem("Tools/Asset Viewer")]
    public static void ShowWindow()
    {
        GetWindow<AssetViewer>("Asset Viewer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Выберите .assets файл", EditorStyles.boldLabel);

        selectedAsset = EditorGUILayout.ObjectField("Файл", selectedAsset, typeof(Object), false);

        if (selectedAsset != null)
        {
            selectedPath = AssetDatabase.GetAssetPath(selectedAsset);
            GUILayout.Label($"Путь: {selectedPath}");

            if (GUILayout.Button("Показать содержимое"))
            {
                LoadAndDisplayAssets();
            }
        }

        if (!string.IsNullOrEmpty(selectedPath) && File.Exists(selectedPath))
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            Object[] assets = AssetDatabase.LoadAllAssetsAtPath(selectedPath);
            GUILayout.Label($"Найдено объектов: {assets.Length}");

            foreach (Object asset in assets)
            {
                if (asset == null) continue;

                EditorGUILayout.BeginHorizontal();

                if (asset is Texture2D texture)
                {
                    GUILayout.Box(texture, GUILayout.Width(50), GUILayout.Height(50));
                    GUILayout.Label($"Тип: Texture2D\nИмя: {asset.name}\nРазмер: {texture.width}x{texture.height}");

                    if (GUILayout.Button("Сохранить", GUILayout.Width(80)))
                    {
                        SaveTexture(texture, asset.name);
                    }
                }
                else if (asset is Sprite sprite)
                {
                    GUILayout.Label($"Тип: Sprite\nИмя: {asset.name}");

                    if (GUILayout.Button("Сохранить текстуру", GUILayout.Width(100)))
                    {
                        SaveTexture(sprite.texture, sprite.name);
                    }
                }
                else if (asset is GameObject go)
                {
                    GUILayout.Label($"Тип: GameObject\nИмя: {asset.name}");
                }
                else if (asset is Material mat)
                {
                    GUILayout.Label($"Тип: Material\nИмя: {asset.name}");
                }
                else
                {
                    GUILayout.Label($"Тип: {asset.GetType().Name}\nИмя: {asset.name}");
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            EditorGUILayout.EndScrollView();
        }
    }

    private void LoadAndDisplayAssets()
    {
        Debug.Log($"Загрузка: {selectedPath}");
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(selectedPath);
        Debug.Log($"Всего объектов: {assets.Length}");

        foreach (Object asset in assets)
        {
            if (asset is Texture2D tex)
                Debug.Log($"Текстура: {tex.name} ({tex.width}x{tex.height})");
            else if (asset != null)
                Debug.Log($"{asset.GetType().Name}: {asset.name}");
        }

        Repaint();
    }

    private void SaveTexture(Texture2D texture, string name)
    {
        string path = EditorUtility.SaveFilePanel("Сохранить текстуру", "", name, "png");
        if (!string.IsNullOrEmpty(path))
        {
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            Debug.Log($"Сохранено: {path}");
            EditorUtility.RevealInFinder(path);
        }
    }
}
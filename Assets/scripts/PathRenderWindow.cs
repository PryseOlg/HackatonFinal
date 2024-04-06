using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PathRenderWindow : EditorWindow
{
    [SerializeField] private Sprite image;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform pointHolder;
    [SerializeField] private List<Vector2> points = new();

    [MenuItem("Window/PathRender")]
    public static void ShowWindow()
    {
        GetWindow<PathRenderWindow>("PathRender");
    }

    private Vector2 scrollPos;
    void OnGUI()
    {
        var obj = new SerializedObject(this);

        if (GUILayout.Button("Recognize path", new GUIStyle(EditorStyles.miniButton) { 
            font = EditorStyles.boldFont, 
            fontSize = 17, 
            fixedHeight = 30, 
            margin = new(5,5,5,5) 
        }))
            if (image != null)
                MakePoints();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        EditorGUI.indentLevel++;
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(obj.FindProperty("image"), true);
        if (image == null)
            EditorGUILayout.HelpBox("Image required", MessageType.Warning);
        EditorGUILayout.PropertyField(obj.FindProperty("spriteRenderer"), true);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(obj.FindProperty("pointPrefab"), true);
        EditorGUILayout.PropertyField(obj.FindProperty("pointHolder"), true);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(obj.FindProperty("lineRenderer"), true);
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(obj.FindProperty("points"), true);

        EditorGUILayout.EndScrollView();

        obj.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
            OnPointsChanged();

        if (spriteRenderer != null && spriteRenderer.sprite != image)
            spriteRenderer.sprite = image;
    }

    private void Update()
    {
        CheckPointsTransformChange();
    }

    private void CheckPointsTransformChange(bool fromGui = false)
    {
        var hasChanged = false;
        if (pointHolder != null)
            for (int i = 0; i < System.Math.Min(points.Count, pointHolder.childCount); i++)
                if (pointHolder.GetChild(i).transform.localPosition != new Vector3(points[i].x, points[i].y))
                {
                    points[i] = pointHolder.GetChild(i).transform.localPosition;
                    hasChanged = true;
                }

        if (hasChanged)
        {
            OnPointsChanged();
            Repaint();
        }
    }

    private void OnPointsChanged()
    {
        if (pointHolder != null && pointPrefab != null)
        {
            for (int i = pointHolder.childCount; i < points.Count; i++)
                Instantiate(pointPrefab, pointHolder);

            for (int i = pointHolder.childCount; i > points.Count; i--)
                DestroyImmediate(pointHolder.GetChild(i - 1).gameObject);

            for (int i = 0; i < points.Count; i++)
                pointHolder.GetChild(i).transform.localPosition = points[i];
        }

        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = false;
            lineRenderer.positionCount = points.Count;

            lineRenderer.SetPositions(points.Select(p => (Vector3)p).ToArray());
        }
    }

    private void MakePoints()
    {
        var scale = new Vector2(image.bounds.size.x / image.rect.width, image.bounds.size.y / image.rect.height);
        points = Enumerable.Range(0, 4).Select(i => new Vector2(Random.Range(0, image.rect.width), Random.Range(0, image.rect.height)))//ImageUploader.UploadImage(image.texture)
            .Select(p => new Vector2(p.x * scale.x, p.y * scale.y) - 
                new Vector2(image.bounds.size.x/ 2, image.bounds.size.y / 2))
            .ToList();
        OnPointsChanged();
    }
}

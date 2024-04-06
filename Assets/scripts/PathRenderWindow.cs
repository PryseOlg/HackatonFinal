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

        if (GUILayout.Button("MakePoints"))
            if (image != null)
                MakePoints();

        EditorGUI.indentLevel++;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true);
        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(obj.FindProperty("image"), true);
        EditorGUILayout.PropertyField(obj.FindProperty("pointPrefab"), true);
        EditorGUILayout.PropertyField(obj.FindProperty("spriteRenderer"), true);
        EditorGUILayout.PropertyField(obj.FindProperty("lineRenderer"), true);
        EditorGUILayout.PropertyField(obj.FindProperty("pointHolder"), true);

        var pointCount = points.Count;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(obj.FindProperty("points"), true);

        EditorGUILayout.EndScrollView();

        obj.ApplyModifiedProperties();

        if (pointCount != points.Count || EditorGUI.EndChangeCheck())
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
        /*for (int i = 0; i < 3; i++)
            points.Add(new Vector2(
                Random.Range(image.bounds.min.x, image.bounds.max.x), 
                Random.Range(image.bounds.min.y, image.bounds.max.y))
            );*/

        
        points = ImageUploader.UploadImage(image.texture);
        OnPointsChanged();
    }
}

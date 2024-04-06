using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Assets.Lines
{
    [ExecuteAlways]
    public class TestLineDrawer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Line linePrefab;
        [SerializeField] private Transform lineParent;
        [SerializeField] private Sprite sprite;

        private void Awake()
        {
            lineParent ??= transform;
        }

        private void OnValidate()
        {
            spriteRenderer ??= GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.sprite = sprite;
        }

        public void DrawLine()
        {
            spriteRenderer.sprite = sprite;

            var currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity, lineParent);
            currentLine.name = "Line";

            currentLine.AddPoint(new Vector3(0, 0));
            currentLine.AddPoint(new Vector3(0, sprite.bounds.size.y / 2));
            currentLine.AddPoint(new Vector3(sprite.bounds.size.x / 2, sprite.bounds.size.y / 2));
            currentLine.AddPoint(new Vector3(sprite.bounds.size.x / 2, 0));
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(TestLineDrawer))]
    public class TestLineDrawerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Click me"))
            {
                (target as TestLineDrawer).DrawLine();
            }
        }
    }

#endif
}
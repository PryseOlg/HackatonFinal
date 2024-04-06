using UnityEngine;
using UnityEditor;

public class GUIPlugin : EditorWindow
{
    string myString = "Hello";
    
    [MenuItem("Window/GUI")]
    public static void ShowWindow ()
    {
        GetWindow<GUIPlugin>("GUI");
    }

    void OnGUI ()
        {
            GUILayout.Label("This is a label.", EditorStyles.boldLabel);

            myString = EditorGUILayout.TextField("Name", myString);

            if (GUILayout.Button("Press me"))
            {
                Debug.Log ("Button was pressed");
            }
        }
    
}

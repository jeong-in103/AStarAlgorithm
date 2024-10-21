using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathFinding))]
public class PathFindButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PathFinding generator = (PathFinding)target;
        if (GUILayout.Button("Start Path Finding"))
        {
            generator.StartFindPath();
        }
    }
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CustomGrid))]
public class GridGenerateButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CustomGrid generator = (CustomGrid)target; 
        if(GUILayout.Button("Generate Cubes"))
        {
            generator.SetUp();
        }
    }
}

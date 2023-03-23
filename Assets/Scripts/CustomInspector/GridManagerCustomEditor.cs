using Grid;
using UnityEditor;
using UnityEngine;

namespace CustomInspector
{
    [CustomEditor(typeof(GridManager))]
    public class GridManagerCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GridManager gridManager = (GridManager)target;
            if (GUILayout.Button("Enter Play Mode"))
            {
                EditorApplication.EnterPlaymode();
            }
            if(GUILayout.Button("Generate Grid Button"))
            {
                gridManager.GenerateGrid();
            }
            if(GUILayout.Button("Exit Play Mode"))
            {
                EditorApplication.ExitPlaymode();
            }
        }
    }
}

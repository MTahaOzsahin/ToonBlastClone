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
            var gridManager = (GridManager)target;
            if(GUILayout.Button("Generate Grid Button"))
            {
                if (EditorApplication.isPlaying)
                {
                    gridManager.GenerateGrid();
                }
            }
        }
    }
}

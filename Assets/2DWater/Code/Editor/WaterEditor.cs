using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR


namespace Bundos.WaterSystem
{
    [CustomEditor(typeof(Water))]
    public class WaterEditor : Editor
    {
        Water water;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            water = target as Water;

            water.Initialize();
            water.CreateShape();
            water.UpdateMesh();
        }
    }
}
#endif
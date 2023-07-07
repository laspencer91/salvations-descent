using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

[CustomEditor(typeof(ShotgunRaycastWeapon))]
public class ShotgunBulletSpreadEditor : OdinEditor
{
    private SerializedProperty points;
    private float graphWidth = 150f;
    private float graphHeight = 150f;
    private float pointRadius = 3;

    protected override void OnEnable()
    {
        base.OnEnable();
        points = serializedObject.FindProperty("BulletAngles");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        DrawGraph();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawGraph()
    {
        SirenixEditorGUI.BeginBox("Bullet Spread Pattern", true);


        ShotgunRaycastWeapon graph = target as ShotgunRaycastWeapon;

        Rect graphRect = GUILayoutUtility.GetRect(graphWidth, graphHeight);
        graphRect.width = graphWidth;
        graphRect.height = graphHeight;

        EditorGUI.DrawRect(graphRect, new Color(80f / 255f, 105f / 255f, 30f / 255f));
        Handles.color = Color.black;
        
        float maxPointValue = 0f;

        // Find the maximum value in the points array
        for (int i = 0; i < graph.BulletAngles.Length; i++)
        {
            float absX = Mathf.Abs(graph.BulletAngles[i].x);
            float absY = Mathf.Abs(graph.BulletAngles[i].y);

            maxPointValue = Mathf.Max(maxPointValue, absX, absY);
        }

        float paddedPointValue = maxPointValue += maxPointValue / 4;


        // Draw X-Axis
        Handles.DrawLine(new Vector2(graphRect.x, graphRect.center.y), new Vector2(graphRect.x + graphRect.width, graphRect.center.y));
        Handles.DrawLine(new Vector2(graphRect.center.x, graphRect.y), new Vector2(graphRect.center.x, graphRect.y + graphRect.height));

        Handles.DrawLine(new Vector2(graphRect.x, graphRect.y), new Vector2(graphRect.xMax, graphRect.y));
        Handles.DrawLine(new Vector2(graphRect.xMax, graphRect.y), new Vector2(graphRect.xMax, graphRect.yMax));
        Handles.DrawLine(new Vector2(graphRect.x, graphRect.yMax), new Vector2(graphRect.x, graphRect.y));
        Handles.DrawLine(new Vector2(graphRect.xMax, graphRect.yMax), new Vector2(graphRect.x, graphRect.yMax));

        for (int i = 0; i < graph.BulletAngles.Length; i++)
        {
            Vector2 pointPosition = new Vector2(
                graphRect.center.x + (graph.BulletAngles[i].x / (2f * maxPointValue)) * graphRect.width,
                graphRect.center.y - (graph.BulletAngles[i].y / (2f * maxPointValue)) * graphRect.height
            );

            Handles.DrawSolidDisc(pointPosition, Vector3.forward, pointRadius);
        }

        SirenixEditorGUI.EndBox();
    }
}
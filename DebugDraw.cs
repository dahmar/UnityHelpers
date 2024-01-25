using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDraw : MonoBehaviour
{
    private static Material mat;
    private static List<LineShape> lines = new List<LineShape>();

    private void OnPostRender()
    {
        if(lines.Count > 0)
        {
            DrawLine();
        }
    }

    public static void Line(Vector3 aStart, Vector3 aEnd, Color aColor = default, float aDuration = 0f)
    {
        var line = new LineShape
        {
            startPos = aStart,
            endPos = aEnd,
            color = aColor,
            timeout = 0f
        };
        if (aDuration > 0f)
            line.timeout = Time.time + aDuration;
        lines.Add(line);
    }

    public static void DrawLine()
    {
        if (!mat)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things. In this case, we just want to use
            // a blend mode that inverts destination colors.
            var shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            // Set blend mode to invert destination colors.
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            // Turn off backface culling, depth writes, depth test.
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt("_ZWrite", 0);
            mat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }

        GL.PushMatrix();
        mat.SetPass(0);
        GL.LoadPixelMatrix();

        GL.Begin(GL.LINES);
        float currentTime = Time.time;
        for (int i = lines.Count - 1; i >= 0; i--)
        {
            var line = lines[i];
            GL.Color(line.color);
            GL.Vertex(line.startPos);
            GL.Vertex(line.endPos);
            if (currentTime > line.timeout)
                lines.RemoveAt(i);
        }
        GL.End();

        GL.PopMatrix();
    }
}

public struct LineShape
{
    public Vector3 startPos;
    public Vector3 endPos;
    public Color color;
    public float timeout;
}
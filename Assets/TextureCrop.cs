using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCrop : MonoBehaviour
{
    public Material mat;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        GameObject screen = GameObject.Find("PerspectiveScreen");

        // Get world space points for the 4 corners of the screen
        Vector3 top_left_world = screen.transform.TransformPoint(new Vector3(0.5f, 0.5f, 0.5f));
        Vector3 top_right_world = screen.transform.TransformPoint(new Vector3(-0.5f, 0.5f, 0.5f));
        Vector3 bottom_left_world = screen.transform.TransformPoint(new Vector3(0.5f, -0.5f, 0.5f));
        Vector3 bottom_right_world = screen.transform.TransformPoint(new Vector3(-0.5f, -0.5f, 0.5f));

        // Get screen space coordinates of the 4 points
        Vector3 top_left_screen = Camera.main.WorldToScreenPoint(top_left_world);
        Vector3 top_right_screen = Camera.main.WorldToScreenPoint(top_right_world);
        Vector3 bottom_left_screen = Camera.main.WorldToScreenPoint(bottom_left_world);
        Vector3 bottom_right_screen = Camera.main.WorldToScreenPoint(bottom_right_world);

        // Normalise the screen coordinates to [0, 1]
        int width = Camera.main.pixelWidth;
        int height = Camera.main.pixelHeight;
        Vector2 top_left_uv = new Vector2(top_left_screen.x/width, top_left_screen.y/height);
        Vector2 top_right_uv = new Vector2(top_right_screen.x/width, top_right_screen.y/height);
        Vector2 bottom_left_uv = new Vector2(bottom_left_screen.x/width, bottom_left_screen.y/height);
        Vector2 bottom_right_uv = new Vector2(bottom_right_screen.x/width, bottom_right_screen.y/height);

        Vector4[] corrected_uvs = { new Vector4(bottom_left_uv.x, bottom_left_uv.y, 0.0f, 0.0f),
                                    new Vector4(top_left_uv.x, top_left_uv.y, 0.0f, 0.0f),
                                    new Vector4(bottom_right_uv.x, bottom_right_uv.y, 0.0f, 0.0f),
                                    new Vector4(top_right_uv.x, top_right_uv.y, 0.0f, 0.0f)};

        Debug.Log("uvs");
        Debug.Log(top_left_uv);
        Debug.Log(top_right_uv);
        Debug.Log(bottom_left_uv);
        Debug.Log(bottom_right_uv);

        mat.SetVectorArray("_corrected_uvs", corrected_uvs);
        Graphics.Blit(src, dest);
    }
}

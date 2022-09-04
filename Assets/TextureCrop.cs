using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureCrop : MonoBehaviour
{
    public Material crop_material;
    public Material flip_material;
    public RenderTexture intermediate_buffer;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
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
        
        Vector4[] corrected_uvs = { new Vector4(top_left_uv.x, top_left_uv.y, 0.0f, 0.0f),
                                    new Vector4(top_right_uv.x, top_right_uv.y, 0.0f, 0.0f),
                                    new Vector4(bottom_left_uv.x, bottom_left_uv.y, 0.0f, 0.0f),
                                    new Vector4(bottom_right_uv.x, bottom_right_uv.y, 0.0f, 0.0f)};

        Debug.Log("uvs");
        Debug.Log(top_left_uv);
        Debug.Log(top_right_uv);
        Debug.Log(bottom_right_uv);
        Debug.Log(bottom_left_uv);

        // top left -> top right -> bottom right -> bottom left
        // thank you https://math.stackexchange.com/questions/3037040/normalized-coordinate-of-point-on-4-sided-concave-polygon and in shader
        float x0 = top_left_uv.x;
        float y0 = top_left_uv.y;
        float x1 = top_right_uv.x;
        float y1 = top_right_uv.y;
        float x2 = bottom_right_uv.x;
        float y2 = bottom_right_uv.y;
        float x3 = bottom_left_uv.x;
        float y3 = bottom_left_uv.y;

        float dx1 = x1 - x2;
        float dx2 = x3 - x2;
        float dx3 = x0 - x1 + x2 - x3;
        float dy1 = (y1 - y2);
        float dy2 = (y3 - y2);
        float dy3 = (y0 - y1 + y2 - y3);
        
        float a13 = (dx3 * dy2 - dy3 * dx2) / (dx1 * dy2 - dy1 * dx2);
        float a23 = (dx1 * dy3 - dy1 * dx3) / (dx1 * dy2 - dy1 * dx2);
        float a11 = x1 - x0 + a13 * x1;
        float a21 = x3 - x0 + a23 * x3;
        float a31 = x0;
        float a12 = y1 - y0 + a13 * y1;
        float a22 = y3 - y0 + a23 * y3;
        float a32 = y0;

        Matrix4x4 distortion_mat = Matrix4x4.identity;
        distortion_mat.SetRow(0, new Vector4(a11, a12, a13, 0.0f));
        distortion_mat.SetRow(1, new Vector4(a21, a22, a23, 0.0f));
        distortion_mat.SetRow(2, new Vector4(a31, a32, 1.0f, 0.0f));

        crop_material.SetVectorArray("_perspective_screen_corners", corrected_uvs);
        crop_material.SetMatrix("_screen_distortion_matrix", distortion_mat);

        //RenderTexture intermediate_buffer = new RenderTexture();
        Graphics.Blit(src, intermediate_buffer, crop_material);
        Graphics.Blit(intermediate_buffer, dst, flip_material);
    }
}

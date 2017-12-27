using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ImageEffectClearAlpha : MonoBehaviour
{

    //Shader clears alpha channel only
    private static string clearAlphaMatString =
@"Shader ""ClearAlpha"" {
    SubShader {
        ColorMask A
        ZTest Always Cull Off ZWrite Off Fog { Mode Off }
       Pass { Color (0,0,0,0) }
    }
    Fallback off
}";

    static Material m_Material = null;
    protected static Material material
    {
        get
        {
            if (m_Material == null)
            {
                m_Material = new Material(clearAlphaMatString);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
                m_Material.shader.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_Material;
        }
    }


    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }
}
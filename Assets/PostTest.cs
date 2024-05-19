using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostTest : MonoBehaviour
{
    
    public Shader postProcessShader;
    private Material postProcessMaterial;
    void Start()
    {
        if (postProcessShader == null)
        {
            Debug.LogError("No shader assigned!", this);
            enabled = false;
            return;
        }

        postProcessMaterial = new Material(postProcessShader);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (postProcessMaterial != null)
        {
            Graphics.Blit(src, dest, postProcessMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}


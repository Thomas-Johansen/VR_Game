using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostTest : MonoBehaviour
{
    
    public Shader postProcessShader;
    private Material postProcessMaterial;

    [SerializeField] public Vector3 planetPosition = new Vector3(0, -75, -100);
    [SerializeField] public float planetRadius = 250;
    [SerializeField] public float atmosRadius = 400;
    [SerializeField] public int numInScatteringPoints = 1;
    [SerializeField] public int numOpticalDepthPoints = 1;
    [SerializeField] public Vector3 lightDirection = new Vector3(0, -1, 0);
    [SerializeField] public float densityFalloff = 1.0f;
    private int _planetCentreID;
    private int _planetRadiusID;
    private int _atmosphereRadiusID;
    private int _numInScatteringPointsID;
    private int _numOpticalDepthPointsID;
    private int _lightDirectionID;
    private int _densityFalloffID;

    private void Awake()
    {
        _planetCentreID = Shader.PropertyToID("_planetCentre");
        _planetRadiusID = Shader.PropertyToID("_planetRadius");
        _atmosphereRadiusID = Shader.PropertyToID(("_atmosphereRadius"));
        _numInScatteringPointsID = Shader.PropertyToID("_numInScatteringPoints");
        _numOpticalDepthPointsID = Shader.PropertyToID("_numOpticalDepthPoints");
        _lightDirectionID = Shader.PropertyToID("_lightDirection");
        _densityFalloffID = Shader.PropertyToID("_densityFalloff");
    }

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
            postProcessMaterial.SetVector(_planetCentreID, planetPosition);
            postProcessMaterial.SetFloat(_planetRadiusID, planetRadius);
            postProcessMaterial.SetFloat(_atmosphereRadiusID, atmosRadius);
            postProcessMaterial.SetInt(_numInScatteringPointsID, numInScatteringPoints);
            postProcessMaterial.SetInt(_numOpticalDepthPointsID, numOpticalDepthPoints);
            postProcessMaterial.SetVector(_lightDirectionID, lightDirection);
            postProcessMaterial.SetFloat(_densityFalloffID, densityFalloff);
            Graphics.Blit(src, dest, postProcessMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}


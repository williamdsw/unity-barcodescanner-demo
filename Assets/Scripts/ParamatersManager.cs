using System;
using UnityEngine;

public class ParamatersManager : MonoBehaviour
{
    public static ParamatersManager Instance { get; private set; }

    // General
    public string DeviceName { get; set; }
    public Resolution? Resolution { get; set; }
    public int? TargetFrameRate { get; set; }
    public float? RequestedFPS { get; set; }
    public int VSyncCount { get; set; }
    public int QualitySettingsLevel { get; set; }

    // Scanner
    public int DelayFrameMin { get; set; }
    public float DecodeInterval { get; set; }
    public bool ParserTryHarder { get; set; }

    // Webcam
    public int WebcamFilterMode { get; set; }
    public Vector2? WebcamAutoFocusPoint { get; set; }

    private void Awake()
    {
        CheckInstance();
        InitParamaters();
    }

    private void CheckInstance()
    {
        int numberOfInstances = FindObjectsOfType(GetType()).Length;
        if (numberOfInstances > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void InitParamaters()
    {
        DeviceName = null;
        Resolution = null;
        TargetFrameRate = null;
        RequestedFPS = null;
        VSyncCount = 0;
        QualitySettingsLevel = 0;
        DelayFrameMin = 3;
        DecodeInterval = 0.1f;
        ParserTryHarder = false;
        WebcamFilterMode = (int) FilterMode.Trilinear;
        WebcamAutoFocusPoint = null;
    }
}

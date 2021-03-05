using BarcodeScanner;
using System;
using UnityEngine;

public class ScannerSettingsBuilder
{
    public static ScannerSettings Build()
    {
        ScannerSettings settings = new ScannerSettings();

        try
        {
            settings.ParserTryHarder = ParamatersManager.Instance.ParserTryHarder;
            settings.ScannerDecodeInterval = ParamatersManager.Instance.DecodeInterval;
            settings.ScannerDelayFrameMin = ParamatersManager.Instance.DelayFrameMin;
            settings.WebcamDefaultDeviceName = ParamatersManager.Instance.DeviceName;
            settings.WebcamFilterMode = (FilterMode)ParamatersManager.Instance.WebcamFilterMode;

            if (ParamatersManager.Instance.Resolution != null)
            {
                settings.WebcamRequestedHeight = ParamatersManager.Instance.Resolution.Value.height;
                settings.WebcamRequestedWidth = ParamatersManager.Instance.Resolution.Value.width;
            }

            if (ParamatersManager.Instance.RequestedFPS != null)
            {
                settings.WebcamRequestedFPS = ParamatersManager.Instance.RequestedFPS;
            }

            if (ParamatersManager.Instance.WebcamAutoFocusPoint != null)
            {
                settings.WebcamAutoFocusPoint = ParamatersManager.Instance.WebcamAutoFocusPoint;
            }

            // Others params
            QualitySettings.SetQualityLevel(ParamatersManager.Instance.QualitySettingsLevel);
            QualitySettings.vSyncCount = ParamatersManager.Instance.VSyncCount;

            if (ParamatersManager.Instance.TargetFrameRate != null)
            {
                Application.targetFrameRate = ParamatersManager.Instance.TargetFrameRate.Value;
            }
        }
        catch (Exception ex)
        {
            Debug.LogErrorFormat("ScannerSettingsBuilder : Exception = {0}", ex.Message);
        }
        
        Debug.Log("Scanner Settings Applied!");

        return settings;
    }
}

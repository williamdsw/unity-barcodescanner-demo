using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScannerScene : MonoBehaviour
{
    // || Inspector References

    [Header("UI Elements - Main")]
    [SerializeField] private RawImage outputImage;
    [SerializeField] private Button playButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI currentResolutionText;

    [Header("UI Elements - QrCode Panel")]
    [SerializeField] private GameObject qrCodePanel;
    [SerializeField] private TextMeshProUGUI qrCodeText;
    [SerializeField] private Button closeButton;

    // || Cached References

    private IScanner scanner;

    private void Awake()
    {
        SetListeners();

        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }

    private void Start()
    {
        InitScannerSettings();
    }

    private void Update()
    {
        if (scanner != null)
        {
            scanner.Update();
            if (currentResolutionText)
            {
                currentResolutionText.text = string.Format("Current Resolution: {0}x{1}", scanner.Camera.Width, scanner.Camera.Height);
            }
        }
    }

    private void SetListeners()
    {
        if (playButton && stopButton && backButton && closeButton)
        {
            playButton.onClick.AddListener(() => StartScanner());
            stopButton.onClick.AddListener(() => StopScanner());
            backButton.onClick.AddListener(() =>
            {
                StartCoroutine(StopCamera(() => SceneManager.LoadScene("DemoScene")));
            });
            closeButton.onClick.AddListener(() => qrCodePanel.SetActive(false));
        }
    }

    private void InitScannerSettings()
    {
        scanner = new Scanner(ScannerSettingsBuilder.Build());
        scanner.Camera.Play();

        scanner.OnReady += (sender, arg) =>
        {
            outputImage.transform.localEulerAngles = scanner.Camera.GetEulerAngles();
            outputImage.transform.localScale = scanner.Camera.GetScale();
            outputImage.texture = scanner.Camera.Texture;

            RectTransform rectTransform = outputImage.GetComponent<RectTransform>();
            float newHeight = (rectTransform.sizeDelta.x * scanner.Camera.Height / scanner.Camera.Width);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
        };

        scanner.StatusChanged += (sender, arg) =>
        {
            statusText.text = string.Format("Status: {0}", scanner.Status);
        };
    }

    private void StartScanner()
    {
        if (scanner != null)
        {
            scanner.Scan((barCodeType, barCodeValue) =>
            {
                scanner.Stop();

                qrCodePanel.SetActive(true);
                qrCodeText.text = string.Format("Found: {0} / {1}", barCodeType, barCodeValue);

                #if UNITY_ANDROID || UNITY_IOS
                Handheld.Vibrate();
                #endif
            });
        }
    }

    private void StopScanner()
    {
        if (scanner != null)
        {
            scanner.Stop();
        }
    }

    private IEnumerator StopCamera(Action callback)
    {
        outputImage = null;
        scanner.Destroy();
        scanner = null;
        yield return new WaitForSeconds(0.1f);
        callback.Invoke();
    }
}

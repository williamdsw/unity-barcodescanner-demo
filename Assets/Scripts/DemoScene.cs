using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoScene : MonoBehaviour
{
    // || Inspector References

    [SerializeField] private GameObject mainCanvas;

    [Header("UI References - General Paramaters Panel")]
    [SerializeField] private TMP_Dropdown devicesDropdown;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private TMP_InputField targetFrameRateInput;
    [SerializeField] private TMP_InputField requestedFpsInput;
    [SerializeField] private TMP_Dropdown vSyncCountDropdown;
    [SerializeField] private TMP_Dropdown qualitySettingsDropdown;

    [Header("UI References - Scanner Paramaters Panel")]
    [SerializeField] private TMP_InputField delayFrameMinInput;
    [SerializeField] private TMP_InputField decodeIntervalInput;
    [SerializeField] private Toggle parserTryHarderToggle;
    [SerializeField] private Toggle autoFocusSupportedToggle;
    [SerializeField] private TMP_Dropdown webcamFilterModeDropdown;
    [SerializeField] private Toggle applySettingsToggle;

    [Header("UI - Auto Toggle Point Elements")]
    [SerializeField] private GameObject container;
    [SerializeField] private TMP_InputField xInput;
    [SerializeField] private TMP_InputField yInput;

    [Header("UI Others")]
    [SerializeField] private Button quitButton;
    [SerializeField] private Button openScannerButton;

    [Header("UI - Error Panel")]
    [SerializeField] private GameObject errorCanvas;
    [SerializeField] private Button tryAgainButton;

    // || Cached

    private CanvasGroup mainCanvasGroup;
    private CanvasGroup errorCanvasGroup;

    private void Awake()
    {
        if (mainCanvas)
        {
            mainCanvasGroup = mainCanvas.GetComponent<CanvasGroup>();
        }

        if (errorCanvas)
        {
            errorCanvasGroup = errorCanvas.GetComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        SetListeners();
        CheckDevices(false);
    }

    private void CheckDevices(bool cameFromError)
    {
        if (ContainsDevices())
        {
            if (cameFromError)
            {
                openScannerButton.interactable = true;
                ToggleCanvas(errorCanvas, errorCanvasGroup);
                ToggleCanvas(mainCanvas, mainCanvasGroup);
            }

            ClearFields();
            FillFields();
        }
        else
        {
            openScannerButton.interactable = false;
            ToggleCanvas(mainCanvas, mainCanvasGroup);
            ToggleCanvas(errorCanvas, errorCanvasGroup);
        }
    }

    private bool ContainsDevices()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            return false;
        }

        foreach (WebCamDevice device in devices)
        {
            if (device.availableResolutions == null)
            {
                return false;
            }
        }

        return true;
    }

    private void SetListeners()
    {
        devicesDropdown.onValueChanged.AddListener(index => ClearAndFillResolutions(WebCamTexture.devices[index]));
        vSyncCountDropdown.onValueChanged.AddListener(index =>
        {
            bool disabled = (index == 0);
            targetFrameRateInput.interactable = !disabled;
            targetFrameRateInput.text = (!disabled ? (ParamatersManager.Instance.TargetFrameRate != null ? ParamatersManager.Instance.TargetFrameRate.ToString() : string.Empty) : string.Empty);
        });

        delayFrameMinInput.onEndEdit.AddListener(value =>
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                delayFrameMinInput.text = ParamatersManager.Instance.DelayFrameMin.ToString();
            }
        });

        decodeIntervalInput.onEndEdit.AddListener(value =>
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                decodeIntervalInput.text = ParamatersManager.Instance.DecodeInterval.ToString();
            }
        });

        autoFocusSupportedToggle.onValueChanged.AddListener(isChecked =>
        {
            container.SetActive(isChecked);
            xInput.text = (isChecked ? (ParamatersManager.Instance.WebcamAutoFocusPoint != null ? ParamatersManager.Instance.WebcamAutoFocusPoint.Value.x.ToString() : "0") : "0");
            yInput.text = (isChecked ? (ParamatersManager.Instance.WebcamAutoFocusPoint != null ? ParamatersManager.Instance.WebcamAutoFocusPoint.Value.y.ToString() : "0") : "0");
        });

        xInput.onEndEdit.AddListener(value =>
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                xInput.text = ParamatersManager.Instance.WebcamAutoFocusPoint != null ? ParamatersManager.Instance.WebcamAutoFocusPoint.Value.x.ToString() : "0";
            }
        });

        yInput.onEndEdit.AddListener(value =>
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                yInput.text = ParamatersManager.Instance.WebcamAutoFocusPoint != null ? ParamatersManager.Instance.WebcamAutoFocusPoint.Value.y.ToString() : "0";
            }
        });

        tryAgainButton.onClick.AddListener(() => CheckDevices(true));
        openScannerButton.onClick.AddListener(() => SaveParameters());
        quitButton.onClick.AddListener(() => Application.Quit());
    }

    private void ClearFields()
    {
        TMP_Dropdown[] dropdowns =
        {
            devicesDropdown, qualitySettingsDropdown
        };

        TMP_InputField[] inputs =
        {
            targetFrameRateInput, requestedFpsInput, delayFrameMinInput, decodeIntervalInput
        };

        foreach (TMP_Dropdown item in dropdowns)
        {
            item.ClearOptions();
        }

        foreach (TMP_InputField item in inputs)
        {
            item.text = string.Empty;
        }

        parserTryHarderToggle.isOn = false;
    }

    private void FillFields()
    {
        foreach (WebCamDevice device in WebCamTexture.devices)
        {
            devicesDropdown.options.Add(new TMP_Dropdown.OptionData(device.name));
        }

        WebCamDevice currentDevice = WebCamTexture.devices[0];
        ClearAndFillResolutions(currentDevice);

        foreach (string name in QualitySettings.names)
        {
            qualitySettingsDropdown.options.Add(new TMP_Dropdown.OptionData(name));
        }

        targetFrameRateInput.text = (ParamatersManager.Instance.TargetFrameRate != null ? ParamatersManager.Instance.TargetFrameRate.ToString() : string.Empty);
        requestedFpsInput.text = (ParamatersManager.Instance.RequestedFPS != null ? ParamatersManager.Instance.RequestedFPS.ToString() : string.Empty);
        vSyncCountDropdown.value = ParamatersManager.Instance.VSyncCount;
        vSyncCountDropdown.onValueChanged.Invoke(vSyncCountDropdown.value);
        qualitySettingsDropdown.value = ParamatersManager.Instance.QualitySettingsLevel;
        qualitySettingsDropdown.RefreshShownValue();
        delayFrameMinInput.text = ParamatersManager.Instance.DelayFrameMin.ToString();
        decodeIntervalInput.text = ParamatersManager.Instance.DecodeInterval.ToString();
        parserTryHarderToggle.isOn = ParamatersManager.Instance.ParserTryHarder;
        webcamFilterModeDropdown.value = ParamatersManager.Instance.WebcamFilterMode;
    }

    private void ClearAndFillResolutions(WebCamDevice device)
    {
        resolutionsDropdown.ClearOptions();
        resolutionsDropdown.options.Add(new TMP_Dropdown.OptionData("Don't set - Default Scanner Resolution"));
        foreach (Resolution resolution in device.availableResolutions)
        {
            string content = string.Format("{0}x{1} - {2} rate", resolution.width, resolution.height, resolution.refreshRate);
            resolutionsDropdown.options.Add(new TMP_Dropdown.OptionData(content));
        }

        autoFocusSupportedToggle.isOn = device.isAutoFocusPointSupported;
        autoFocusSupportedToggle.onValueChanged.Invoke(autoFocusSupportedToggle.isOn);
    }

    private void SaveParameters()
    {
        WebCamDevice device = WebCamTexture.devices[devicesDropdown.value];
        ParamatersManager.Instance.DeviceName = device.name;
        int pickedResolution = resolutionsDropdown.value;
        if (pickedResolution >= 1)
        {
            ParamatersManager.Instance.Resolution = device.availableResolutions[pickedResolution - 1];
        }

        if (!string.IsNullOrEmpty(targetFrameRateInput.text) && !string.IsNullOrWhiteSpace(targetFrameRateInput.text))
        {
            ParamatersManager.Instance.TargetFrameRate = int.Parse(targetFrameRateInput.text);
        }

        if (!string.IsNullOrEmpty(requestedFpsInput.text) && !string.IsNullOrWhiteSpace(requestedFpsInput.text))
        {
            ParamatersManager.Instance.RequestedFPS = float.Parse(requestedFpsInput.text);
        }

        ParamatersManager.Instance.VSyncCount = vSyncCountDropdown.value;
        ParamatersManager.Instance.QualitySettingsLevel = qualitySettingsDropdown.value;
        ParamatersManager.Instance.DelayFrameMin = int.Parse(delayFrameMinInput.text);
        ParamatersManager.Instance.DecodeInterval = float.Parse(decodeIntervalInput.text, CultureInfo.InvariantCulture);
        ParamatersManager.Instance.ParserTryHarder = parserTryHarderToggle.isOn;
        ParamatersManager.Instance.WebcamFilterMode = webcamFilterModeDropdown.value;
        ParamatersManager.Instance.ApplyParamaters = applySettingsToggle.isOn;

        SceneManager.LoadScene("ScannerScene");
    }

    private void ToggleCanvas(GameObject canvas, CanvasGroup canvasGroup)
    {
        canvas.SetActive(!canvas.activeSelf);
        canvasGroup.alpha = (canvas.activeSelf ? 1f : 0f);
        canvasGroup.interactable = canvas.activeSelf;
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionSetting : MonoBehaviour
{
    private Resolution[] _resolutions;
    [SerializeField] private TMP_Dropdown _dropdown;

    void Start()
    {
        _resolutions = Screen.resolutions;
        _dropdown.ClearOptions();

        var options = new List<string>();
        foreach (var res in _resolutions)
        {
            options.Add(res.width + " x " + res.height);
        }
        _dropdown.AddOptions(options);
    }

    public void OnResolutionChanged(int index)
    {
        Resolution res = _resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
    }
}

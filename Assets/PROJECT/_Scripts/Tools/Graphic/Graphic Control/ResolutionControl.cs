using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using Service;

namespace Settings
{
    public class ResolutionControl : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _resolutionDropdown;

        private List<Resolution> _filteredResolutions;
        private IGraphicsService _graphicsService;

        [Inject]
        public void Construct(IGraphicsService graphicsService)
        {
            _graphicsService = graphicsService;
        }

        private void Awake()
        {
            _filteredResolutions = new List<Resolution>();

            _resolutionDropdown.ClearOptions();

            Resolution[] availableResolutions = Screen.resolutions;
            int currentRefreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.numerator / Screen.currentResolution.refreshRateRatio.denominator);

            foreach (var res in availableResolutions)
            {
                int refreshRate = Mathf.RoundToInt((float)res.refreshRateRatio.numerator / res.refreshRateRatio.denominator);
                if (refreshRate == currentRefreshRate)
                {
                    _filteredResolutions.Add(res);
                }
            }

            if (_filteredResolutions.Count == 0)
            {
                Debug.LogWarning("No matching refresh rate resolutions found. Using all available.");
                _filteredResolutions.AddRange(availableResolutions);
            }

            List<string> options = new();
            foreach (var res in _filteredResolutions)
            {
                int refresh = Mathf.RoundToInt((float)res.refreshRateRatio.numerator / res.refreshRateRatio.denominator);
                options.Add($"{res.width}x{res.height} {refresh}Hz");
            }

            _resolutionDropdown.AddOptions(options);

            var savedResolution = _graphicsService.GetResolution();
            int currentIndex = _filteredResolutions.FindIndex(r =>
            {
                int refresh = Mathf.RoundToInt((float)r.refreshRateRatio.numerator / r.refreshRateRatio.denominator);
                return r.width == savedResolution.Width &&
                       r.height == savedResolution.Height &&
                       refresh == savedResolution.RefreshRate;
            });

            _resolutionDropdown.value = Mathf.Clamp(currentIndex, 0, _filteredResolutions.Count - 1);
            _resolutionDropdown.RefreshShownValue();

            if (currentIndex >= 0)
                ApplyResolution(currentIndex);

            _resolutionDropdown.onValueChanged.AddListener(ApplyResolution);
        }

        private void ApplyResolution(int index)
        {
            var res = _filteredResolutions[Mathf.Clamp(index, 0, _filteredResolutions.Count - 1)];
            int refreshRate = Mathf.RoundToInt((float)res.refreshRateRatio.numerator / res.refreshRateRatio.denominator);

            var resolutionData = new ResolutionData(res.width, res.height, refreshRate);
            _graphicsService.SetResolution(resolutionData);
        }

        private void OnDestroy()
        {
            _resolutionDropdown.onValueChanged.RemoveListener(ApplyResolution);
        }
    }
}

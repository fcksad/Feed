using Service;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace Settings
{
    public class VSynceControl : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _vSynceModeDropdown;

        private IGraphicsService _graphicsService;

        [Inject]
        public void Construct(IGraphicsService graphicsService)
        {
            _graphicsService = graphicsService;
        }

        private void Awake()
        {
            List<TMP_Dropdown.OptionData> VSyncenOptions = new List<TMP_Dropdown.OptionData>();
            VSyncenOptions.Add(new TMP_Dropdown.OptionData("Enable"));
            VSyncenOptions.Add(new TMP_Dropdown.OptionData("Disable"));

            _vSynceModeDropdown.options = VSyncenOptions;
            _vSynceModeDropdown.value = _graphicsService.Get(GraphicType.VSync);
            _vSynceModeDropdown.RefreshShownValue();

            _vSynceModeDropdown.onValueChanged.AddListener(SetVSynce);
        }

        private void SetVSynce(int selectedVSynceIndex)
        {
            _graphicsService.Set(GraphicType.VSync, selectedVSynceIndex);
        }

        private void OnDestroy()
        {
            _vSynceModeDropdown.onValueChanged.RemoveListener(SetVSynce);
        }
    }
}

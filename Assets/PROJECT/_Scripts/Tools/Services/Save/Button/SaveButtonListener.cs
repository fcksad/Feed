using Zenject;

public class SaveButtonListener : CustomButton
{
    private ISaveService _saveService;

    [Inject]
    public void Construct(ISaveService saveService)
    {
        _saveService = saveService;
    }

    private void Start()
    {
        Button.onClick.AddListener(SaveSettings);
    }

    private void SaveSettings()
    {
        _saveService.SaveSettings();
    }
}

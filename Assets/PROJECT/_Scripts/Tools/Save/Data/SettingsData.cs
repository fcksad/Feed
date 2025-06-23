
[System.Serializable]
public class SettingsData 
{
    public AudioData AudioData;
    public GraphicsData GraphicsData;
    public LocalizationData LocalizationData;
    public ControlsData ControlsData;
    public CharacterSettingsData CharacterSettingsData;
    public FPSData FPSData;

    public SettingsData() 
    {
        AudioData = new AudioData();
        GraphicsData = new GraphicsData();
        LocalizationData = new LocalizationData();
        ControlsData = new ControlsData();
        CharacterSettingsData = new CharacterSettingsData();
        FPSData = new FPSData();
    }
}

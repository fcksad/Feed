
[System.Serializable]
public class SettingsData 
{
    public AudioData AudioData;
    public GraphicsData GraphicsData;
    public LocalizationData LocalizationData;
    public ControlsData ControlsData;
    public SettingsData() 
    {
        AudioData = new AudioData();
        GraphicsData = new GraphicsData();
        LocalizationData = new LocalizationData();
        ControlsData = new ControlsData();
    }
}

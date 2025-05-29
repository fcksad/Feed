using System;

[System.Serializable]
public class GraphicsData
{
    public int QualityLevel;
    public int VSync;
    public int ScreenMode;
    public ResolutionData Resolution;
    public int FPS;

    public GraphicsData()
    {
        QualityLevel = 2;      
        VSync = 1;    
        ScreenMode = 2;           
        Resolution = new ResolutionData(1920, 1080, 60);
        FPS = 60;
    }
}

[Serializable]
public class ResolutionData
{
    public int Width;
    public int Height;
    public int RefreshRate;

    public ResolutionData(int width, int height, int refreshRate)
    {
        Width = width;
        Height = height;
        RefreshRate = refreshRate;
    }
}

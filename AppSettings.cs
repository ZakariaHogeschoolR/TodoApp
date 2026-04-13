public static class AppSettings
{
    private const string ConfigPath = "config.txt";
    public static string Mode { get; set; } = "ARRAY"; // Standaard

    public static void Save() => File.WriteAllText(ConfigPath, Mode);
    
    public static void Load()
    {
        if (File.Exists(ConfigPath))
            Mode = File.ReadAllText(ConfigPath);
    }
}
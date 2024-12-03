namespace RakazielPsycasts;

/// <summary>
///     Wraps the game's logging system with a custom prefix and color.
/// </summary>
public static class Log
{
    /// <summary>
    ///     The color of the log prefix.
    /// </summary>
    private const string color = "#1c6beb";

    /// <summary>
    ///     The name of the mod.
    /// </summary>
    private const string mod = "RakazielPsycasts";

    /// <summary>
    ///     The prefix of the log messages.
    /// </summary>
    public static string Prefix => $"<color={color}>[{mod}] ";

    /// <summary>
    ///     The suffix of the log messages.
    /// </summary>
    public static string Suffix => "</color>";

    /// <summary>
    ///     Whether the log window should open when a message is logged.
    /// </summary>
    public static bool OpenOnMessage
    {
        get => Verse.Log.openOnMessage;
        set => Verse.Log.openOnMessage = value;
    }

    /// <summary>
    ///     Logs a message to the console.
    /// </summary>
    /// <param name="text"></param>
    public static void Message(string text)
    {
        Verse.Log.Message($"{Prefix}{text}{Suffix}");
    }

    /// <summary>
    ///     Logs an object to the console.
    /// </summary>
    /// <param name="obj"></param>
    public static void Message(object obj)
    {
        Message(obj.ToString());
    }

    /// <summary>
    ///     Logs a message to the console once.
    /// </summary>
    /// <param name="text"></param>
    public static void Warning(string text)
    {
        Verse.Log.Warning($"{Prefix}{text}{Suffix}");
    }

    /// <summary>
    ///     Logs a message to the console once.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="key"></param>
    public static void WarningOnce(string text, int key)
    {
        Verse.Log.WarningOnce($"{Prefix}{text}{Suffix}", key);
    }

    /// <summary>
    ///     Logs an error to the console.
    /// </summary>
    /// <param name="text"></param>
    public static void Error(string text)
    {
        Verse.Log.Error($"{Prefix}{text}{Suffix}");
    }

    /// <summary>
    ///     Logs an error to the console once.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="key"></param>
    public static void ErrorOnce(string text, int key)
    {
        Verse.Log.ErrorOnce($"{Prefix}{text}{Suffix}", key);
    }

    /// <summary>
    ///     Clears the console.
    /// </summary>
    public static void Clear()
    {
        Verse.Log.Clear();
    }

    /// <summary>
    ///     Opens the console.
    /// </summary>
    public static void TryOpenLogWindow()
    {
        Verse.Log.TryOpenLogWindow();
    }
}

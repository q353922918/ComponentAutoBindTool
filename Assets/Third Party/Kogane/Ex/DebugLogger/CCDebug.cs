using Kogane;

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public static class CCDebug
{
    private static readonly IDebugLogger GameTaggedDebugLogger =
        TaggedDebugLogger.Create("[CCDebug] <color=green>[Game]</color>");
    private static readonly IDebugLogger BattleTaggedDebugLogger = 
        TaggedDebugLogger.Create("[CCDebug] [Battle]");

    public static void GameLog(string obj)
    {
        GameTaggedDebugLogger.Log(obj);
    }

    public static void BattleLog(string obj)
    {
        BattleTaggedDebugLogger.Log(obj);
    }
    
    public static void LogError(string obj)
    {
        BattleTaggedDebugLogger.LogError(obj);
    }
}
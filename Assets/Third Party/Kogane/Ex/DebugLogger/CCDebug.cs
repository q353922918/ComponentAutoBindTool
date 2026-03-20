using System;
using Kogane;

public enum LogType
{
    System,
    Game,
    Battle,
}

// ReSharper disable once CheckNamespace
// ReSharper disable once InconsistentNaming
public static class CCDebug
{
    private static readonly IDebugLogger GameTaggedDebugLogger =
        TaggedDebugLogger.Create("[CCDebug] [Game]");
    private static readonly IDebugLogger BattleTaggedDebugLogger = 
        TaggedDebugLogger.Create("[CCDebug] <color=green>[Battle]</color>");
    private static readonly IDebugLogger SystemTaggedDebugLogger = 
        TaggedDebugLogger.Create("[CCDebug] <color=blue>[System]</color>");

    public static void Log(string obj, LogType logType = LogType.Game)
    {
        switch (logType)
        {
            case LogType.System:
                SystemTaggedDebugLogger.Log(obj);
                break;
            case LogType.Game:
                GameTaggedDebugLogger.Log(obj);
                break;
            case LogType.Battle:
                BattleTaggedDebugLogger.Log(obj);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
        }   
    }
}
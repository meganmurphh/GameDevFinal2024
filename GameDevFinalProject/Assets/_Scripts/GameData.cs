public static class GameData
{
    public static int Score { get; set; }
    public static int Lives { get; set; }
    public static float RemainingTime { get; set; }
    public static int CurrentLevel { get; set; }
    public static int FinalScore { get; set; }

    public static void Reset()
    {
        Score = 0;
        Lives = 3;
        RemainingTime = 240f;
        CurrentLevel = 0;
        FinalScore = 0;
    }
}

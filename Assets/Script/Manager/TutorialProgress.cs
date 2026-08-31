using UnityEngine;

public static class TutorialProgress
{
    private const string CompletedKey = "TutorialCompleted";

    public static bool IsCompleted => PlayerPrefs.GetInt(CompletedKey, 0) == 1;

    public static void SaveCompleted()
    {
        PlayerPrefs.SetInt(CompletedKey, 1);
        PlayerPrefs.Save();
    }
}

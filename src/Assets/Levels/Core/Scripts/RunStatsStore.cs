using System.Collections.Generic;
using UnityEngine;

namespace SuperPorkOut.Core
{
    public static class RunStatsStore
    {
        private const int MaxEntries = 5;
        private const string PrefsKeyPrefix = "RunStats.";

        public static void Save(string sceneName, RunStatsEntry entry)
        {
            var board = Load(sceneName);
            board.entries.Add(entry);
            board.entries.Sort((a, b) => b.distanceTraveled.CompareTo(a.distanceTraveled));

            if (board.entries.Count > MaxEntries)
                board.entries.RemoveRange(MaxEntries, board.entries.Count - MaxEntries);

            string json = JsonUtility.ToJson(board);
            PlayerPrefs.SetString(PrefsKeyPrefix + sceneName, json);
            PlayerPrefs.Save();
        }

        public static RunStatsBoard Load(string sceneName)
        {
            string key = PrefsKeyPrefix + sceneName;

            if (!PlayerPrefs.HasKey(key))
                return new RunStatsBoard();

            string json = PlayerPrefs.GetString(key);
            var board = JsonUtility.FromJson<RunStatsBoard>(json);
            return board ?? new RunStatsBoard();
        }

        public static List<RunStatsEntry> GetTopRuns(string sceneName)
        {
            return Load(sceneName).entries;
        }
    }
}

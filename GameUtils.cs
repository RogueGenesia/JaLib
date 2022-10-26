using HarmonyLib;

using RogueGenesia.Data;

namespace JaLib
{
    public static class GameUtils
    {
        public delegate void OnStartNewGameHandler();
        public delegate void OnGameStartHandler();
        public delegate void OnGameEndHandler();
        public delegate void OnPlayerFinalDeathHandler();

        public static event OnStartNewGameHandler OnStartNewGameEvent;
        public static event OnGameStartHandler OnGameStartEvent;
        public static event OnGameEndHandler OnGameEndEvent;
        public static event OnPlayerFinalDeathHandler OnPlayerFinalDeathEvent;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "OnStartNewGame")]
        internal static bool GameData_OnStartNewGame()
        {
            OnStartNewGameEvent?.Invoke();
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "OnGameStart")]
        internal static bool GameData_OnGameStart()
        {
            OnGameStartEvent?.Invoke();
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "OnGameEnd")]
        internal static bool GameData_OnGameEnd()
        {
            OnGameEndEvent?.Invoke();
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameData), "OnPlayerFinalDeath")]
        internal static bool GameData_OnPlayerFinalDeathHandler()
        {
            OnPlayerFinalDeathEvent?.Invoke();
            return true;
        }
    }
}

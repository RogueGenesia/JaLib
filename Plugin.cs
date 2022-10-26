using BepInEx;
using BepInEx.Unity.IL2CPP;

using HarmonyLib;

namespace JaLib
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        private const string _HARMONY_ID = "HARMONY_JALIB";

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(GameUtils), _HARMONY_ID);
        }

        public override bool Unload()
        {
            Harmony.UnpatchID(_HARMONY_ID);
            return true;
        }
    }
}

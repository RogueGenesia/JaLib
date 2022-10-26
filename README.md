# JaLib

A modding library to help making mods in Rogue Genesia

## How to use it

### Add JaLib as dependency

Add `JaLib.dll` as reference in your csproj file:

```csproj
<Reference Include="<Path_to Rogue Genesia game folder>\BepInEx\plugins\JaLib\JaLib.dll" />
```

Add `JaLib` as a dependency in your BepInEx's Plugin class

```cs
using BepInEx;
using BepInEx.Unity.IL2CPP;

namespace MyMod
{
    [BepInDependency("JaLib")]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        // ...
    }
}
```

### AssetsUtils

```cs
public static Texture2D LoadTextureFromFile(string texturePath)
public static Sprite LoadSpriteFromFile(string spritePath)
public static Sprite LoadSpriteFromFile(string spritePath, Vector2 pivot)
// not guaranteed to works
public static AudioClip LoadAudioClipFromFile(string audioClipPath, AudioType type)
// doesn't work due to an Unity error
public static AssetBundle LoadAssetBundleFromFile(string bundlePath)
```

### CardUtils

```cs
public static void AddStatCard(CardTemplate cardTemplate)
public static void AddCustomCard<T>(CardTemplate cardTemplate) where T : RogueGenesia.Data.SoulCard
public static void AddWeaponCard<T>(CardTemplate cardTemplate) where T : RogueGenesia.Actors.Survival.Weapon
```

### Constants

```cs
// contains all vanilla souls cards names
public static class SoulCardsNames
// conains all player projectiles `Resources` path
public static class WeaponProjectiles
```

### Il2CppUtils

```cs
// return a new instance of `T` created on Il2Cpp side
public static T NewILOjectInstance<T>() where T : Il2CppObjectBase
```

### GameUtils

```cs
public static event OnStartNewGameHandler OnStartNewGameEvent;
public static event OnGameStartHandler OnGameStartEvent;
public static event OnGameEndHandler OnGameEndEvent;
public static event OnPlayerFinalDeathHandler OnPlayerFinalDeathEvent;

// usage

JaLib.GameUtils.OnStartNewGameHandler += () => {
    // do things when a new game is started
};
```
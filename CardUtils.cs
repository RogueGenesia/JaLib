using System.Collections.Generic;
using System.Linq;

using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

using Il2CppSystem;

using ModGenesia;

using RogueGenesia.Data;
using RogueGenesia.Actors.Survival;

namespace JaLib
{
    public static class CardUtils
    {
        public struct ModifierTemplate
        {
            public ModifierType ModifierType { get; set; }
            public StatsType StatsType { get; set; }
            public float Value { get; set; }
        }

        public struct CardTemplate
        {
            public string ModName { get; set; }
            public string Name { get; set; }
            public CardTag Tags { get; set; }
            public Dictionary<string, string> NameLocalization { get; set; }
            public Dictionary<string, string> DescriptionLocalization { get; set; }
            public ModifierTemplate[] Modifiers { get; set; }
            public Dictionary<string, int> RequiresAll { get; set; }
            public Dictionary<string, int> RequiresAny { get; set; }
            public string[] CardsToRemove { get; set; }
            public string[] CardsToBanByName { get; set; }
            public StatsType[] cardsToBanByStatsOfType { get; set; }
            public CardRarity Rarity { get; set; }
            public int MaxLevel { get; set; }
            public float DropWeight { get; set; }
            public float LevelUpWeight { get; set; }
            public string SpritePath { get; set; }
        }

        private static Dictionary<string, UnityEngine.Localization.Locale> _LOCALES_CACHE = null;

        public static void AddStatCard(CardTemplate cardTemplate)
        {
            var souldCardCreationData = CreateSouldCardCreationData(cardTemplate);
            var so = ModGenesia.ModGenesia.AddCustomStatCard(
                cardTemplate.Name,
                souldCardCreationData
            );

            ApplyDescriptionLocalization(so, cardTemplate.DescriptionLocalization);
        }

        public static void AddCustomCard<T>(CardTemplate cardTemplate) where T : SoulCard
        {
            var customCardType = typeof(T);
            var souldCardCreationData = CreateSouldCardCreationData(cardTemplate);
            ClassInjector.RegisterTypeInIl2Cpp(customCardType);
            var ptr = IL2CPP.GetIl2CppClass(
                "Assembly-CSharp.dll",
                customCardType.Namespace,
                customCardType.Name
            );

            var so = ModGenesia.ModGenesia.AddCustomCard(
                cardTemplate.Name,
                Il2CppType.TypeFromPointer(ptr).GetConstructor(System.Array.Empty<Type>()),
                souldCardCreationData
            );

            ApplyDescriptionLocalization(so, cardTemplate.DescriptionLocalization);
        }

        public static void AddWeaponCard<T>(CardTemplate cardTemplate) where T : Weapon
        {
            var weaponClassType = typeof(T);
            var souldCardCreationData = CreateSouldCardCreationData(cardTemplate);
            ClassInjector.RegisterTypeInIl2Cpp(weaponClassType);
            var ptr = IL2CPP.GetIl2CppClass(
                "Assembly-CSharp.dll",
                weaponClassType.Namespace,
                weaponClassType.Name
            );

            var so = ModGenesia.ModGenesia.AddCustomWeapon(
                cardTemplate.Name,
                Il2CppType.TypeFromPointer(ptr).GetConstructor(System.Array.Empty<Type>()),
                souldCardCreationData
            );

            ApplyDescriptionLocalization(so, cardTemplate.DescriptionLocalization);
        }

        private static Dictionary<string, UnityEngine.Localization.Locale> GetLocales()
        {
            if (_LOCALES_CACHE != null)
                return _LOCALES_CACHE;

            _LOCALES_CACHE = new Dictionary<string, UnityEngine.Localization.Locale>();

            foreach (var locale in ModGenesia.ModGenesia.GetLocales())
            {
                _LOCALES_CACHE.Add(locale.Identifier.Code, locale);
            }

            return _LOCALES_CACHE;
        }

        private static SoulCardCreationData CreateSouldCardCreationData(CardTemplate cardTemplate)
        {
            SoulCardCreationData card = Il2CppUtils.NewILOjectInstance<SoulCardCreationData>();

            card.ModSource = cardTemplate.ModName;
            card.Tags = cardTemplate.Tags;
            card.Rarity = cardTemplate.Rarity;
            card.MaxLevel = cardTemplate.MaxLevel;
            card.DropWeight = cardTemplate.DropWeight;
            card.LevelUpWeight = cardTemplate.LevelUpWeight;
            card.Texture = AssetUtils.LoadSpriteFromFile(cardTemplate.SpritePath);
            card.NameOverride = new Il2CppSystem.Collections.Generic.List<LocalizationData>();

            if (cardTemplate.NameLocalization != null && cardTemplate.NameLocalization.Count > 0)
            {
                var locales = GetLocales();

                foreach (var kv in cardTemplate.NameLocalization)
                {
                    if (locales.TryGetValue(kv.Key, out var locale))
                    {
                        LocalizationData localization =
                            Il2CppUtils.NewILOjectInstance<LocalizationData>();

                        localization.Key = locale;
                        localization.Value = kv.Value;

                        card.NameOverride.Add(localization);
                    }
                }
            }

            if (cardTemplate.Modifiers != null && cardTemplate.Modifiers.Length > 0)
            {
                card.ModifyPlayerStat = true;
                card.StatsModifier = Il2CppUtils.NewILOjectInstance<StatsModifier>();
                card.StatsModifier.ModifiersList =
                    new Il2CppSystem.Collections.Generic.List<StatModifier>();

                for (int i = 0; i < cardTemplate.Modifiers.Length; i++)
                {
                    var statModifier = Il2CppUtils.NewILOjectInstance<StatModifier>();
                    statModifier.Value = Il2CppUtils.NewILOjectInstance<SingularModifier>();

                    statModifier.Value.Value = cardTemplate.Modifiers[i].Value;
                    statModifier.Value.ModifierType = cardTemplate.Modifiers[i].ModifierType;

                    statModifier.Key = cardTemplate.Modifiers[i].StatsType.ToString();

                    card.StatsModifier.ModifiersList.Add(statModifier);
                }
            }

            card.CardHardRequirement = Il2CppUtils.NewILOjectInstance<SCSORequirementList>();

            if (cardTemplate.RequiresAll != null && cardTemplate.RequiresAll.Count > 0)
            {
                ModCardRequirement[] modCardRequirements = new ModCardRequirement[
                    cardTemplate.RequiresAll.Count
                ];

                foreach (
                    var item in cardTemplate.RequiresAll.Select(
                        (Entry, Index) => new { Entry, Index }
                    )
                )
                {
                    ModCardRequirement requirement =
                        Il2CppUtils.NewILOjectInstance<ModCardRequirement>();

                    requirement.cardName = item.Entry.Key;
                    requirement.requiredLevel = item.Entry.Value;

                    modCardRequirements[item.Index] = requirement;
                }

                card.CardHardRequirement = ModGenesia.ModGenesia.MakeCardRequirement(
                    (Il2CppReferenceArray<ModCardRequirement>)modCardRequirements,
                    null,
                    false
                );
            }

            card.CardRequirement = Il2CppUtils.NewILOjectInstance<SCSORequirementList>();

            if (cardTemplate.RequiresAny != null && cardTemplate.RequiresAny.Count > 0)
            {
                ModCardRequirement[] modCardRequirements = new ModCardRequirement[
                    cardTemplate.RequiresAny.Count
                ];

                foreach (
                    var item in cardTemplate.RequiresAny.Select(
                        (Entry, Index) => new { Entry, Index }
                    )
                )
                {
                    ModCardRequirement requirement =
                        Il2CppUtils.NewILOjectInstance<ModCardRequirement>();

                    requirement.cardName = item.Entry.Key;
                    requirement.requiredLevel = item.Entry.Value;

                    modCardRequirements[item.Index] = requirement;
                }

                card.CardRequirement = ModGenesia.ModGenesia.MakeCardRequirement(
                    (Il2CppReferenceArray<ModCardRequirement>)modCardRequirements,
                    null,
                    false
                );
            }

            if (cardTemplate.CardsToRemove != null && cardTemplate.CardsToRemove.Length > 0)
            {
                SoulCardScriptableObject[] soulCardScriptableObjects = new SoulCardScriptableObject[
                    cardTemplate.CardsToRemove.Length
                ];

                for (int i = 0; i < cardTemplate.CardsToRemove.Length; i++)
                {
                    soulCardScriptableObjects[i] = ModGenesia.ModGenesia.GetCardFromName(
                        cardTemplate.CardsToRemove[i]
                    );
                }

                card.CardToRemove =
                    (Il2CppReferenceArray<SoulCardScriptableObject>)soulCardScriptableObjects;
            }
            else
            {
                card.CardToRemove =
                    (Il2CppReferenceArray<SoulCardScriptableObject>)
                        new SoulCardScriptableObject[] { };
            }

            if (cardTemplate.CardsToBanByName != null && cardTemplate.CardsToBanByName.Length > 0)
            {
                SoulCardScriptableObject[] soulCardScriptableObjects = new SoulCardScriptableObject[
                    cardTemplate.CardsToBanByName.Length
                ];

                for (int i = 0; i < cardTemplate.CardsToBanByName.Length; i++)
                {
                    soulCardScriptableObjects[i] = ModGenesia.ModGenesia.GetCardFromName(
                        cardTemplate.CardsToBanByName[i]
                    );
                }

                card.CardExclusion =
                    (Il2CppReferenceArray<SoulCardScriptableObject>)soulCardScriptableObjects;
            }
            else
            {
                card.CardExclusion =
                    (Il2CppReferenceArray<SoulCardScriptableObject>)
                        new SoulCardScriptableObject[] { };
            }

            if (
                cardTemplate.cardsToBanByStatsOfType != null
                && cardTemplate.cardsToBanByStatsOfType.Length > 0
            )
            {
                card.CardWithStatsToBan =
                    (Il2CppStructArray<StatsType>)cardTemplate.cardsToBanByStatsOfType;
            }
            else
            {
                card.CardWithStatsToBan = (Il2CppStructArray<StatsType>)new StatsType[] { };
            }

            return card;
        }

        private static void ApplyDescriptionLocalization(
            SoulCardScriptableObject so,
            Dictionary<string, string> descriptionLocalization
        )
        {
            so.DescriptionOverride = new Il2CppSystem.Collections.Generic.List<LocalizationData>();

            if (descriptionLocalization != null && descriptionLocalization.Count > 0)
            {
                var locales = GetLocales();

                foreach (var kv in descriptionLocalization)
                {
                    if (locales.TryGetValue(kv.Key, out var locale))
                    {
                        LocalizationData localization =
                            Il2CppUtils.NewILOjectInstance<LocalizationData>();

                        localization.Key = locale;
                        localization.Value = kv.Value;

                        so.DescriptionOverride.Add(localization);
                    }
                }
            }
        }
    }
}

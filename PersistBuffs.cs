using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PersistBuffs
{
    public class PersistBuffs : Mod
    {
        private bool Loaded = false;

        public int[] BuffsToModify = {
            BuffID.Regeneration,
            BuffID.Swiftness,
            BuffID.Gills,
            BuffID.Ironskin,
            BuffID.ManaRegeneration,
            BuffID.MagicPower,
            BuffID.Featherfall,
            BuffID.Spelunker,
            BuffID.Invisibility,
            BuffID.Shine,
            BuffID.NightOwl,
            BuffID.Battle,
            BuffID.Thorns,
            BuffID.WaterWalking,
            BuffID.Archery,
            BuffID.Hunter,
            BuffID.Gravitation,
            BuffID.WellFed,
            BuffID.Clairvoyance,
            BuffID.Merfolk,
            BuffID.Honey,
            BuffID.RapidHealing,
            BuffID.ShadowDodge,
            BuffID.Panic,
            BuffID.WeaponImbueVenom,
            BuffID.WeaponImbueCursedFlames,
            BuffID.WeaponImbueFire,
            BuffID.WeaponImbueGold,
            BuffID.WeaponImbueIchor,
            BuffID.WeaponImbueNanites,
            BuffID.WeaponImbueConfetti,
            BuffID.WeaponImbuePoison,
            BuffID.AmmoBox,
            BuffID.Mining,
            BuffID.Heartreach,
            BuffID.Calm,
            BuffID.Builder,
            BuffID.Titan,
            BuffID.Flipper,
            BuffID.Summoning,
            BuffID.Dangersense,
            BuffID.AmmoReservation,
            BuffID.Lifeforce,
            BuffID.Endurance,
            BuffID.Rage,
            BuffID.Inferno,
            BuffID.Wrath,
            BuffID.Lovestruck,
            BuffID.Fishing,
            BuffID.Sonar,
            BuffID.Crate,
            BuffID.Warmth,
            BuffID.Bewitched,
            BuffID.Sharpened,
            BuffID.NebulaUpLife1,
            BuffID.NebulaUpLife2,
            BuffID.NebulaUpLife3,
            BuffID.NebulaUpMana1,
            BuffID.NebulaUpMana2,
            BuffID.NebulaUpMana3,
            BuffID.NebulaUpDmg1,
            BuffID.NebulaUpDmg2,
            BuffID.NebulaUpDmg3,
            BuffID.SugarRush,
            BuffID.ParryDamageBuff,
            BuffID.WellFed2,
            BuffID.WellFed3,
            BuffID.Lucky,
            BuffID.TitaniumStorm,
            BuffID.SwordWhipPlayerBuff,
            BuffID.ScytheWhipPlayerBuff,
            BuffID.CoolWhipPlayerBuff,
            BuffID.ThornWhipPlayerBuff,
            BuffID.HeartyMeal,
        };

        public List<int> DefaultPersistentBuffs = new();

        public override void Load()
        {
            // Save default persistent buffs.
            for (int i = 0; i < Main.persistentBuff.Length; i++)
            {
                if (Main.persistentBuff[i] == true)
                    DefaultPersistentBuffs.Add(i);
            }

            var config = ModContent.GetInstance<PersistBuffsServerConfig>();

            HandlePersistBuffs(config);

            Loaded = true;
        }

        public override void Unload()
        {
            // Disable persistance on mod unload.
            SetBuffsPersistance(BuffsToModify, false);

            Loaded = false;
        }

        public void HandlePersistBuffs(PersistBuffsServerConfig config)
        {
            if (config.ModEnabled)
            {
                // disable persistance of all buffs before changing it. So there's no conflict with the previous changes.
                SetBuffsPersistance(BuffsToModify, false);

                var hasOnlyBuffs = config.OnlyBuffs.Count > 0;

                List<int> excludedBuffs = new();

                if (config.ExcludedBuffs.Count > 0)
                {
                    excludedBuffs.AddRange(GetBuffsFromStringList(config.ExcludedBuffs));
                }

                SetBuffsPersistance(
                    buffs: hasOnlyBuffs ? GetBuffsFromStringList(config.OnlyBuffs) : BuffsToModify,
                    enabled: true,
                    excludedBuffs: hasOnlyBuffs ? null : excludedBuffs
                );

            }
            else
            {
                SetBuffsPersistance(BuffsToModify, false);
            }
        }

        public void OnServerConfigChanged(PersistBuffsServerConfig config)
        {
            if (!Loaded) return; // prevent doing things before the mod loads.

            HandlePersistBuffs(config);
        }

        public int[] GetBuffsFromStringList(List<string> list)
        {
            List<int> buffs = new();

            foreach (string str in list)
            {
                int buffId;
                if (int.TryParse(str, out buffId))
                {
                    buffs.Add(buffId);
                }
                else if (BuffID.Search.TryGetId(str.Trim(), out buffId))
                {
                    buffs.Add(buffId);
                }
            }

            return buffs.ToArray();
        }


        public void SetBuffsPersistance(int[] buffs, bool enabled, List<int> excludedBuffs = null, bool ignoreDefaultPersistantBuffs = true)
        {
            for (int i = 0; i < buffs.Length; i++)
            {
                var buffID = buffs[i];

                if (ignoreDefaultPersistantBuffs && DefaultPersistentBuffs.Contains(buffID)) continue;
                if (excludedBuffs != null && excludedBuffs.Contains(buffID)) continue;
                if (buffID > 0 && buffID < BuffID.Count) Main.persistentBuff[buffID] = enabled;
            }
        }
    }
}
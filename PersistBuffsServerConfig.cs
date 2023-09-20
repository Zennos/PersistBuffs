using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PersistBuffs
{
    public class PersistBuffsServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(true)]
        public bool ModEnabled = true;

        public List<string> ExcludedBuffs = new();

        public List<string> OnlyBuffs = new();

        public override void OnChanged()
        {
            ModContent.GetInstance<PersistBuffs>().OnServerConfigChanged(this);
        }

        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
        {
            // prevent clients other than host to change the config.
            if (!IsPlayerLocalServerOwner(Main.player[whoAmI]))
            {
                message = Language.GetTextValue("Mods.PersistBuffs.Configs.OnlyOwner");
                return false;
            }
            return base.AcceptClientChanges(pendingConfig, whoAmI, ref message);
        }

        public static bool IsPlayerLocalServerOwner(Player player)
        {
            // This will check if the player is the host of the server.
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return Netplay.Connection.Socket.GetRemoteAddress().IsLocalHost();
            }
            for (int plr = 0; plr < 255; plr++)
            {
                if (Netplay.Clients[plr].State == 10 && Main.player[plr] == player && Netplay.Clients[plr].Socket.GetRemoteAddress().IsLocalHost())
                {
                    return true;
                }
            }
            return false;
        }

    }
}
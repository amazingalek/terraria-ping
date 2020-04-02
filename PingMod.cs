using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace PingMod
{
    public class PingMod : Mod
    {
        public static event Action OnPostDrawInterface;
        public static event Action OnPostDrawFullscreenMap;

        private Projectile _ping;

        public override void Load()
        {
            Main.NewText($"{nameof(PingMod)} loaded");
        }

        public override void PostUpdateEverything()
        {
            if (Main.playerLoaded && _ping == null || !_ping.active)
            {
                _ping = Projectile.NewProjectileDirect(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<Ping>(), 0, 0, Main.myPlayer);
                _ping.hide = true;
                Main.NewText("Created ping");
            }
        }

        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            OnPostDrawInterface?.Invoke();
        }

        public override void PostDrawFullscreenMap(ref string mouseText)
        {
            OnPostDrawFullscreenMap?.Invoke();
        }

    }
}

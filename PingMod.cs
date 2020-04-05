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

        public override void PostUpdateEverything()
        {
            if (Main.playerLoaded && (_ping == null || !_ping.active))
            {
                _ping = CreatePing();
            }
        }

        private Projectile CreatePing()
        {
            var ping = Projectile.NewProjectileDirect(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<Ping>(), 0, 0, Main.myPlayer);
            ping.hide = true;
            ping.ai[0] = 1;
            ping.netUpdate = true;
            return ping;
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

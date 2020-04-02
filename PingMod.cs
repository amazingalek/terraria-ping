using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace PingMod
{
    public class PingMod : Mod
    {
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
                //_ping.hide = true;
                Main.NewText("Created ping");
            }
        }

        //public override void HandlePacket(BinaryReader reader, int whoAmI)
        //{
        //    var posX = reader.ReadInt32();
        //    var posY = reader.ReadInt32();
        //    var pos = new Vector2(posX, posY);
        //    Main.NewText($"Received ping from {Main.player[whoAmI].name} at {pos}");

        //    Projectile.NewProjectileDirect(pos, Vector2.Zero, ProjectileType("Ping"), 0, 0, whoAmI);
        //}

        //public override void UpdateUI(GameTime gameTime)
        //{
        //    if (ShouldCreateWorldMapPing())
        //    {
        //        var mousePos = GetMousePos();
        //        var worldMapCoords = GetWorldMapCoords(mousePos);
        //        var pingPos = GetWorldCoords(worldMapCoords);
        //        CreatePing(pingPos);
        //    }
        //    else if (ShouldCreateMiniMapPing())
        //    {
        //        var pingPos = GetMiniMapMousePos();
        //        CreatePing(pingPos);
        //    }
        //    else if (ShouldCreateWorldPing())
        //    {
        //        var pingPos = GetWorldMousePos();
        //        CreatePing(pingPos);
        //    }
        //}

        //private Vector2 GetMousePos()
        //{
        //    return new Vector2(Main.mouseX, Main.mouseY);
        //}

        //private Point GetWorldMapMousePos()
        //{
        //    var method = typeof(CaptureInterface).GetMethod("GetMapCoords", BindingFlags.Static | BindingFlags.NonPublic);
        //    var point = new Point();
        //    method.Invoke(null, new object[] { (int)mouse.X, (int)mouse.Y, 0, point });
        //    //CaptureInterface.GetMapCoords((int)mouse.X, (int)mouse.Y, 0, out point);
        //    return point;
        //}


        //private bool ShouldCreateWorldMapPing()
        //{
        //    return Main.mapReady && Main.mapEnabled && Main.mapFullscreen && Main.mouseLeft && Main.mouseLeftRelease;
        //}

        //private void CreatePing(Point pingPos)
        //{
        //    throw new System.NotImplementedException();
        //}

    }
}

/* todo
 * ping sprite
 * ping sound
 * click on mini map: create ping
 * click on large map: create ping - see mouse hover over ore
 */

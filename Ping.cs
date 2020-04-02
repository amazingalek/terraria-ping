using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PingMod
{
    public class Ping : ModProjectile
    {
        private const float RotationSpeed = 0.05f;

        private bool _isMoving;

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;

            drawOffsetX = -10;
            drawOriginOffsetY = -11;
        }

        public override void AI()
        {
            if (!Main.player[projectile.owner].active)
            {
                projectile.Kill();
                projectile.netUpdate = true;
            }

            if (projectile.owner == Main.myPlayer)
            {
                if (Main.mouseLeft && Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
                {
                    projectile.position = Main.MouseWorld;
                    _isMoving = true;
                }

                if (_isMoving && !Main.mouseLeft)
                {
                    _isMoving = false;
                    projectile.netUpdate = true;
                    Main.NewText("Moved ping");
                }
            }

            projectile.rotation += RotationSpeed;
            projectile.timeLeft = 100;
        }

        // Based on Main.DrawInterface_20_MultiplayerPlayerNames
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var screenWidth = Main.screenWidth;
            var screenHeight = Main.screenHeight;
            var screenPosition = Main.screenPosition;
            var owner = Main.player[projectile.owner];

            var pingLabel = owner.name + "s ping";
            var pingLabelPos = Main.fontMouseText.MeasureString(pingLabel);
            var pingLabelPosYNegative = 0f;
            if (owner.chatOverhead.timeLeft > 0)
            {
                pingLabelPosYNegative = -pingLabelPos.Y;
            }
            var screenCenter = new Vector2(screenWidth / 2 + screenPosition.X, screenHeight / 2 + screenPosition.Y);
            var pingPos = projectile.position;
            var distance2 = 0f;
            var color = owner.team < 1 ? Color.White : Main.teamColor[owner.team];
            var distanceX = pingPos.X + projectile.width / 2 - screenCenter.X;
            var distanceY = pingPos.Y - pingLabelPos.Y - 2f + pingLabelPosYNegative - screenCenter.Y;
            var distance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
            var screenHeight2 = screenHeight;
            if (screenHeight > screenWidth)
            {
                screenHeight2 = screenWidth;
            }
            screenHeight2 = screenHeight2 / 2 - 30;
            if (screenHeight2 < 100)
            {
                screenHeight2 = 100;
            }
            if (distance < screenHeight2)
            {
                pingLabelPos.X = pingPos.X + projectile.width / 2 - pingLabelPos.X / 2f - screenPosition.X;
                pingLabelPos.Y = pingPos.Y - pingLabelPos.Y - 2f + pingLabelPosYNegative - screenPosition.Y;
            }
            else
            {
                distance2 = distance;
                distance = screenHeight2 / distance;
                pingLabelPos.X = screenWidth / 2 + distanceX * distance - pingLabelPos.X / 2f;
                pingLabelPos.Y = screenHeight / 2 + distanceY * distance;
            }
            if (Main.player[Main.myPlayer].gravDir == -1f)
            {
                pingLabelPos.Y = screenHeight - pingLabelPos.Y;
            }
            var pingLabelPos2 = Main.fontMouseText.MeasureString(pingLabel);
            if (distance2 > 0f)
            {
                var distanceTextValue = Language.GetTextValue("GameUI.PlayerDistance", (int)(distance2 / 16f * 2f));
                var distanceTestPosition = Main.fontMouseText.MeasureString(distanceTextValue);
                distanceTestPosition.X = pingLabelPos.X + pingLabelPos2.X / 2f - distanceTestPosition.X / 2f;
                distanceTestPosition.Y = pingLabelPos.Y + pingLabelPos2.Y / 2f - distanceTestPosition.Y / 2f - 20f;
                Main.spriteBatch.DrawString(Main.fontMouseText, distanceTextValue, new Vector2(distanceTestPosition.X - 2f, distanceTestPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(Main.fontMouseText, distanceTextValue, new Vector2(distanceTestPosition.X + 2f, distanceTestPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(Main.fontMouseText, distanceTextValue, new Vector2(distanceTestPosition.X, distanceTestPosition.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(Main.fontMouseText, distanceTextValue, new Vector2(distanceTestPosition.X, distanceTestPosition.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(Main.fontMouseText, distanceTextValue, distanceTestPosition, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.DrawString(Main.fontMouseText, pingLabel, new Vector2(pingLabelPos.X - 2f, pingLabelPos.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, pingLabel, new Vector2(pingLabelPos.X + 2f, pingLabelPos.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, pingLabel, new Vector2(pingLabelPos.X, pingLabelPos.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, pingLabel, new Vector2(pingLabelPos.X, pingLabelPos.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, pingLabel, pingLabelPos, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
        }

        //private void Sync(Vector2 pos)
        //{
        //    var packet = mod.GetPacket();
        //    packet.Write((int)pos.X);
        //    packet.Write((int)pos.Y);
        //    packet.Send();
        //    Main.NewText("Sent ping");
        //}

    }
}

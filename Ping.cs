using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PingMod
{
    public class Ping : ModProjectile
    {
        private const float RotationSpeed = 0.05f;

        private bool _isLoaded;
        private bool _isMoving;
        private bool _isRemoving;

        private Player Owner => Main.player[projectile.owner];
        private string PingLabel => Owner.name + "s ping";
        private Texture2D Sprite => Main.projectileTexture[projectile.type];

        public Ping()
        {
            PingMod.OnPostDrawInterface += DrawInterface;
            PingMod.OnPostDrawFullscreenMap += DrawOnFullscreenMap;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;

            drawOffsetX = -10;
            drawOriginOffsetY = -11;

            projectile.hide = true;
        }

        public override void AI()
        {
            if (!Owner.active || !_isLoaded)
            {
                _isLoaded = true;
                projectile.hide = true;
                projectile.ai[0] = 1;
            }

            if (projectile.owner == Main.myPlayer)
            {
                UpdateLocal();
            }
            else
            {
                UpdateRemote();
            }

            projectile.rotation += RotationSpeed;
            projectile.timeLeft = 100;
        }

        private void UpdateLocal()
        {
            if (Main.mouseLeft && Keyboard.GetState().IsKeyDown(Keys.LeftAlt) && !_isRemoving)
            {
                projectile.position = GetPingPosition();
                projectile.hide = false;
                _isMoving = true;
            }
            if (_isMoving && !Main.mouseLeft && !_isRemoving)
            {
                _isMoving = false;
                projectile.ai[0] = 0;
                projectile.netUpdate = true;
                PlaySound();
                Main.NewText("You pinged!");
            }

            if (!_isRemoving && Main.mouseRight && Keyboard.GetState().IsKeyDown(Keys.LeftAlt))
            {
                _isRemoving = true;
                _isMoving = false;
                projectile.hide = true;
                projectile.ai[0] = 1;
                projectile.netUpdate = true;
            }
            if (_isRemoving && !Main.mouseRight && !Main.mouseLeft)
            {
                _isRemoving = false;
                _isMoving = false;
            }
        }

        private void UpdateRemote()
        {
            projectile.hide = projectile.ai[0] == 1;
            if (!projectile.hide && projectile.position != projectile.oldPosition)
            {
                Main.NewText(Owner.name + " pinged!");
                PlaySound();
            }
        }

        private Vector2 GetPingPosition()
        {
            var mousePos = GetUnzoomedMousePos();
            if (Main.mapFullscreen)
            {
                return GetFullscreenMapToWorldPos(mousePos);
            }
            if (InMinimap(mousePos))
            {
                return GetMinimapToWorldPos(mousePos);
            }
            return Main.MouseWorld;
        }

        private Vector2 GetUnzoomedMousePos()
        {
            PlayerInput.SetZoom_UI();
            var mousePos = Main.MouseScreen;
            PlayerInput.SetZoom_World();
            return mousePos;
        }

        private bool InMinimap(Vector2 mousePos)
        {
            return Main.mapStyle == 1 &&
                   mousePos.X > Main.miniMapX &&
                   mousePos.X < Main.miniMapX + Main.miniMapWidth &&
                   mousePos.Y > Main.miniMapY &&
                   mousePos.Y < Main.miniMapY + Main.miniMapHeight;
        }

        private Vector2 GetMinimapToWorldPos(Vector2 mousePos)
        {
            var worldPosX = Main.player[Main.myPlayer].position.X + (16 * (mousePos.X - Main.miniMapX) - 1920) / Main.mapMinimapScale;
            var worldPosY = Main.player[Main.myPlayer].position.Y + (16 * (mousePos.Y - Main.miniMapY) - 1920) / Main.mapMinimapScale;
            return new Vector2(worldPosX, worldPosY);
        }

        private Vector2 GetFullscreenMapToWorldPos(Vector2 mousePos)
        {
            const float multiplier1 = 10;
            const float multiplier2 = 16;
            var mapScale = Main.mapFullscreenScale;
            var mapPosX = Main.mapFullscreenPos.X * mapScale;
            var mapPosY = Main.mapFullscreenPos.Y * mapScale;
            var posOffsetX = -mapPosX + Main.screenWidth / 2 + multiplier1 * mapScale;
            var posOffsetY = -mapPosY + Main.screenHeight / 2 + multiplier1 * mapScale;
            var worldPosX = (int)((-posOffsetX + mousePos.X * Main.UIScale) / mapScale + multiplier1) * multiplier2;
            var worldPosY = (int)((-posOffsetY + mousePos.Y * Main.UIScale) / mapScale + multiplier1) * multiplier2;
            return new Vector2(worldPosX, worldPosY);
        }

        private void PlaySound()
        {
            Main.PlaySound(new LegacySoundStyle(SoundID.Unlock, 0), projectile.position);
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            DrawGlowMask();
        }

        private void DrawGlowMask()
        {
            if (Main.mapFullscreen || projectile.hide)
            {
                return;
            }
            var posX = projectile.position.X - Main.screenPosition.X + projectile.width * 0.5f + 1f;
            var posY = projectile.position.Y - Main.screenPosition.Y + projectile.height - Sprite.Height * 0.5f + 11f;
            Main.spriteBatch.Draw(Sprite, new Vector2(posX, posY),
                new Rectangle(0, 0, Sprite.Width, Sprite.Height), Color.White,
                projectile.rotation, Sprite.Size() * 0.5f, 1, SpriteEffects.None, 0f);
        }

        // Based on Main.DrawMap
        private void DrawOnFullscreenMap()
        {
            if (!Main.mapFullscreen || projectile.hide)
            {
                return;
            }
            const float num6 = 10f;
            const float num7 = 10f;
            const byte b = byte.MaxValue;

            var num108 = Main.UIScale;
            var num16 = Main.mapFullscreenScale / num108;
            var num20 = Main.mapFullscreenPos.X;
            var num21 = Main.mapFullscreenPos.Y;
            num20 *= num16;
            num21 *= num16;
            var num = -num20 + Main.screenWidth / 2;
            var num2 = -num21 + Main.screenHeight / 2;
            num += num6 * num16;
            num2 += num7 * num16;

            var num111 = (projectile.position.X + projectile.width / 2) / 16f * num16;
            var num112 = (projectile.position.Y + projectile.gfxOffY + projectile.height / 2) / 16f * num16;
            num111 += num;
            num112 += num2;
            num111 -= 10f * num16;
            num112 -= 10f * num16;

            Main.spriteBatch.Draw(Sprite, new Vector2(num111, num112),
                new Rectangle(0, 0, Sprite.Width, Sprite.Height), new Color(b, b, b, b), projectile.rotation,
                new Vector2(Sprite.Width / 2, Sprite.Height / 2), num108 / 2, SpriteEffects.None, 0f);

            var num113 = num111 - Sprite.Width / 2 * num108;
            var num114 = num112 - Sprite.Height / 2 * num108;
            var num115 = num113 + Sprite.Width * num108;
            var num116 = num114 + Sprite.Height * num108;
            if (Main.mouseX >= num113 && Main.mouseX <= num115 && Main.mouseY >= num114 && Main.mouseY <= num116)
            {
                Main.instance.MouseText(PingLabel);
            }
        }

        private void DrawInterface()
        {
            DrawOnMiniMap();
            DrawDistanceMarker();
        }

        // Based on Main.DrawMap
        private void DrawOnMiniMap()
        {
            if (Main.mapFullscreen || Main.mapStyle != 1 || projectile.hide)
            {
                return;
            }
            var num145 = (Main.screenPosition.X + PlayerInput.RealScreenWidth / 2) / 16f;
            var num28 = (Main.screenPosition.Y + PlayerInput.RealScreenHeight / 2) / 16f;
            var num16 = Main.mapMinimapScale;
            var num14 = Main.miniMapWidth / num16;
            var num15 = Main.miniMapHeight / num16;
            var num12 = (int)num145 - num14 / 2f;
            var num60 = ((projectile.position.X + projectile.width / 2) / 16f - num12) * num16;
            var num13 = (int)num28 - num15 / 2f;
            var num61 = ((projectile.position.Y + projectile.gfxOffY + projectile.height / 2) / 16f - num13) * num16;
            var num = (float)Main.miniMapX;
            var num2 = (float)Main.miniMapY;
            var num3 = num;
            var num4 = num2;
            num60 += num3;
            num61 += num4;
            num61 -= 2f * num16 / 5f;
            if (num60 > Main.miniMapX + 12 && num60 < Main.miniMapX + Main.miniMapWidth - 16 &&
                num61 > Main.miniMapY + 10 && num61 < Main.miniMapY + Main.miniMapHeight - 14)
            {
                var num10 = -(num145 - (int)((Main.screenPosition.X + PlayerInput.RealScreenWidth / 2) / 16f)) * num16;
                var num11 = -(num28 - (int)((Main.screenPosition.Y + PlayerInput.RealScreenHeight / 2) / 16f)) * num16;
                var b = (byte)(255f * Main.mapMinimapAlpha);
                var num57 = (num16 * 0.25f * 2f + 1f) / 3f;
                Main.spriteBatch.Draw(Sprite, new Vector2(num60 + num10, num61 + num11),
                    new Rectangle(0, 0, Sprite.Width, Sprite.Height), new Color(b, b, b, b), projectile.rotation,
                    new Vector2(Sprite.Width / 2, Sprite.Height / 2), num57, SpriteEffects.None, 0f);
                var num62 = num60 - Sprite.Width / 2 * num57;
                var num63 = num61 - Sprite.Height / 2 * num57;
                var num64 = num62 + Sprite.Width * num57;
                var num65 = num63 + Sprite.Height * num57;
                if (Main.mouseX >= num62 && Main.mouseX <= num64 && Main.mouseY >= num63 && Main.mouseY <= num65)
                {
                    Main.instance.MouseText(PingLabel);
                }
            }
        }

        // Based on Main.DrawInterface_20_MultiplayerPlayerNames
        private void DrawDistanceMarker()
        {
            if (Main.mapFullscreen || projectile.hide)
            {
                return;
            }
            PlayerInput.SetZoom_World();
            var screenWidth = Main.screenWidth;
            var screenHeight = Main.screenHeight;
            var screenPosition = Main.screenPosition;
            PlayerInput.SetZoom_UI();
            var uIScale = Main.UIScale;

            var pingLabelPos = Main.fontMouseText.MeasureString(PingLabel);
            var pingLabelPosYNegative = 0f;
            if (Owner.chatOverhead.timeLeft > 0)
            {
                pingLabelPosYNegative = -pingLabelPos.Y;
            }
            var screenCenter = new Vector2(screenWidth / 2 + screenPosition.X, screenHeight / 2 + screenPosition.Y);
            var pingPos = projectile.position;
            pingPos += (pingPos - screenCenter) * (Main.GameViewMatrix.Zoom - Vector2.One);
            var distance2 = 0f;
            var color = Owner.team < 1 ? Color.White : Main.teamColor[Owner.team];
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
            pingLabelPos *= 1f / uIScale;
            var pingLabelPos2 = Main.fontMouseText.MeasureString(PingLabel);
            pingLabelPos += pingLabelPos2 * (1f - uIScale) / 4f;
            if (distance2 > 0f)
            {
                var distanceTextValue = Language.GetTextValue("GameUI.PlayerDistance", (int)(distance2 / 16f * 2f));
                var distanceTextPosition = Main.fontMouseText.MeasureString(distanceTextValue);
                distanceTextPosition.X = pingLabelPos.X + pingLabelPos2.X / 2f - distanceTextPosition.X / 2f;
                distanceTextPosition.Y = pingLabelPos.Y + pingLabelPos2.Y / 2f - distanceTextPosition.Y / 2f - 20f;
                Main.spriteBatch.DrawString(Main.fontMouseText, distanceTextValue, new Vector2(distanceTextPosition.X - 2f, distanceTextPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(Main.fontMouseText, distanceTextValue, new Vector2(distanceTextPosition.X + 2f, distanceTextPosition.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(Main.fontMouseText, distanceTextValue, new Vector2(distanceTextPosition.X, distanceTextPosition.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(Main.fontMouseText, distanceTextValue, new Vector2(distanceTextPosition.X, distanceTextPosition.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.DrawString(Main.fontMouseText, distanceTextValue, distanceTextPosition, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.DrawString(Main.fontMouseText, PingLabel, new Vector2(pingLabelPos.X - 2f, pingLabelPos.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, PingLabel, new Vector2(pingLabelPos.X + 2f, pingLabelPos.Y), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, PingLabel, new Vector2(pingLabelPos.X, pingLabelPos.Y - 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, PingLabel, new Vector2(pingLabelPos.X, pingLabelPos.Y + 2f), Color.Black, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.DrawString(Main.fontMouseText, PingLabel, pingLabelPos, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
        }

    }
}

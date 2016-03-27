using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Tools;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace JJB
{
    public class Class1 : Mod
    {
        public static Dictionary<int, Dictionary<int, object>> neededItems = new Dictionary<int, Dictionary<int, object>>();

        public override void Entry(params object[] objects)
        {
            RegisterCommands();
            MenuEvents.MenuChanged += Events_UpdateTick;
            GraphicsEvents.DrawTick += Events_DrawTick;
        }

        public static void RegisterCommands()
        {
        }

        static void Events_DrawTick(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu == null)
                return;

            Item obj = null;
            if (Game1.activeClickableMenu is GameMenu)
            {
                GameMenu gameMenu = (GameMenu)Game1.activeClickableMenu;
                if (gameMenu.currentTab == 0)
                    obj = (Item)typeof(InventoryPage).GetField("hoveredItem", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object)(InventoryPage)((List<IClickableMenu>)typeof(GameMenu).GetField("pages", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object)gameMenu))[0]);
                if (gameMenu.currentTab == 4)
                    obj = (Item)typeof(CraftingPage).GetField("hoverItem", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object)(CraftingPage)((List<IClickableMenu>)typeof(GameMenu).GetField("pages", BindingFlags.Instance | BindingFlags.NonPublic).GetValue((object)gameMenu))[4]);
            }
            if (Game1.activeClickableMenu is MenuWithInventory)
            {
                MenuWithInventory menuWithInventory = (MenuWithInventory)Game1.activeClickableMenu;
                obj = (Item)menuWithInventory.hoveredItem;
            }

            if (obj == null)
            {
                return;
            }

            foreach (int bundleIndex in neededItems.Keys)
            {
                if (obj.parentSheetIndex != -1 && neededItems[bundleIndex].ContainsKey(obj.parentSheetIndex))
                {
                    drawNeededText((SpriteFont)Game1.smallFont);
                }
            }

        }

        private static void drawNeededText(SpriteFont font)
        {
            string text = "Needed for a bundle";
            SpriteBatch spriteBatch = (SpriteBatch)Game1.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, (DepthStencilState)null, (RasterizerState)null);
            int width = (int)font.MeasureString(text).X + Game1.tileSize / 2 + 5;
            int height = (int)font.MeasureString(text).Y + Game1.tileSize / 3 + 5;
            int x = Game1.oldMouseState.X - Game1.tileSize / 2 - width;
            int y = Game1.oldMouseState.Y + Game1.tileSize / 2 - height;
            if (x < 0)
                x = 0;

            Viewport viewport = ((GraphicsDeviceManager)Game1.graphics).GraphicsDevice.Viewport;
            if (y + height > viewport.Height)
            {
                viewport = ((GraphicsDeviceManager)Game1.graphics).GraphicsDevice.Viewport;
                y = viewport.Height - height;
            }
            IClickableMenu.drawTextureBox(spriteBatch, (Texture2D)Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, Color.White, 1f, true);
            Utility.drawTextWithShadow(spriteBatch, text, font, new Vector2((float)(x + Game1.tileSize / 4), (float)(y + Game1.tileSize / 4)), (Color)Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
            spriteBatch.End();
        }

        static void Events_UpdateTick(object sender, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.player == null || Game1.activeClickableMenu == null)
                return;

            if (Game1.activeClickableMenu is JunimoNoteMenu)
            {
                JunimoNoteMenu v = (JunimoNoteMenu)Game1.activeClickableMenu;
                List<Bundle> bndl = new List<Bundle>(v.GetType().GetBaseFieldValue<List<Bundle>>(v, "bundles"));
                foreach (Bundle b in bndl)
                {
                    if (!neededItems.ContainsKey(b.bundleIndex))
                    {
                        neededItems.Add(b.bundleIndex, new Dictionary<int, object>());
                    }

                    foreach (BundleIngredientDescription ingredient in b.ingredients)
                    {
                        if (ingredient.completed)
                        {
                            neededItems[b.bundleIndex].Remove(ingredient.index);
                            continue;
                        }

                        if (ingredient.index != -1 && !neededItems[b.bundleIndex].ContainsKey(ingredient.index))
                        { 
                            neededItems[b.bundleIndex].Add(ingredient.index, null);
                        }

                        foreach (Item item in Game1.player.items)
                        {
                            if (item != null)
                            {
                                if (item.parentSheetIndex == ingredient.index)
                                {
                                    b.shake(0.4f);
                                    break;
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}

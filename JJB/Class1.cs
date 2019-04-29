using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace JJB
{
    public class Class1 : Mod
    {
        public static Dictionary<int, Dictionary<int, object>> neededItems = new Dictionary<int, Dictionary<int, object>>();

        public override void Entry(IModHelper helper)
        {
            Helper.Events.Display.MenuChanged += Events_UpdateTick;
            Helper.Events.Display.Rendered += Events_DrawTick;
        }

        private void Events_DrawTick(object sender, EventArgs e)
        {
            if (Game1.activeClickableMenu == null)
                return;

            Item obj = null;
            if (Game1.activeClickableMenu is GameMenu)
            {
                GameMenu gameMenu = (GameMenu)Game1.activeClickableMenu;
                var pages = Helper.Reflection.GetField<List<IClickableMenu>>(gameMenu, "pages").GetValue();
                if (gameMenu.currentTab == 0)
                {
                    obj = Helper.Reflection.GetField<Item>(pages[0], "hoveredItem").GetValue();
                    Console.WriteLine(obj?.DisplayName);
                }
                else if (gameMenu.currentTab == 4)
                {
                    obj = Helper.Reflection.GetField<Item>(pages[4], "hoverItem").GetValue();
                    Console.WriteLine(obj?.DisplayName);
                }
            }
            else if (Game1.activeClickableMenu is MenuWithInventory)
            {
                MenuWithInventory menuWithInventory = (MenuWithInventory)Game1.activeClickableMenu;
                obj = menuWithInventory.hoveredItem;
            }

            if (obj == null)
            {
                return;
            }

            foreach (int bundleIndex in neededItems.Keys)
            {
                if (obj.ParentSheetIndex != -1 && neededItems[bundleIndex].ContainsKey(obj.ParentSheetIndex))
                {
                    drawNeededText(Game1.smallFont);
                }
            }

        }

        private void drawNeededText(SpriteFont font)
        {
            string text = "Needed for a bundle";

            int width = (int)font.MeasureString(text).X + Game1.tileSize / 2 + 5;
            int height = (int)font.MeasureString(text).Y + Game1.tileSize / 3 + 5;
            int x = Game1.oldMouseState.X - Game1.tileSize / 2 - width;
            int y = Math.Max(0, Game1.oldMouseState.Y + Game1.tileSize / 2 - height);

            Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
            if (y + height > viewport.Height)
            {
                viewport = Game1.graphics.GraphicsDevice.Viewport;
                y = viewport.Height - height;
            }
            IClickableMenu.drawTextureBox(Game1.spriteBatch, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y, width, height, Color.White);
            Utility.drawTextWithShadow(Game1.spriteBatch, text, font, new Vector2(x + Game1.tileSize / 4, y + Game1.tileSize / 4), Game1.textColor);
        }

        private void Events_UpdateTick(object sender, EventArgs e)
        {
            if (!Game1.hasLoadedGame || Game1.player == null || Game1.activeClickableMenu == null)
                return;

            if (Game1.activeClickableMenu is JunimoNoteMenu)
            {
                JunimoNoteMenu menu = (JunimoNoteMenu)Game1.activeClickableMenu;
                foreach (Bundle b in menu.bundles)
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

                        foreach (Item item in Game1.player.Items)
                        {
                            if (item != null)
                            {
                                if (item.ParentSheetIndex == ingredient.index)
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

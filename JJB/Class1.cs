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

namespace JJB
{
    public class Class1 : Mod
    {
        public static int[] craftsRoom = { 16, 18, 20, 22 , 398, 396, 402 , 404, 406, 408, 410,
            412, 414, 416, 418, 388 , 390, 709, 88, 90, 78, 420, 422, 724, 725, 726, 257};

        public override void Entry(params object[] objects)
        {
            RegisterCommands();
            MenuEvents.MenuChanged += Events_UpdateTick;
        }

        public static void RegisterCommands()
        {
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
                    foreach (BundleIngredientDescription ingredient in b.ingredients)
                    {
                        if (ingredient.completed)
                            continue;

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

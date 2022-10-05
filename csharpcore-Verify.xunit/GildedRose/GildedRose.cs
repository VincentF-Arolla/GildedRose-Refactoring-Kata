using System.Collections.Generic;

namespace GildedRoseKata
{
    static class GildedRoseTestExtensions
    {
        public static bool IsAgedBrie(this Item item) => item.Name == "Aged Brie";
        public static bool IsBackstagePasses(this Item item) => item.Name.StartsWith("Backstage passes");
        public static bool IsSulfuras(this Item item) => item.Name.StartsWith("Sulfuras");
        public static bool willStrengthen(this Item item) => IsAgedBrie(item) || IsBackstagePasses(item);
        public static void nextday(this Item item) => item.SellIn--;
        public static bool expired(this Item item) => item.SellIn < 0;
        public static bool expiresIn10days(this Item item) => item.SellIn < 10;
        public static bool expiresIn5days(this Item item) => item.SellIn < 5;
        public static void strengthen(this Item item) => item.Quality++;
        public static bool strengthenable(this Item item) => item.Quality < 50;
        public static void strengthenIfPossible(this Item item) { if (strengthenable(item)) strengthen(item); }
        public static void weaken(this Item item) => item.Quality--;
        public static bool weakenable(this Item item) => item.Quality > 0;
        public static void weakenIfPossible(this Item item) { if (weakenable(item)) weaken(item); }
        public static void weakenTotally(this Item item) => item.Quality = 0;
    }

    public class GildedRose
    {
        IList<Item> Items;
        public GildedRose(IList<Item> Items)
        {
            this.Items = Items;
        }

        public void UpdateQuality()
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];

                item.nextday();

                //particular
                if (item.IsSulfuras())
                    break;

                if (item.willStrengthen())
                {

                    if (item.IsBackstagePasses()) //particular strenghtenable
                    {
                        item.strengthenIfPossible();
                        if (item.expiresIn10days()) item.strengthenIfPossible();
                        if (item.expiresIn5days()) item.strengthenIfPossible();

                        if (item.expired()) item.weakenTotally();
                    }
                    else //nominal strenghtenable
                    {
                        item.strengthenIfPossible();

                        if (item.expired()) item.strengthenIfPossible();
                    }

                    break;
                }

                //nominal weakenable
                item.weakenIfPossible();

                if (item.expired()) item.weakenIfPossible();
            }
        }
    }
}

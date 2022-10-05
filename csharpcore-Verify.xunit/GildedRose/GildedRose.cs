using System;
using System.Collections.Generic;

namespace GildedRoseKata
{
    static class GildedRoseTestExtensions
    {
        public static bool expired(this Item item) => item.SellIn < 0;
        public static void strengthen(this Item item) => item.Quality++;
        public static bool strengthenable(this Item item) => item.Quality < 50;
        public static void strengthenIfPossible(this Item item) { if (strengthenable(item)) strengthen(item); }
        public static void weaken(this Item item) => item.Quality--;
        public static bool weakenable(this Item item) => item.Quality > 0;
        public static void weakenIfPossible(this Item item) { if (weakenable(item)) weaken(item); }
    }

    public class GildedRose
    {
        IList<Item> Items;
        public GildedRose(IList<Item> Items)
        {
            this.Items = Items;
        }


        static Func<string, bool> IsAgedBrie = name => name == "Aged Brie";

        static Func<string, bool> IsBackstagePasses = name => name.StartsWith("Backstage passes");

        static Func<string, bool> IsStrengthenable =
            name => IsAgedBrie(name)
            || IsBackstagePasses(name);


        public void UpdateQuality()
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                var name = item.Name;

                item.SellIn = item.SellIn - 1;

                //particular
                if (name.StartsWith("Sulfuras"))
                    break;

                if (IsStrengthenable(name))
                {
                  

                   
                    if (IsBackstagePasses(name)) //particular strenghtenable
                    {
                        if (item.strengthenable())
                        {
                            item.strengthen();

                            if (item.SellIn < 10)
                                item.strengthen();

                            if (item.SellIn < 5)
                                item.strengthen();

                            if (item.SellIn < 0)
                                item.Quality = 0;
                        }
                    } else //nominal strenghtenable
                    {
                        if (item.strengthenable())
                        {
                            item.strengthen();

                            if (item.SellIn < 0)
                                item.strengthen();
                        }
                    }

                    break;
                }

                //nominal weakenable
                item.weakenIfPossible();

                if (item.expired())
                    item.weakenIfPossible();

            }
        }
    }
}

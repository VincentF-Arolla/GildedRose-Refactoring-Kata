using System;
using System.Collections.Generic;

namespace GildedRoseKata
{
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

        static readonly int maxValue = 50;

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
                        if (item.Quality < maxValue)
                        {
                            item.Quality++;

                            if (item.SellIn < 10)
                                item.Quality++;

                            if (item.SellIn < 5)
                                item.Quality++;

                            if (item.SellIn < 0)
                                item.Quality = 0;
                        }
                    } else //nominal strenghtenable
                    {
                        if (item.Quality < maxValue)
                        {
                            item.Quality++;

                            if (item.SellIn < 0)
                                item.Quality++;
                        }
                    }

                    break;
                }

                //nominal weakenable
                if (item.Quality > 0)
                {
                    item.Quality--;

                    if (item.SellIn < 0)
                        item.Quality--;
                }

            }
        }
    }
}

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

        public void UpdateQuality()
        {
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                var name = item.Name;



                if (name.StartsWith("Sulfuras"))
                    break;



                if (name == "Aged Brie" || name.StartsWith("Backstage passes"))
                {
                    if (item.Quality < 50)
                    {
                        item.Quality = item.Quality + 1;

                        if (name.StartsWith("Backstage passes"))
                        {
                            if (item.SellIn < 11)
                                item.Quality = item.Quality + 1;

                            if (item.SellIn < 6)
                                item.Quality = item.Quality + 1;
                        }
                    }
                }
                else
                    if (item.Quality > 0)
                        item.Quality = item.Quality - 1;



                item.SellIn = item.SellIn - 1;

                if (item.SellIn < 0)
                {
                    if (name == "Aged Brie")
                    {
                        if (item.Quality < 50)
                            item.Quality = item.Quality + 1;
                    }
                    else
                    {
                        if (name.StartsWith("Backstage passes"))
                            item.Quality = 0;
                        else
                        {
                            if (item.Quality > 0)
                                item.Quality = item.Quality - 1;
                        }
                    }
                }
            }
        }
    }
}

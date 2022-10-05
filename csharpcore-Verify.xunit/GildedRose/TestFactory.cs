using GildedRoseKata;
using System;
using System.Collections.Generic;

namespace GildedRoseKata
{
    public class TestTrial
    {
        public List<Item> stock { get; set; }

        public TestTrial(IList<Item> stock) {
            this.stock = TestFactory.getTestItems();
        }

        public void runDays(int days) => TestFactory.run(stock, days);
    }

    public class TestFactory
    {
        public static TestTrial getTestTrial() => new TestTrial(getTestItems());

        public static List<Item> getTestItems() => new List<Item>{
            new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
            new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
            new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
            new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
            new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = -1, Quality = 80},
            new Item
            {
                Name = "Backstage passes to a TAFKAL80ETC concert",
                SellIn = 15,
                Quality = 20
            },
            new Item
            {
                Name = "Backstage passes to a TAFKAL80ETC concert",
                SellIn = 10,
                Quality = 49
            },
            new Item
            {
                Name = "Backstage passes to a TAFKAL80ETC concert",
                SellIn = 5,
                Quality = 49
            },
	        // this conjured item does not work properly yet
	        new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
        };


        public static void run(List<Item> Items, int days)
        {
            var app = new GildedRose(Items);

            for (var i = 0; i <= days; i++)
            {
                Console.WriteLine("-------- day " + i + " --------");
                Console.WriteLine("name, sellIn, quality");
                for (var j = 0; j < Items.Count; j++)
                {
                    System.Console.WriteLine(Items[j].Name + ", " + Items[j].SellIn + ", " + Items[j].Quality);
                }
                Console.WriteLine("");
                app.UpdateQuality();
            }
        }
    }

}

using Xunit;
using System.Collections.Generic;
using GildedRoseKata;
using System.Linq;
using NFluent;

namespace GildedRoseTests
{
    public class GildedRoseTest
    {
        [Fact]
        public void foo()
        {
            IList<Item> Items = new List<Item> { new Item { Name = "foo", SellIn = 0, Quality = 0 } };
            GildedRose app = new GildedRose(Items);
            app.UpdateQuality();
            Assert.Equal("foo", Items[0].Name);
        }

        
        [Fact]
        public void Next_day_both_SellIn_and_Quality_lower_for_every_item()
        {
            var foo = new Item { Name = "foo", SellIn = 1000, Quality = 10 };
            var bar = new Item { Name = "bar", SellIn = 1000, Quality = 20 };
            var stock = new List<Item> { foo, bar };
            GildedRose app = new GildedRose(stock);

            var nextday = () => app.UpdateQuality();
            var lower = (int value) => value--;
            var quality = (List<Item> s) => s.Select(item => item.Quality);
            var sellin = (List<Item> s) => s.Select(item => item.SellIn);

            var quality_before = quality(stock);
            var sellin_before = sellin(stock);
            var expected_quality_after = quality_before.Select(value => lower(value));
            var expected_sellin_after = sellin_before.Select(value => lower(value));

            nextday();
            Check.That(quality(stock)).Equals(expected_quality_after);
            Check.That(sellin(stock)).Equals(expected_sellin_after);
        }

        /*
      TODO write tests for the below requirements
      Once the sell by date has passed, Quality degrades twice as fast
      The Quality of an item is never negative
      "Aged Brie" actually increases in Quality the older it gets
      The Quality of an item is never more than 50
      "Sulfuras", being a legendary item, never has to be sold or decreases in Quality
      "Backstage passes", like aged brie, increases in Quality as its SellIn value approaches;
      Quality increases by 2 when there are 10 days or less and by 3 when there are 5 days or less but
      Quality drops to 0 after the concert
      "Conjured" items degrade in Quality twice as fast as normal items
      an item can never have its Quality increase above 50, however "Sulfuras" is a legendary item and as such its Quality is 80 and it never alters.
         */


    }
}

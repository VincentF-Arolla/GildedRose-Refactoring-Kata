using Xunit;
using System.Collections.Generic;
using GildedRoseKata;
using System.Linq;
using NFluent;
using System;

namespace GildedRoseTests
{

    static class GildedRoseExtensions
    {
        public static void nextday(this GildedRose app)
            => app.UpdateQuality();
        public static List<int> quality(this List<Item> items)
            => items.Select(item => item.Quality).ToList();
        public static List<int> sellin(this List<Item> items)
            => items.Select(item => item.SellIn).ToList();
    }


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

        readonly int FarFromExpiring = 1000;
        Func<int, int> lower = value => value - 1;

        [Fact]
        public void Next_day_both_SellIn_and_Quality_lower_for_every_item()
        {
            //given
            var foo = new Item { Name = "foo", SellIn = FarFromExpiring, Quality = 10 };
            var bar = new Item { Name = "bar", SellIn = FarFromExpiring, Quality = 20 };
            var stock = new List<Item> { foo, bar };

            //when
            var expected_sellin_nextday = stock.sellin().Select(lower);
            var expected_quality_nextday = stock.quality().Select(lower);
            Check.That(expected_quality_nextday).IsEquivalentTo(new[] { 9, 19 });
            GildedRose app = new GildedRose(stock);
            app.nextday();

            //then
            Check.That(stock.sellin()).Equals(expected_sellin_nextday);
            Check.That(stock.quality()).Equals(expected_quality_nextday);
        }


        readonly int DateHasPassed = 0;
        Func<int, int> degradesTwiceAsFast = value => value - 2;

        [Fact]
        public void Once_the_sell_by_date_has_passed__Quality_degrades_twice_as_fast()
        {
            //given
            var foo = new Item { Name = "foo", SellIn = DateHasPassed, Quality = 10 };
            var bar = new Item { Name = "bar", SellIn = DateHasPassed, Quality = 20 };
            var stock = new List<Item> { foo, bar };

            //when
            var expected_quality_nextday = stock.quality().Select(degradesTwiceAsFast);
            GildedRose app = new GildedRose(stock);
            app.nextday();

            //then
            Check.That(stock.quality()).Equals(expected_quality_nextday);
        }

        Func<Item, bool> qualityIsNotNegative = item => item.Quality >= 0;
        Func<bool, bool> not = condition => !condition;

        [Fact]
        public void The_Quality_of_an_item_is_never_negative__simple_case()
        {
            //given
            var foo = new Item { Name = "foo", SellIn = FarFromExpiring, Quality = 0 };
            var stock = new List<Item> { foo };

            //when
            GildedRose app = new GildedRose(stock);
            app.nextday();

            //then
            Check.That(stock.All(qualityIsNotNegative)).IsTrue();
        }

        [Fact]
        public void The_Quality_of_an_item_is_never_negative__complex_case()
        {
            //given
            var trial = TestFactory.getTestTrial();

            //when
            trial.runDays(30);

            //then
            Check.That(trial.stock.All(qualityIsNotNegative)).IsTrue();
        }

        /*
      TODO write tests for the below requirements
      
      
      Aged_Brie_actually_increases_in_Quality_the_older_it_gets
      The_Quality_of_an_item_is_never_more_than_50
      Sulfuras_never has to be sold or decreases in Quality
      "Backstage passes", like aged brie, increases in Quality as its SellIn value approaches;
      Quality increases by 2 when there are 10 days or less and by 3 when there are 5 days or less but
      Quality drops to 0 after the concert
      "Conjured" items degrade in Quality twice as fast as normal items
      an item can never have its Quality increase above 50, however "Sulfuras" is a legendary item and as such its Quality is 80 and it never alters.
         */


    }
}

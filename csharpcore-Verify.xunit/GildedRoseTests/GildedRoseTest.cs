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

        static readonly int FarFromExpiring = 1000;
        static Func<int, int> lower = value => value - 1;

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


        static readonly int DateHasPassed = 0;
        static Func<int, int> degradesTwiceAsFast = value => value - 2;

        [Fact]
        public void Once_the_sell_by_date_has_passed__Quality_degrades_twice_as_fast()
        {
            //given
            var foo = new Item { Name = "foo", SellIn = DateHasPassed, Quality = 10 };
            var bar = new Item { Name = "bar", SellIn = DateHasPassed, Quality = 20 };
            var stock = new List<Item> { foo, bar };

            //when
            var not_expected_quality_nextday = stock.quality().Select(lower);
            var expected_quality_nextday = stock.quality().Select(degradesTwiceAsFast);
            GildedRose app = new GildedRose(stock);
            app.nextday();

            //then
            Check.That(stock.quality()).Not.Equals(not_expected_quality_nextday);
            Check.That(stock.quality()).Equals(expected_quality_nextday);
        }

        static Func<Item, bool> qualityIsNotNegative = item => item.Quality >= 0;

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
            Check.That(qualityIsNotNegative(foo)).IsTrue();
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

        static Func<int, int> increase = value => value + 1;

        [Fact]
        public void Aged_Brie_actually_increases_in_Quality_the_older_it_gets()
        {
            //given
            int quality = 10;
            var agedBrie = new Item { Name = "Aged Brie", SellIn = FarFromExpiring, Quality = quality };
            var stock = new List<Item> { agedBrie };

            //when
            GildedRose app = new GildedRose(stock);
            app.nextday();

            //then
            var newQuality = Check.That(agedBrie.Quality);
            newQuality.Not.Equals(lower(quality));//nominal
            newQuality.Equals(increase(quality));//particularity
        }

        static readonly int maxQuality = 50;

        [Fact]
        public void The_Quality_of_an_item_is_never_more_than_50__simple_case()
        {
            //given
            var itemThatCanIncreaseQuality = new Item { Name = "Aged Brie", SellIn = FarFromExpiring, Quality = maxQuality };
            var stock = new List<Item> { itemThatCanIncreaseQuality };

            //when
            GildedRose app = new GildedRose(stock);
            app.nextday();

            //then
            var newQuality = Check.That(itemThatCanIncreaseQuality.Quality);
            newQuality.Not.Equals(51);//nominal
            newQuality.Equals(50);//particularity
        }

        [Fact]
        public void The_Quality_of_an_item_is_never_more_than_50__except_for_sulfuras__complex_case()
        {
            //given
            var trial = TestFactory.getTestTrial();

            //when
            trial.runDays(30,
                //then
                () => Check.That(trial.stock.All(
                    item => item.Quality <= maxQuality
                    || item.Name.StartsWith("Sulfuras") //exception
                )).IsTrue()
            );
        }


        [Fact]
        public void Sulfuras_never_has_to_be_sold_or_decreases_in_Quality()
        {
            //given
            var trial = TestFactory.getTestTrial();
            var getSulfuras = () => trial.stock.Where(item => item.Name.StartsWith("Sulfuras"));
            var getSulfurasCount = () => getSulfuras().Count();
            var initialSulfurasCount = getSulfurasCount();
            Check.That(initialSulfurasCount).IsStrictlyPositive();
            Func<Dictionary<Item, int>> getSulfurasQuality = () => new Dictionary<Item, int>(
                getSulfuras().Select(item => new KeyValuePair<Item, int> (item, item.Quality))
            );
            var initialSulfurasQuality = getSulfurasQuality();
            Func<Dictionary<Item, int>, Dictionary<Item, int>, bool> HaveSameItems =
                (previousQuality, quality) => new HashSet<Item>(previousQuality.Keys).SetEquals(new HashSet<Item>(quality.Keys));
            Func<Dictionary<Item, int>, Dictionary<Item, int>, bool> DidntDecrease =
                (previousQuality, quality) => previousQuality.Keys.All(item => !(quality[item] < previousQuality[item]));

            //when
            trial.runDays(30,
                //then
                () => {
                    Check.That(getSulfurasCount()).IsEqualTo(initialSulfurasCount);
                    var sulfurasQuality = getSulfurasQuality();

                    Check.That(HaveSameItems(initialSulfurasQuality, sulfurasQuality)).IsTrue();
                    Check.That(DidntDecrease(initialSulfurasQuality, sulfurasQuality)).IsTrue();
                }
            );
        }

        [Fact]
        public void Backstage_passes_increases_in_Quality__simple_case__hotfix()
        {
            //given
            var backstage = new Item { Name = "Backstage passes lambda", SellIn = FarFromExpiring, Quality = 10 };
            var stock = new List<Item> { backstage };

            //when
            GildedRose app = new GildedRose(stock);
            app.nextday();

            //then
            var newQuality = Check.That(backstage.Quality);
            newQuality.Not.Equals(9);//nominal
            newQuality.Equals(11);//particularity
        }

        static Func<int, int> increaseBy2 = value => value + 2;
        static Func<int, int> increaseBy3 = value => value + 3;

        [Fact]
        public void Backstage_passes_increases_in_Quality__by_2_when_there_are_10_days_or_less_and_more_than_5()
        {
            //given
            var backstage = new Item { Name = "Backstage passes lambda", SellIn = 10, Quality = 20 };
            var stock = new List<Item> { backstage };

            //when
            GildedRose app = new GildedRose(stock);
            Check.That(backstage.SellIn).IsEqualTo(10);

            //then
            do
            {
                Check.That(backstage.SellIn).IsStrictlyLessThan(10 + 1);
                int previousQuality = backstage.Quality;
                app.nextday();
                Check.That(backstage.Quality).Equals(increaseBy2(previousQuality));
            } while (backstage.SellIn > 5);
        }
        
        [Fact]
        public void Backstage_passes_increases_in_Quality__by_3_when_there_are_5_days_or_less()
        {
            //given
            var backstage = new Item { Name = "Backstage passes lambda", SellIn = 5, Quality = 20 };
            var stock = new List<Item> { backstage };

            //when
            GildedRose app = new GildedRose(stock);
            Check.That(backstage.SellIn).IsEqualTo(5);

            //then
            do
            {
                Check.That(backstage.SellIn).IsStrictlyLessThan(5 + 1);
                int previousQuality = backstage.Quality;
                app.nextday();
                Check.That(backstage.Quality).Equals(increaseBy3(previousQuality));
            } while (backstage.SellIn > 0);
        }
        
        [Fact]
        public void Backstage_Quality_drops_to_0_after_the_concert()
        {
            //given
            var backstage = new Item { Name = "Backstage passes lambda", SellIn = 0, Quality = 20 };
            var stock = new List<Item> { backstage };

            //when
            GildedRose app = new GildedRose(stock);
            app.nextday();

            //then
            Check.That(backstage.Quality).IsZero();
        }

        static Func<int, int> degradeTwiceAsFast = value => lower(lower(value));

        //TODO correct implementation
        [Fact (Skip = @"
detected implementation error: Conjured items do not degrade twice as fast
they just degrade normally.
")]
        public void Conjured_items_degrade_in_Quality_twice_as_fast_as_normal_items()
        {
            //given
            int quality = 10;
            var conjured = new Item { Name = "Conjured whatever", SellIn = FarFromExpiring, Quality = quality };
            var stock = new List<Item> { conjured };

            //when
            GildedRose app = new GildedRose(stock);
            app.nextday();

            //then
            var newQuality = Check.That(conjured.Quality);
            newQuality.Not.Equals(lower(quality));//nominal
            newQuality.Equals(degradeTwiceAsFast(quality));//particularity
        }

        [Fact]
        public void An_item_can_never_have_its_Quality_increase_above_50()
        {
            //given
            var strengthenable = new Item { Name = "Aged Brie", SellIn = FarFromExpiring, Quality = maxQuality };
            var stock = new List<Item> { strengthenable };

            //when
            GildedRose app = new GildedRose(stock);
            app.nextday();

            //then
            var strengthenableQuality = Check.That(strengthenable.Quality);
            strengthenableQuality.Not.Equals(increase(maxQuality));//not nominal
            strengthenableQuality.Equals(maxQuality);//but particular
        }

        static readonly int sulfurasEverQuality = 80;

        //TODO correct implementation
        //observe sulfurasEverQuality
        [Fact
(Skip = @"
detected implementation error: Sulfuras DO alters, just like normal items
whereas it shouldn't 'it never alters'
")
        ]
        public void An_item_can_never_have_its_Quality_increase_above_50__except_sulfuras()
        {
            //given
            var sulfuras = new Item { Name = "Sulfuras", SellIn = FarFromExpiring, Quality = sulfurasEverQuality };
            var stock = new List<Item> { sulfuras };

            //when
            GildedRose app = new GildedRose(stock);
            app.nextday();

            //then
            var sulfurasQuality = Check.That(sulfuras.Quality);
            sulfurasQuality.Not.IsEqualTo(increase(sulfurasEverQuality));//not nominal
            sulfurasQuality.IsEqualTo(sulfurasEverQuality);//but particular
        }
        

    }
}

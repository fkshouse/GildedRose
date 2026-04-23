using GildedRoseKata;
using System.Collections.Generic;
using Xunit;

namespace GildedRoseTests;

public class GildedRoseTests
{
    Database db = new Database();

    Dictionary<int, Item> Items = new Dictionary<int, Item>
        {
            { 1, new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20} },
            { 2, new Item {Name = "Aged Brie", SellIn = 2, Quality = 0} },
            { 3, new Item { Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7}},
            { 4, new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80}},
            { 5, new Item { Name = "Sulfuras, Hand of Ragnaros", SellIn = -1, Quality = 80} },
            { 6, new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 15, Quality = 20 }},
            { 7, new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 10, Quality = 49 }},
            { 8, new Item { Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 5, Quality = 49 }},
            { 9, new Item { Name = "Conjured Mana Cake", SellIn = 3, Quality = 6 }}
        };

    [Fact]
    public void UpdateQuality_OneDay_Pass()
    {
        //Expected values have been populated based on these criterias:
        //- Once the sell by date has passed, Quality degrades twice as fast
        //- The Quality of an item is never negative
        //- "Aged Brie" actually increases in Quality the older it gets
        //- The Quality of an item is never more than 40
        //- "Sulfuras", being a legendary item, never has to be sold or decreases in Quality
        //- "Backstage passes", like aged brie, increases in Quality as its SellIn value approaches; Quality increases by 3 when there are 7 days or less and by 4 when there are 2 days or less but
        //- Quality drops to 0 after the concert
        //- "Conjured" items degrade in Quality twice as fast as normal items

        IList<Item> ExpectedAfter1Day = new List<Item>
        {
            new Item {Name = "+5 Dexterity Vest", SellIn = 9, Quality = 19},
            new Item {Name = "Aged Brie", SellIn = 1, Quality = 1},
            new Item {Name = "Elixir of the Mongoose", SellIn = 4, Quality = 6},
            new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 40},
            new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = -1, Quality = 40},
            new Item {Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 14, Quality = 21},
            new Item {Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 9, Quality = 40},
            new Item {Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 4, Quality = 40},
            new Item {Name = "Conjured Mana Cake", SellIn = 2, Quality = 4}
        };

        db.ResetAllOriginalValues();
        GildedRose app = new GildedRose(Items, db);
        app.UpdateQuality();

        foreach (var item in Items)
        {
            Assert.Equal(ExpectedAfter1Day[item.Key-1].SellIn, item.Value.SellIn);
            Assert.Equal(ExpectedAfter1Day[item.Key - 1].Quality, item.Value.Quality);
        }
    }

    [Fact]
    public void UpdateQuality_TenDays_Pass()
    {
        IList<Item> ExpectedAfter10Days = new List<Item>
        {
            new Item {Name = "+5 Dexterity Vest", SellIn = 0, Quality = 10},
            new Item {Name = "Aged Brie", SellIn = -8, Quality = 18},
            new Item {Name = "Elixir of the Mongoose", SellIn = -5, Quality = 0},
            new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 40},
            new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = -1, Quality = 40},
            new Item {Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 5, Quality = 34},
            new Item {Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = 0, Quality = 40},
            new Item {Name = "Backstage passes to a TAFKAL80ETC concert", SellIn = -5, Quality = 0},
            new Item {Name = "Conjured Mana Cake", SellIn = -7, Quality = 0}
        };

        db.ResetAllOriginalValues();
        GildedRose app = new GildedRose(Items, db);
        for (var i = 0; i < 10; i++)
        {
            app.UpdateQuality();
        }

        foreach (var item in Items)
        {
            Assert.Equal(ExpectedAfter10Days[item.Key - 1].SellIn, item.Value.SellIn);
            Assert.Equal(ExpectedAfter10Days[item.Key - 1].Quality, item.Value.Quality);
        }
    }
}
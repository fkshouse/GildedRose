using GildedRoseKata;
using System.Collections.Generic;
using Xunit;

namespace GildedRoseTests
{
    public class DatabaseTests
    {
        Database db = new Database();
        
        [Fact]
        public void GetitemById_Success()
        {
            db.ResetAllOriginalValues();
            Item item = db.GetItemById(2);

            Assert.Equal(2, item.SellIn);
            Assert.Equal(0, item.Quality);
        }

        [Fact]
        public void Update_Success()
        {
            db.ResetAllOriginalValues();
            db.Update(2, new Item { Name = "Aged Brie", SellIn = -5, Quality = 17 });
            Item item = db.GetItemById(2);

            Assert.Equal(-5, item.SellIn);
            Assert.Equal(17, item.Quality);
        }

        [Fact]
        public void Update_Fail_ItemNotExist()
        {
            db.ResetAllOriginalValues();
            db.Update(99, new Item { Name = "Caged Brie", SellIn = -5, Quality = 17 });
            Item item = db.GetItemById(99);

            Assert.NotEqual(-5, item.SellIn);
            Assert.NotEqual(17, item.Quality);
        }

        [Fact]
        public void GetQualityChangeRate_Success()
        {
            db.ResetAllOriginalValues();
            string value = db.GetQualityChangeRate(1);

            Assert.Equal("P|MINUS1|N|MINUS1", value);
        }

        [Fact]
        public void GetQualityChangeRate_Fail_DoesNotExist()
        {
            db.ResetAllOriginalValues();
            string value = db.GetQualityChangeRate(99);

            Assert.NotEqual("P|MINUS1|N|MINUS1", value);
        }
    }
}

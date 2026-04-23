using System.Collections.Generic;

namespace GildedRoseKata;

public class GildedRose
{
    Dictionary<int, Item> Items;
    Database _Db;
    int _maxQuality = 40;
    int _minQuality = 0;
    const int _exponentialIncreaseThreshold1 = 7;
    const int _exponentialIncreaseThreshold2 = 2;
    int _changeFactor = 1;

    public GildedRose(Dictionary<int, Item> Items, Database Db)
    {
        this.Items = Items;
        this._Db = Db;
    }

    public void UpdateQuality()
    {
        foreach (var item in Items)
        {
            int id = item.Key;
            Item itm = item.Value;
            

            if (itm.Name != "Sulfuras, Hand of Ragnaros")
            {
                ResetChangeFactor();
                ReduceSellIn(itm);
                string changerate = _Db.GetQualityChangeRate(id);
                //Trying both methods here to show using db values instead of harcoded values for 2 cases and hardcoded values for the rest
                if (!string.IsNullOrEmpty(changerate))
                {                    
                    ApplyQualityChangeRate(changerate, itm);
                }
                else
                {
                    if (itm.Name == "Backstage passes to a TAFKAL80ETC concert") // OR item.Name.StartsWith("Backstage passes") for multiple and different backstage passes items
                        IncreaseQuality(itm, true);
                    else if (itm.Name == "Aged Brie")
                        IncreaseQuality(itm);
                    else
                        ReduceQuality(itm);
                }
            }
            //Normally I would place this inside the if above
            //But I will place this outside the if because one of the conditions is that quality is never over 40
            //So if an item in the list has an original value above 40 or is also an item whose value never changes
            //Then in that case it will at least make sure that it is set to the maximum allowed
            AdjustMinMaxQuality(itm);

            _Db.Update(id, itm);
        }
    }

    private void ApplyQualityChangeRate(string changerate, Item item)
    {
        string[] rates = changerate.Split('|');
        bool isforpostivesellin = true;
        int positiverate = 0, negativerate = 0;

        foreach (string rate in rates)
        {
            if (rate == "P")
            {
                isforpostivesellin = true;
            }
            else if (rate == "N")
            {
                isforpostivesellin = false;
            }
            else
            {
                if (isforpostivesellin) positiverate = GetQualityChangeValue(rate);
                else negativerate = GetQualityChangeValue(rate);
            }
        }

        item.Quality += _changeFactor * (item.SellIn >= 0 ? positiverate : negativerate);
    }

    private int GetQualityChangeValue(string rate)
    {
        int value = 0;
        //The rest of the conditions checks can be added here
        //for example after sellin date quality value is 0 so value would be 0
        //or quality reduces twice as fast so value would be 2
        //or increase it by 3 when 7 days left for sellin 
        switch(rate)
        {
            case "MINUS1":
                value = -1;
                break;
            case "PLUS1":
                value = 1;
                break;
        }
        return value;
    }

    private void AdjustMinMaxQuality(Item item)
    {
        if (item.Quality < _minQuality) item.Quality = _minQuality;
        if (item.Quality > _maxQuality) item.Quality = _maxQuality;
    }

    private void ResetChangeFactor()
    {
        _changeFactor = 1;
    }


    private void ReduceSellIn(Item item)
    {
        item.SellIn--;
        if (item.SellIn < 0)
        {
            _changeFactor = 2;
        }
        if (item.Name == "Conjured Mana Cake") // OR item.Name.StartsWith("Conjured") for multiple and different conjured items
        {
            //"Degrades twice as fast as normal items" - Here I am assuming that normal items means non conjured
            //So multiplying it by 2 so that when the sell by date is over 0 it removes twice the normal value for quality (1 x 2) and when sell by date is in the negative it doubles the already doubled value to remove for quality (1 x 2 x 2) 
            _changeFactor *= 2;
        }
    }

    private void ReduceQuality(Item item)
    {
        if (item.Quality > _minQuality)
            item.Quality -= _changeFactor;
    }

    private void IncreaseQuality(Item item, bool concertIncrease = false)
    {
        if (item.Quality <= _maxQuality)
        {
            item.Quality += _changeFactor;
            if (concertIncrease)
            {
                if (item.SellIn < 0)
                    item.Quality = 0;
                else
                {
                    if (item.SellIn < _exponentialIncreaseThreshold1)
                    {
                        item.Quality += 2;

                        if (item.SellIn < _exponentialIncreaseThreshold2)
                            item.Quality++;
                    }
                }
            }
        }
    }

    //public void UpdateQuality()
    //{
    //    for (var i = 0; i < Items.Count; i++)
    //    {
    //        if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
    //        {
    //            Items[i].SellIn = Items[i].SellIn - 1;
    //        }

    //        if (Items[i].Name != "Aged Brie" && Items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
    //        {
    //            if (Items[i].Quality > 0)
    //            {
    //                if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
    //                {
    //                    Items[i].Quality = Items[i].Quality - 1;
    //                }
    //            }
    //        }
    //        else
    //        {
    //            if (Items[i].Quality < 50)
    //            {
    //                Items[i].Quality = Items[i].Quality + 1;

    //                if (Items[i].Name == "Backstage passes to a TAFKAL80ETC concert")
    //                {
    //                    if (Items[i].SellIn < 11)
    //                    {
    //                        if (Items[i].Quality < 50)
    //                        {
    //                            Items[i].Quality = Items[i].Quality + 1;
    //                        }
    //                    }

    //                    if (Items[i].SellIn < 6)
    //                    {
    //                        if (Items[i].Quality < 50)
    //                        {
    //                            Items[i].Quality = Items[i].Quality + 1;
    //                        }
    //                    }
    //                }
    //            }
    //        }

            

    //        if (Items[i].SellIn < 0)
    //        {
    //            if (Items[i].Name != "Aged Brie")
    //            {
    //                if (Items[i].Name != "Backstage passes to a TAFKAL80ETC concert")
    //                {
    //                    if (Items[i].Quality > 0)
    //                    {
    //                        if (Items[i].Name != "Sulfuras, Hand of Ragnaros")
    //                        {
    //                            Items[i].Quality = Items[i].Quality - 1;
    //                        }
    //                    }
    //                }
    //                else
    //                {
    //                    Items[i].Quality = Items[i].Quality - Items[i].Quality;
    //                }
    //            }
    //            else
    //            {
    //                if (Items[i].Quality < 50)
    //                {
    //                    Items[i].Quality = Items[i].Quality + 1;
    //                }
    //            }
    //        }
    //    }
    //}
}
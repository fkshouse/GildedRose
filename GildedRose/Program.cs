using System;
using System.Collections.Generic;

namespace GildedRoseKata;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("OMGHAI!");

        Database db = new Database();
        db.ResetAllOriginalValues();
        Dictionary<int, Item> items = db.GetAllItems();

        var app = new GildedRose(items, db);

        int days = 2;
        if (args.Length > 0)
        {
            days = int.Parse(args[0]) + 1;
        }

        for (var i = 0; i < days; i++)
        {
            Console.WriteLine("-------- day " + i + " --------");
            Console.WriteLine("name, sellIn, quality");
            foreach (var item in items)
            {
                Console.WriteLine(item.Value.Name + ", " + item.Value.SellIn + ", " + item.Value.Quality);
            }
            
            Console.WriteLine("");
            app.UpdateQuality();
        }
    }
}
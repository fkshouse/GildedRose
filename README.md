Decisions and assumptions made from the instructions

I'm not sure if those were mistakes made when writing up the instructions or if they were made on purpose for this exercise, but:

1. Instructions say that quality for concert increases by 3 when 7 days or less or less and by 4 when 2 days or less but code increased it by 2 when 10 days or less and by 3 when 5 days or less.
	I decided to go with what the instructions were saying and modified the conditions in the code to increase by 3 for 7 days and by 4 when 2 days.
	I have also put this in a global variable so if it needs to be changed it can be done in 1 place instead of modifiyng it multiple times in the code.

2. Instructions say that quality is never more than 40 but code has 50 as the limit. So I changed the code to set it at 40 as per the instructions.
	I have also put this in a global variable so if it needs to be changed it can be done in 1 place instead of modifiyng it multiple times in the code.

3. Sell In value is reduced after updating concert quality values, I ahve put this in the beginning so that the updated quality values reflect the correct sell in days.

4. The quality of an item is never more than 40 but the quality value for Sulfuras is set to 80. Going off on the assumption that quality can never be over 40 even for manually entered items in the list, i will set the quality values to the maximum of 40 for any item that has a quality value bigger than 40.



Code explanations and decisions made

I refactored the code and made it less coupled and removed all the nested if statements.

After refactoring the code, I decided to remove the dependency to hardcoded lists. As per the task; a new item needed to be added. Updating the code every time a new item needs to be added is not feasible for the future. Currently it has 9 items and seems difficult to maintain, so what would happen when they have 100 items.
The code will not be maintanable. 

Along with that I decided to introduce a simple automated mechanism where the change for the quality value is stored in the db and we just need to retrieve that to update the quality value; instead of harcoding the change value. This is for the same reason as mentioned before,
currently there are only a few items, but if the item count goes higher it would become unmanageable.

The database changes I introduced are basic level changes to show how it would work without hardcoded values and checks. I went with SQLite as it is easy to implement and doesnt require major changes to the structure.
I could also use EF Core for cleaner better and safer data access and for a proper repository pattern usage but it would require changes to the Item class,
and since changes to it were prohibited in the instructions, I opted for this method as it allows the use of db without changing the Item class

Currently the min/max values for quality and quality value change depending on the remaining sellin days for concert tickets are hardcoded as global variables, but the ApplyQualityChangeRate method shows how this could be replaced.
The goal was to remove dependency on hardcoded values and do it the proper way by getting that info from the db and then applying it accordingly.

I added a couple of unit tests to tests basic situations, this could be expanded to include more tests with various different scenarios and data.


Possible Future Improvements

Option to add Items and Quality Change rates properly
Using EF properly to structure DB access
Entirely remove hardcoded values (items and quality change checks)
As the list of Items could grow bigger, create a an alert or something to notify the user of items approaching their sell in days
Many more features could be added that are part of a normal inventory manangement system thus allowing us to use proper coding patterns
More Unit tests to test all possible scenarios



Setup Requirements

Installing SQLite on Windows
Download: Go to the SQLite Download Page and download the sqlite-tools-win-x64-xxxxxxx.zip bundle.
Extract: Extract the downloaded ZIP file to a dedicated folder, such as C:\sqlite.
Add to PATH:
Search for "Environment Variables" in the Windows search bar and select "Edit the system environment variables".
Click "Environment Variables," then under "System variables," find and select "Path" and click "Edit".
Click "New" and add the path to your C:\sqlite folder.
Verify: Open Command Prompt, type sqlite3, and press Enter. If successful, you will see the SQLite version information
Create a folder called database in the C drive and open a new Command Prompt.
In the command prompt, navigate to the newly created database folder on the C drive with cd C:\database
and type the following command to create the database and press enter: sqlite3 guildedrosedb
Verify: Type .database and press enter. If the database was succesfully created you should see the following line: main: C:\database\guildedrosedb r/w




































# Gilded Rose starting position in C# xUnit

## Build the project

Use your normal build tools to build the projects in Debug mode.
For example, you can use the `dotnet` command line tool:

``` cmd
dotnet build GildedRose.sln -c Debug
```

## Run the Gilded Rose Command-Line program

For e.g. 10 days:

``` cmd
GildedRose/bin/Debug/net8.0/GildedRose 10
```

## Run all the unit tests

``` cmd
dotnet test
```
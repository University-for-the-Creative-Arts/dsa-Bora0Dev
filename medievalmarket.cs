using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        // Create a new instance of the Game and start it
        Game game = new Game();
        game.Run();
    }
}

class Game
{
    // Represents a tradeable item, such as Grain or Wool
    class Good
    {
        public string Name;
        public Good(string name) { Name = name; }
    }

    // Represents a town with a set of prices for goods
    class Town
    {
        public string Name;
        public Dictionary<string, int> Prices;

        public Town(string name, Dictionary<string, int> prices)
        {
            Name = name;
            Prices = prices;
        }
    }

    // List of all available goods in the world
    List<Good> Goods = new List<Good>()
    {
        new Good("Grain"),
        new Good("Wool"),
        new Good("Iron")
    };

    // Towns and roads (a simple graph structure)
    Dictionary<string, Town> Towns;
    // Roads connect towns and include toll costs to travel
    Dictionary<string, List<Tuple<string, int>>> Roads;

    // Player state: current town, gold, and inventory of goods
    string cur = "Eldham";
    int gold = 30;
    Dictionary<string, int> inv = new Dictionary<string, int>();

    // Constructor: set up the world and player inventory
    public Game()
    {
        // Initialize the three towns with their own prices
        Towns = new Dictionary<string, Town>()
        {
            {"Eldham", new Town("Eldham", new Dictionary<string,int>{{"Grain",5},{"Wool",8},{"Iron",12}})},
            {"Brackenridge", new Town("Brackenridge", new Dictionary<string,int>{{"Grain",7},{"Wool",6},{"Iron",14}})},
            {"Stoneford", new Town("Stoneford", new Dictionary<string,int>{{"Grain",4},{"Wool",10},{"Iron",9}})}
        };

        // Define roads (edges in the graph) with toll prices
        Roads = new Dictionary<string, List<Tuple<string, int>>>()
        {
            {"Eldham", new List<Tuple<string,int>>{ Tuple.Create("Brackenridge",2), Tuple.Create("Stoneford",4) }},
            {"Brackenridge", new List<Tuple<string,int>>{ Tuple.Create("Eldham",2), Tuple.Create("Stoneford",3) }},
            {"Stoneford", new List<Tuple<string,int>>{ Tuple.Create("Eldham",4), Tuple.Create("Brackenridge",3) }}
        };

        // Start player with zero of each good
        foreach (Good g in Goods) inv[g.Name] = 0;
    }

    // Main game loop: reads player input and executes commands
    public void Run()
    {
        WriteTitle(); // Display title and initial market info
        Help(); // Show available commands

        // Infinite loop until player types 'quit'
        while (true)
        {
            Console.Write("\n> ");
            string input = (Console.ReadLine() ?? "").Trim();
            if (input == "") continue; // Ignore empty input

            // Split input into command and arguments
            string[] parts = input.Split(' ');
            string cmd = parts[0].ToLower();

            try
            {
                // Match the command typed by the player
                if (cmd == "help") Help();
                else if (cmd == "status") Status();
                else if (cmd == "market") Market();
                else if (cmd == "buy") Buy(parts);
                else if (cmd == "sell") Sell(parts);
                else if (cmd == "travel") Travel(parts);
                else if (cmd == "quit") return; // Exit game
                else Console.WriteLine("Unknown command. Type 'help' for options.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    // Display the game title, starting gold, and current location
    void WriteTitle()
    {
        Console.WriteLine("==== Medieval Market (Mini) ====");
        Console.WriteLine("You are a trader moving between medieval towns.");
        Console.WriteLine("You start in " + cur + " with " + gold + " gold.");
        ShowRoads(); // Show roads from current town
        Market(); // Show local market
    }

    // Print list of commands
    void Help()
    {
        Console.WriteLine("help | status | market | buy <qty> <good> | sell <qty> <good> | travel <town> | quit");
    }

    // Display player’s current gold and inventory contents
    void Status()
    {
        Console.WriteLine("Gold: " + gold);
        Console.WriteLine("Inventory:");
        foreach (Good g in Goods)
            Console.WriteLine("  " + g.Name + ": " + inv[g.Name]);
    }

    // Display the market (goods and prices) in the current town
    void Market()
    {
        Town t = Towns[cur];
        Console.WriteLine("Market in " + t.Name + ":");
        Console.WriteLine("  Good    Price");
        foreach (Good g in Goods)
            Console.WriteLine("  " + g.Name.PadRight(7) + t.Prices[g.Name].ToString().PadLeft(5));
    }

    // Show all roads leaving the current town and their tolls
    void ShowRoads()
    {
        Console.WriteLine("Roads:");
        foreach (Tuple<string, int> r in Roads[cur])
            Console.WriteLine("  -> " + r.Item1 + " (toll " + r.Item2 + ")");
    }

    // Handle buying goods from the current market
    void Buy(string[] p)
    {
        if (p.Length < 3) { Console.WriteLine("Usage: buy <qty> <good>"); return; }

        // Parse quantity argument
        int qty;
        if (!int.TryParse(p[1], out qty) || qty <= 0)
        {
            Console.WriteLine("Invalid quantity.");
            return;
        }

        // Get the good name (everything after the quantity)
        string good = Capitalize(string.Join(" ", p, 2, p.Length - 2));

        // Check if the good exists in this town
        if (!Towns[cur].Prices.ContainsKey(good))
        {
            Console.WriteLine("Unknown good.");
            return;
        }

        // Calculate cost and check player gold
        int price = Towns[cur].Prices[good] * qty;
        if (gold < price)
        {
            Console.WriteLine("Not enough gold.");
            return;
        }

        // Complete purchase
        gold -= price;
        inv[good] += qty;
        Console.WriteLine("Bought " + qty + " " + good + " for " + price + " gold.");
    }

    // Handle selling goods to the current market
    void Sell(string[] p)
    {
        if (p.Length < 3) { Console.WriteLine("Usage: sell <qty> <good>"); return; }

        // Parse quantity argument
        int qty;
        if (!int.TryParse(p[1], out qty) || qty <= 0)
        {
            Console.WriteLine("Invalid quantity.");
            return;
        }

        // Get the good name (everything after the quantity)
        string good = Capitalize(string.Join(" ", p, 2, p.Length - 2));

        // Check if player has enough of that good
        if (!inv.ContainsKey(good) || inv[good] < qty)
        {
            Console.WriteLine("Not enough goods.");
            return;
        }

        // Calculate sale revenue (player sells at 90% of price)
        int revenue = (int)(Towns[cur].Prices[good] * 0.9) * qty;

        // Complete sale
        inv[good] -= qty;
        gold += revenue;
        Console.WriteLine("Sold " + qty + " " + good + " for " + revenue + " gold.");
    }

    // Handle traveling to a neighboring town
    void Travel(string[] p)
    {
        if (p.Length < 2)
        {
            Console.WriteLine("Usage: travel <town>");
            return;
        }

        // Get destination name
        string to = string.Join(" ", p, 1, p.Length - 1);

        // Find the road that leads to the destination
        Tuple<string, int> edge = Roads[cur].Find(r => r.Item1.Equals(to, StringComparison.OrdinalIgnoreCase));

        // Check if that road exists
        if (edge == null)
        {
            Console.WriteLine("That town is not connected. Try 'roads'.");
            return;
        }

        // Check if player can afford the toll
        if (gold < edge.Item2)
        {
            Console.WriteLine("Need " + edge.Item2 + " gold for the toll.");
            return;
        }

        // Move player to the new town
        gold -= edge.Item2;
        cur = edge.Item1;
        Console.WriteLine("Arrived at " + cur + ". Gold left: " + gold);

        // Show new town info
        ShowRoads();
        Market();
    }

    // Capitalizes first letter of a word (for matching names)
    string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return char.ToUpper(s[0]) + s.Substring(1).ToLower();
    }
}

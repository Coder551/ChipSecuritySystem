using System;
using System.Collections.Generic;
using ChipSecuritySystem;

public class Program
{
    public static void Main()
    {
        Console.WriteLine("Please type true to use a random set of chips, or false to use the manual test set:");

        if (!bool.TryParse(Console.ReadLine(), out bool useRandom))
            {
                Console.WriteLine("Invalid input. Defaulting to manual test.");
                useRandom = false;
            }

        var chips = useRandom ? ChipSet() : GetManualTest();
        var chainResult = BestLinkChain(chips);

        Console.WriteLine("The Best Link Chain is:");

        foreach (var chip in chainResult)
        {
            Console.WriteLine($"[{chip.StartColor}, {chip.EndColor}]");
        }

        Console.WriteLine("The length of the Best Link Chain is: " + chainResult.Count);
    }

    /* Function returns a List of Color Chips for testing.
       Returns a List<ColorChip> */
    public static List<ColorChip> GetManualTest()
    {
        return new List<ColorChip>
        {
            new ColorChip(Color.Blue, Color.Red),
            new ColorChip(Color.Yellow, Color.Red),
            new ColorChip(Color.Red, Color.Purple),
            new ColorChip(Color.Purple, Color.Blue),
            new ColorChip(Color.Blue, Color.Green),
            new ColorChip(Color.Red, Color.Orange),
        };
    }

    /* Function returns a randomized List of Color Chips for testing.
       Returns a List<ColorChip> */
    public static List<ColorChip> ChipSet()
    {
        Random rand = new Random();
        Color[] colors = (Color[])Enum.GetValues(typeof(Color));

        int size = rand.Next(5, 25);
        List<ColorChip> colorChips = new List<ColorChip>(size);

        // Needed for Blue chip to start chain, and Green chip to end chain
        colorChips.Add(new ColorChip(Color.Blue, colors[rand.Next(colors.Length)]));
        colorChips.Add(new ColorChip(colors[rand.Next(colors.Length)], Color.Green));

        for (int i = 2; i < size; i++)
        {
            colorChips.Add(new ColorChip(
                colors[rand.Next(colors.Length)],
                colors[rand.Next(colors.Length)]
            ));
        }

        Console.WriteLine("The length of the Color Chip List is: " + colorChips.Count);
        return colorChips;
    }

    /* Function to find the best link of Color Chips.
       Returns a List<ColorChip> */
    public static List<ColorChip> BestLinkChain(List<ColorChip> chips)
    {
        List<ColorChip> longestlinkChain = new List<ColorChip>();
        bool[] usedChip = new bool[chips.Count];

        Dictionary<Color, List<int>> colorMap = new Dictionary<Color, List<int>>();

        for (int i = 0; i < chips.Count; i++)
        {
            // creates a bucket for color not yet processed
            if (!colorMap.ContainsKey(chips[i].StartColor))
            {
                colorMap[chips[i].StartColor] = new List<int>();   
            }

            // stores index of chip under starting color
            colorMap[chips[i].StartColor].Add(i);
        }

        // initial function call to find the longest valid link chain
        ExploreLinkChain(
            Color.Blue,
            chips,
            colorMap,
            usedChip,
            new List<ColorChip>(),
            longestlinkChain
        );

        return longestlinkChain;
    }

    /* Recursive function that finds the 
       Longest Valid Link Chain from Blue -> Green */
    private static void ExploreLinkChain(
        Color currentColor,
        List<ColorChip> chips,
        Dictionary<Color, List<int>> colorMap,
        bool[] usedChip,
        List<ColorChip> currentLinkChain,
        List<ColorChip> longestLinkChain)
    {
        // check to see if reached valid chain
        if (currentColor == Color.Green)
        {
            // check to see if current link chain is longer, and if so add to longest link chain after clearing
            if (currentLinkChain.Count > longestLinkChain.Count)
            {
                longestLinkChain.Clear();
                longestLinkChain.AddRange(currentLinkChain);
            }
        }

        // stop exploring if no chips start from this color
        if (!colorMap.ContainsKey(currentColor))
            return;

        // look at all chips that start with the current color
        foreach (int i in colorMap[currentColor])
        {
            // skip if already used in current path 
            if (usedChip[i]) continue;

            usedChip[i] = true;
            currentLinkChain.Add(chips[i]);

            // recursive function call to continue finding the longest link chain
            ExploreLinkChain(chips[i].EndColor, chips, colorMap, usedChip, currentLinkChain, longestLinkChain);

            currentLinkChain.RemoveAt(currentLinkChain.Count - 1);
            usedChip[i] = false;
        }
    }
}
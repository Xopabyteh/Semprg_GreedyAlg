using System.Diagnostics;

var availableJewels = Jewel.GenerateRandomJewelsFrom(1_000, 13337, Jewel.StaticJewelTypes);
//var availableJewels = new Jewel[] {
//    new(3, 60),
//    new(4, 100),
//    new(2, 120)
//};

var sw = new Stopwatch();

var bag2 = new Bag(1000);
sw.Restart();
IRobber robber2 = new GreedyRobber();
robber2.FillBagWithJewels(bag2, availableJewels);
sw.Stop();

Console.WriteLine($"It took the greedy robber {sw.Elapsed}");
bag2.PrintContents();


//var bag1 = new Bag(100);
//sw.Start();
//IRobber robber1 = new BruteRobber();
//robber1.FillBagWithJewels(bag1, availableJewels);
//sw.Stop();

//Console.WriteLine($"It took the robber {sw.Elapsed}");
//bag1.PrintContents();

var bag3 = new Bag(1000);
sw.Restart();
IRobber robber3 = new DynamicRobber();
robber3.FillBagWithJewels(bag3, availableJewels);
sw.Stop();

Console.WriteLine($"It took the dynamic robber {sw.Elapsed}");
bag3.PrintContents();


public class DynamicRobber : IRobber
{
    public void FillBagWithJewels(Bag bag, Jewel[] availableJewels)
    {
        (int JI, int CI, double Value) biggestOptimum = (0, 0, 0d);

        // Create table with dimensions [nJewels + 1, volume + 1]
        //  Rows are jewels
        //  Columns are volumes
        var rowsL = availableJewels.Length + 1;
        var columnsL = bag.MaxVolume + 1;
        var priceVolumeTable = new double[rowsL, columnsL];

        // Fill table going through all jewels and all volumes:
        //  First row and column are 0, so fill from cI=1, jI=1
        //  Can jewel fit the volume?
        //  If yes:
        //      See sum of jewel + optimum of cell above and to the left (go to the left by jewels value)
        //      If sum > optimum of cell above, take sum, else take optimum from cell above (Take the bigger of the two)
        //  If no:
        //      Take optimum from cell above
        // Repeat until all cells are filled
        // cI is also the volume

        for (int jI = 1; jI < rowsL; jI++)
        for (int cI = 1; cI < columnsL; cI++)
        {
            var currentJewel = availableJewels[jI - 1]; // -1 because first row is all 0s, and we're counting from 1
            var canJewelFit = currentJewel.Volume <= cI;
            double pickedOptimum = 0;
            if(canJewelFit)
            {
                // Optimum of cell above and to the left
                var optimumWithLessVolumeAndJewel = priceVolumeTable[jI - 1, cI - currentJewel.Volume] + currentJewel.Price;
                // Optimum of cell above
                var optimumWithSameVolume = priceVolumeTable[jI - 1, cI];

                pickedOptimum = Math.Max(optimumWithLessVolumeAndJewel, optimumWithSameVolume);
            }
            else
            {
                // Optimum of cell above
                var optimumWithSameVolume = priceVolumeTable[jI - 1, cI];
                pickedOptimum = optimumWithSameVolume;
            }

            priceVolumeTable[jI, cI] = pickedOptimum;
            biggestOptimum = pickedOptimum > biggestOptimum.Value 
                    ? (jI, cI, pickedOptimum)
                    : biggestOptimum;
        }

        //PrintOptimumTable(priceVolumeTable);

        // Find the biggest optimum in the table (by keeping track of it while filling the table)
        // Backtrace to find the jewels picked by the bag:
            // If optimum is same as cell above, jewel was not picked: Go up
            // If optimum is not same as cell above, jewel was picked: Add jewel to list, go up and left by jewels volume
            // Repeat until volume is 0 or no more jewels
        var jewelsPicked = new List<Jewel>();
        (int JI, int CI) backtrackI = (biggestOptimum.JI, biggestOptimum.CI);
        
        while(true)
        {
            if(backtrackI.CI == 0)
                break;

            var currentOptimum = priceVolumeTable[backtrackI.JI, backtrackI.CI];
            var jewel = availableJewels[backtrackI.JI - 1]; // -1 again because first row is all 0s, and we're counting from 1
            // Optimum of cell above
            var optimumWithSameVolume = priceVolumeTable[backtrackI.JI - 1, backtrackI.CI];
            
            if(currentOptimum == optimumWithSameVolume)
            {
                // Same, go aboce
                backtrackI = (backtrackI.JI - 1, backtrackI.CI);
                continue;
            }

            // -> Not same, add and go up and left
            jewelsPicked.Add(jewel);
            backtrackI = (backtrackI.JI - 1, backtrackI.CI - jewel.Volume);
        }

        bag.AddJewels(jewelsPicked);
    }

    private void PrintOptimumTable(double[,] table)
    {
        var rowsL = table.GetLength(0);
        var columnsL = table.GetLength(1);

        // Print the table
        for (int i = 0; i < rowsL; i++)
        {
            for (int j = 0; j < columnsL; j++)
            {
                Console.Write($"{table[i, j],-10}");
            }
            Console.WriteLine();
        }
    }
}

public class GreedyRobber : IRobber
{
    /// <summary>
    /// Greedy algorithm which solves for ~largest price such
    /// that it fits in a bag of a given size.
    /// 
    /// For all jewels, pick the jewel with largest price that still
    /// fits into the bagg and add it to the bag
    /// End when No more jewels can fit
    /// </summary>
    public void FillBagWithJewels(Bag bag, Jewel[] availableJewels)
    {
        // Sort by price descending
        var jewelsSorted = availableJewels
            .Select(j => (
                Jewel: j,
                OptimumIndex: OptimumIndexOfJewel(j)
                )
            )
            .OrderByDescending(t => t.OptimumIndex);

        foreach (var jewelWithOpt in jewelsSorted)
        {
            if (!bag.DoesJewelFit(jewelWithOpt.Jewel))
                continue;
            
            bag.AddJewel(jewelWithOpt.Jewel);
        }

        // No more jewels can fit, end
    }

    public double OptimumIndexOfJewel(Jewel jewel)
        => jewel.Price / jewel.Volume;
}

public class BruteRobber : IRobber
{
    /// <summary>
    /// Go through all possible combinations of jewels
    /// and pick the best one
    /// </summary>
    public void FillBagWithJewels(Bag bag, Jewel[] availableJewels)
    {
        // Recursively:
            // Pick a jewel in next slot
            // If it fits, continue
            // If it doesn't fit, return
            // End when it's the last slot
            // End when no more jewels can fit

        var bestJewels = GetBestPriceBruteForce(availableJewels, 0, bag.MaxVolume, new(), 0);

        bag.AddJewels(bestJewels);
    }


    /// <summary>
    /// A recursive method to find the best combination of jewels based on their value
    /// </summary>
    /// <param name="availableJewels">The array of all available jewels</param>
    /// <param name="currentIndex">The index of the current jewel being considered</param>
    /// <param name="remainingVolume">The remaining capacity of the bag</param>
    /// <param name="currentJewels">The current list of jewels in the bag</param>
    /// <param name="currentValue">The current total value of the jewels in the bag</param>
    /// <returns>The best combination of jewels maximizing the value</returns>
    private List<Jewel> GetBestPriceBruteForce(ReadOnlySpan<Jewel> availableJewels, int currentIndex, double remainingVolume, List<Jewel> currentJewels, double currentValue)
    {
        // Base case: if we have no more capacity or no more jewels to consider
        if (currentIndex >= availableJewels.Length || remainingVolume <= 0)
        {
            return currentJewels;
        }

        // Current jewel
        var currentJewel = availableJewels[currentIndex];

        // Option 1: Skip the current jewel
        var bestCombination = GetBestPriceBruteForce(availableJewels, currentIndex + 1, remainingVolume, new List<Jewel>(currentJewels), currentValue);

        // Option 2: Take the current jewel, if it fits
        if (currentJewel.Volume <= remainingVolume)
        {
            currentJewels.Add(currentJewel);
            var newCombination = GetBestPriceBruteForce(availableJewels, currentIndex + 1, remainingVolume - currentJewel.Volume, new List<Jewel>(currentJewels), currentValue + currentJewel.Price);

            // Compare the value of the two combinations and take the better on
            if (GetTotalPrice(newCombination) > GetTotalPrice(bestCombination))
            {
                bestCombination = newCombination;
            }
        }

        return bestCombination;
    }

    private double GetTotalPrice(List<Jewel> jewels)
        => jewels.Sum(j => j.Price);
}

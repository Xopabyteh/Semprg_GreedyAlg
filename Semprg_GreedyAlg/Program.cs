using System.Diagnostics;

var availableJewels = Jewel.GenerateRandomJewelsFrom(100
    , 133337, Jewel.StaticJewelTypes);
var sw = new Stopwatch();

var bag2 = new Bag(100);
sw.Restart();
IRobber robber2 = new GreedyRobber();
robber2.FillBagWithJewels(bag2, availableJewels);
sw.Stop();

Console.WriteLine($"It took the robber {sw.Elapsed}");
bag2.PrintContents();


var bag1 = new Bag(100);
sw.Start();
IRobber robber1 = new BruteRobber();
robber1.FillBagWithJewels(bag1, availableJewels);
sw.Stop();

Console.WriteLine($"It took the robber {sw.Elapsed}");
bag1.PrintContents();



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

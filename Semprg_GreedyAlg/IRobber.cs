// Greedy algorithm which solves for ~largest price such
// that it fits in a bag of a given size.

// For all jewels, pick the jewel with largest price that still
// fits into the bagg and add it to the bag
// End when No more jewels can fit

public interface IRobber
{
    public void FillBagWithJewels(Bag bag, Jewel[] availableJewels);
}

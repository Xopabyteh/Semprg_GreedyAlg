using System.Text;

public ref struct Bag
{
    public int MaxVolume { get; init; }
    
    private List<Jewel> jewels;
    private int volumeInside = 0;

    /// <summary>
    /// New empty bag
    /// </summary>
    public Bag(int maxVolume)
    {
        MaxVolume = maxVolume;
        jewels = new List<Jewel>();
    }

    public bool DoesJewelFit(Jewel jewel)
    {
        return volumeInside + jewel.Volume <= MaxVolume;
    }

    public void AddJewel(Jewel jewel)
    {
        if (!DoesJewelFit(jewel))
        {
            throw new InvalidOperationException("Jewel does not fit in bag");
        }

        jewels.Add(jewel);
        volumeInside += jewel.Volume;
    }

    public void AddJewels(IEnumerable<Jewel> jewels)
    {
        var jewelsVolume = jewels.Sum(j => j.Volume);

        if (volumeInside + jewelsVolume > MaxVolume)
        {
            throw new InvalidOperationException("Jewels do not fit in bag");
        }

        this.jewels.AddRange(jewels);
        volumeInside += jewelsVolume;    
    }

    public void PrintContents()
    {
        var builder = new StringBuilder();
       
        var totalPrice = jewels.Sum(j => j.Price);
        var usedVolume = jewels.Sum(j => j.Volume);

        builder.AppendLine($"Bag with volume {usedVolume}l/{MaxVolume}l contains ({totalPrice:C}):");
        //foreach (var jewel in jewels)
        //{
        //    builder.AppendLine($"\t{jewel.Volume}l, {jewel.Price:C}");
        //}

        Console.WriteLine(builder.ToString());
    }
}

public readonly record struct Jewel(double Volume, double Price)
{
    public static Jewel[] GenerateRandomJewelsFrom(int amm, int seed, ReadOnlySpan<Jewel> fromJewelTypes)
    {
        var rng = new Random(seed);
        var jewels = new Jewel[amm];

        for (int i = 0; i < amm; i++)
        {
            var jewelType = fromJewelTypes[rng.Next(fromJewelTypes.Length)];

            jewels[i] = (jewelType with { });
        }

        return jewels;
    }

    public static Jewel[] StaticJewelTypes =
    [
        new Jewel(1, 10),
        new Jewel(2, 15),
        new Jewel(3, 99),
        new Jewel(15, 20),
        new Jewel(36, 100),
        new Jewel(40, 15),
    ];
}
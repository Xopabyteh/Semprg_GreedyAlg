public readonly record struct Jewel(int Volume, double Price)
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
        new Jewel(2, 120),
        new Jewel(3, 140),
        new Jewel(5, 100),
        new Jewel(15, 600),
        new Jewel(36, 100),
        new Jewel(40, 15),
        new Jewel(42, 900),
    ];
}
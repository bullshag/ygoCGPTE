using System;
using System.Linq;
using WinFormsApp2;

namespace WinFormsApp2.Tests;

public class LootServiceTests
{
    [Fact]
    public void BonusLootDropRateIsApproximatelyHalf()
    {
        var rng = new Random(0);
        const int trials = 10000;
        int drops = Enumerable.Range(0, trials)
            .Count(_ => LootService.ShouldDropBonusLoot(rng));
        double rate = drops / (double)trials;
        Assert.InRange(rate, 0.45, 0.55);
    }
}

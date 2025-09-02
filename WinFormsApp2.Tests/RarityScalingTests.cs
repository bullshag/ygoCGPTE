using System;
using System.Linq;
using Xunit;

namespace WinFormsApp2.Tests;

public class RarityScalingTests
{
    [Fact]
    public void LowLevelNpcRareItemsAreScarce()
    {
        int rareOrBetter = 0;
        for (int i = 0; i < 1000; i++)
        {
            var rarity = LootService.RollRarityForLevel(1);
            if (rarity >= Rarity.Blue)
                rareOrBetter++;
        }
        Assert.True(rareOrBetter < 150, $"Unexpected rare count: {rareOrBetter}");
    }

    [Fact]
    public void HighLevelNpcGetsMoreRareItems()
    {
        int rareOrBetter = 0;
        for (int i = 0; i < 1000; i++)
        {
            var rarity = LootService.RollRarityForLevel(50);
            if (rarity >= Rarity.Blue)
                rareOrBetter++;
        }
        Assert.True(rareOrBetter > 300, $"Unexpected rare count: {rareOrBetter}");
    }
}

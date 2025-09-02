using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace WinFormsApp2.Tests;

public class TrinketEffectsTests
{
    [Fact]
    public void AllTrinketEffectsAreHandledInCode()
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        var sqlPath = Path.Combine(root, "trinkets.sql");
        var codePath = Path.Combine(root, "WinFormsApp2", "BattleForm.cs");

        var sql = File.ReadAllText(sqlPath);
        var code = File.ReadAllText(codePath);

        var sqlKeys = new HashSet<string>();
        foreach (Match m in Regex.Matches(sql, @"JSON_OBJECT\((.*?)\)", RegexOptions.Singleline))
        {
            foreach (Match km in Regex.Matches(m.Groups[1].Value, @"'([a-z_]+)'"))
            {
                sqlKeys.Add(km.Groups[1].Value);
            }
        }

        sqlKeys.Remove("combat_regen_interval_sec");

        var codeKeys = new HashSet<string>();
        foreach (Match m in Regex.Matches(code, @"case ""([a-z_]+)""") )
        {
            codeKeys.Add(m.Groups[1].Value);
        }

        Assert.True(sqlKeys.IsSubsetOf(codeKeys), $"Missing handlers for: {string.Join(", ", sqlKeys.Except(codeKeys))}");
    }
}

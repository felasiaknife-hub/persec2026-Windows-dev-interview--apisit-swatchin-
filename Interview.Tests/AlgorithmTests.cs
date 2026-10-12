using Interview.Core;
using Xunit;

namespace Interview.Tests;

public class AlgorithmTests
{
    [Theory]
    [InlineData("()", true)]
    [InlineData("([{)", false)]
    [InlineData("([]]", false)]
    [InlineData("([{}])", true)]
    [InlineData("([[{}]]]", false)]
    [InlineData(")", false)]
    [InlineData("(]}])", false)]
    [InlineData("([)]", false)]
    [InlineData("{", false)]
    [InlineData("", true)]
    [InlineData(null, false)]
    [InlineData("()[]{}", true)]
    [InlineData("(a)", false)]
    [InlineData(")( ", false)]
    public void BracketCases(string? input, bool expected) => Assert.Equal(expected, Algorithms.IsBalanced(input));

    [Fact]
    public void NaturalSortMatchesBothExamples()
    {
        Assert.Equal(new[] { "SG20", "TH2", "TH19" }, Algorithms.NaturalSort(["TH19", "SG20", "TH2"]));
        Assert.Equal(new[] { "TH1", "TH3Netflix", "TH7", "TH10" }, Algorithms.NaturalSort(["TH10", "TH3Netflix", "TH1", "TH7"]));
    }

    [Fact]
    public void NaturalSortHandlesHugeNumbersStableTiesAndDoesNotMutate()
    {
        string[] input = ["TH99999999999999999999999", "TH02", "th2", "TH1"];
        string[] copy = (string[])input.Clone();
        Assert.Equal(new[] { "TH1", "TH02", "th2", "TH99999999999999999999999" }, Algorithms.NaturalSort(input));
        Assert.Equal(copy, input);
        Assert.Equal(new[] { "A2B3", "A2B10", "A10B1" }, Algorithms.NaturalSort(["A10B1", "A2B10", "A2B3"]));
        Assert.Empty(Algorithms.NaturalSort([]));
        Assert.Throws<ArgumentNullException>(() => Algorithms.NaturalSort(null!));
        Assert.Throws<ArgumentException>(() => Algorithms.NaturalSort([null!]));
    }

    [Fact]
    public void AutocompleteMatchesExample() => Assert.Equal(new[] { "Think", "Mother" },
        Algorithms.Autocomplete("th", ["Mother", "Think", "Worthy", "Apple", "Android"], 2));

    [Fact]
    public void AutocompletePrioritizesStartMiddleEndAndPreservesTies()
    {
        Assert.Equal(new[] { "THink", "th", "th", "Mother", "Worthy", "Bath" },
            Algorithms.Autocomplete("TH", ["Bath", "THink", "th", "Mother", "Worthy", "th"], 99));
    }

    [Fact]
    public void AutocompleteBoundaries()
    {
        Assert.Empty(Algorithms.Autocomplete("", ["Think"], 2));
        Assert.Empty(Algorithms.Autocomplete("th", ["Think"], 0));
        Assert.Empty(Algorithms.Autocomplete("zzz", ["Think"], 10));
        Assert.Empty(Algorithms.Autocomplete("th", [], 3));
        Assert.Throws<ArgumentOutOfRangeException>(() => Algorithms.Autocomplete("th", [], -1));
        Assert.Throws<ArgumentNullException>(() => Algorithms.Autocomplete(null!, [], 1));
    }

    [Theory]
    [InlineData(1989, "MCMLXXXIX")]
    [InlineData(2000, "MM")]
    [InlineData(68, "LXVIII")]
    [InlineData(109, "CIX")]
    [InlineData(1, "I")]
    [InlineData(4, "IV")]
    [InlineData(9, "IX")]
    [InlineData(3999, "MMMCMXCIX")]
    public void RomanExamples(int value, string roman)
    {
        Assert.Equal(roman, Algorithms.ToRoman(value));
        Assert.Equal(value, Algorithms.FromRoman(roman));
    }

    [Fact]
    public void RomanRoundTripsEverySupportedNumber()
    {
        for (int n = 1; n <= 3999; n++) Assert.Equal(n, Algorithms.FromRoman(Algorithms.ToRoman(n)));
        Assert.Equal(4, Algorithms.FromRoman(" iv "));
    }

    [Theory]
    [InlineData("")]
    [InlineData("IIII")]
    [InlineData("IC")]
    [InlineData("VX")]
    [InlineData("MMMM")]
    [InlineData("ABC")]
    [InlineData("I V")]
    public void InvalidRomansAreRejected(string roman) => Assert.Throws<ArgumentException>(() => Algorithms.FromRoman(roman));

    [Fact]
    public void RomanOutOfRangeIsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Algorithms.ToRoman(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Algorithms.ToRoman(4000));
        Assert.Throws<ArgumentNullException>(() => Algorithms.FromRoman(null!));
    }

    [Theory]
    [InlineData(3008, 8300)]
    [InlineData(1989, 9981)]
    [InlineData(2679, 9762)]
    [InlineData(9163, 9631)]
    [InlineData(5, 5)]
    [InlineData(111, 111)]
    public void DescendingDigitExamples(int input, int expected) => Assert.Equal(expected, Algorithms.DescendingDigits(input));

    [Fact]
    public void DigitErrors()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Algorithms.DescendingDigits(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Algorithms.DescendingDigits(-1));
        Assert.Throws<OverflowException>(() => Algorithms.DescendingDigits(int.MaxValue));
    }

    [Fact]
    public void TribonacciExamples()
    {
        Assert.Equal(new[] { 1, 3, 5, 9, 17 }, Algorithms.Tribonacci([1, 3, 5], 5));
        Assert.Equal(new[] { 2, 2, 2 }, Algorithms.Tribonacci([2, 2, 2], 3));
        Assert.Equal(new[] { 10, 10, 10, 30 }, Algorithms.Tribonacci([10, 10, 10], 4));
        Assert.Equal(new[] { 1, 1, 1, 3, 5, 9 }, Algorithms.Tribonacci([1, 1, 1], 6));
        Assert.Equal(new[] { 2, 3, 1, 6, 10, 17 },Algorithms.Tribonacci([2, 3, 1], 6));
    }

    [Fact]
    public void TribonacciShortSeedsAndTruncation()
    {
        Assert.Equal(new[] { 0, 0, 0, 0 }, Algorithms.Tribonacci([], 4));
        Assert.Equal(new[] { 1, 0, 0, 1, 1, 2 }, Algorithms.Tribonacci([1], 6));
        Assert.Equal(new[] { 2, 3, 0, 5, 8 }, Algorithms.Tribonacci([2, 3], 5));
        Assert.Equal(new[] { 1, 3 }, Algorithms.Tribonacci([1, 3, 5], 2));
        Assert.Empty(Algorithms.Tribonacci([1, 2, 3], 0));
        Assert.Equal(new[] { int.MaxValue, 1, -1, int.MaxValue }, Algorithms.Tribonacci([int.MaxValue, 1, -1], 4));
    }

    [Fact]
    public void TribonacciErrors()
    {
        Assert.Throws<ArgumentException>(() => Algorithms.Tribonacci([1, 2, 3, 4], 5));
        Assert.Throws<ArgumentOutOfRangeException>(() => Algorithms.Tribonacci([], -1));
        Assert.Throws<ArgumentNullException>(() => Algorithms.Tribonacci(null!, 1));
        Assert.Throws<OverflowException>(() => Algorithms.Tribonacci([int.MaxValue, 1, 0], 4));
    }
}

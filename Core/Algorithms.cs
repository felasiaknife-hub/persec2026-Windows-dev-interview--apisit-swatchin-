using System.Globalization;
using System.Text;

namespace Interview.Core;

public static class Algorithms
{
    // 1. Last opened bracket must be the first one closed.
    public static bool IsBalanced(string? input)
    {
        if (input is null) return false;
        var stack = new Stack<char>();
        foreach (char c in input)
        {
            if (c is '(' or '[' or '{') { stack.Push(c); continue; }
            if (c is not (')' or ']' or '}') || stack.Count == 0) return false;
            char open = stack.Pop();
            if (c == ')' && open != '(' || c == ']' && open != '[' || c == '}' && open != '{') return false;
        }
        return stack.Count == 0;
    }

    // 2. Compare digit runs numerically, without parsing them into a bounded integer.
    public static string[] NaturalSort(string[] items)
    {
        ValidateItems(items);
        return items.OrderBy(x => x, Comparer<string>.Create(CompareNatural)).ToArray();
    }

    private static bool IsDigit(char c) => c >= '0' && c <= '9';

    private static int CompareNatural(string? a, string? b)
    {
        a ??= ""; b ??= "";
        int i = 0, j = 0;
        while (i < a.Length && j < b.Length)
        {
            if (IsDigit(a[i]) && IsDigit(b[j]))
            {
                int endA = i, endB = j;
                while (endA < a.Length && IsDigit(a[endA])) endA++;
                while (endB < b.Length && IsDigit(b[endB])) endB++;
                while (i < endA && a[i] == '0') i++;
                while (j < endB && b[j] == '0') j++;
                int cmp = (endA - i).CompareTo(endB - j);
                if (cmp != 0) return cmp;
                cmp = string.Compare(a, i, b, j, endA - i, StringComparison.Ordinal);
                if (cmp != 0) return cmp;
                i = endA; j = endB;
            }
            else
            {
                int cmp = char.ToUpperInvariant(a[i]).CompareTo(char.ToUpperInvariant(b[j]));
                if (cmp != 0) return cmp;
                i++; j++;
            }
        }
        return (a.Length - i).CompareTo(b.Length - j);
    }

    // 3. Stable order within each group: start, middle, end.
    public static string[] Autocomplete(string search, string[] items, int maxResult)
    {
        ArgumentNullException.ThrowIfNull(search);
        ValidateItems(items);
        if (maxResult < 0) throw new ArgumentOutOfRangeException(nameof(maxResult));
        if (search.Length == 0 || maxResult == 0) return [];
        return items.Select(item => new { Item = item, Rank = MatchRank(item, search) })
            .Where(x => x.Rank >= 0).OrderBy(x => x.Rank)
            .Take(maxResult).Select(x => x.Item).ToArray();
    }

    private static int MatchRank(string item, string search)
    {
        int index = item.IndexOf(search, StringComparison.OrdinalIgnoreCase);
        if (index < 0) return -1;
        if (index == 0) return 0;
        return index + search.Length < item.Length ? 1 : 2;
    }

    private static readonly (int Value, string Symbol)[] RomanPairs =
    [ (1000,"M"), (900,"CM"), (500,"D"), (400,"CD"), (100,"C"),
      (90,"XC"), (50,"L"), (40,"XL"), (10,"X"), (9,"IX"), (5,"V"), (4,"IV"), (1,"I") ];

    // 4. Standard notation without overbars: 1..3999.
    public static string ToRoman(int number)
    {
        if (number < 1 || number > 3999) throw new ArgumentOutOfRangeException(nameof(number), "รองรับเลข 1 ถึง 3999");
        var result = new StringBuilder();
        foreach (var pair in RomanPairs)
            while (number >= pair.Value) { result.Append(pair.Symbol); number -= pair.Value; }
        return result.ToString();
    }

    public static int FromRoman(string roman)
    {
        ArgumentNullException.ThrowIfNull(roman);
        string text = roman.Trim().ToUpperInvariant();
        if (text.Length == 0 || text.Length > 15) throw new ArgumentException("เลขโรมันไม่ถูกต้อง", nameof(roman));
        int index = 0, number = 0;
        foreach (var pair in RomanPairs)
            while (text.AsSpan(index).StartsWith(pair.Symbol.AsSpan(), StringComparison.Ordinal))
            { number += pair.Value; index += pair.Symbol.Length; }
        if (index != text.Length || number < 1 || number > 3999 || ToRoman(number) != text)
            throw new ArgumentException("เลขโรมันไม่ถูกต้อง เช่น IV = 4 แต่ IIII ไม่ใช่รูปแบบมาตรฐาน", nameof(roman));
        return number;
    }

    // 5. Overflow is reported instead of silently wrapping around.
    public static int DescendingDigits(int number)
    {
        if (number <= 0) throw new ArgumentOutOfRangeException(nameof(number), "ต้องเป็นจำนวนเต็มบวก");
        var digits = number.ToString(CultureInfo.InvariantCulture).OrderByDescending(c => c);
        int result = 0;
        foreach (char digit in digits) result = checked(result * 10 + digit - '0');
        return result;
    }

    // 6. Assumption for incomplete seeds: pad missing seed positions on the right with 0.
    public static int[] Tribonacci(int[] seed, int count)
    {
        ArgumentNullException.ThrowIfNull(seed);
        if (seed.Length > 3) throw new ArgumentException("ค่าเริ่มต้นต้องมี 0 ถึง 3 ตัว", nameof(seed));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
        var result = new int[count];
        Array.Copy(seed, result, Math.Min(seed.Length, count));
        for (int i = 3; i < count; i++)
            result[i] = checked((int)((long)result[i - 1] + result[i - 2] + result[i - 3]));
        return result;
    }

    private static void ValidateItems(string[] items)
    {
        ArgumentNullException.ThrowIfNull(items);
        if (items.Any(x => x is null)) throw new ArgumentException("รายการต้องไม่มีค่า null", nameof(items));
    }
}

using System.Globalization;
using System.Text.RegularExpressions;
using Bestella.Models;

namespace Bestella.Services;

public sealed class OrderParseResult
{
    public OrderParseResult(ParsedOrder order, IReadOnlyList<string> errors)
    {
        Order = order;
        Errors = errors;
    }

    public ParsedOrder Order { get; }
    public IReadOnlyList<string> Errors { get; }
    public bool IsValid => Errors.Count == 0;
}

public sealed class OrderParser
{
    private static readonly string[] DateFormats =
    [
        "d MMMM yyyy", "dd MMMM yyyy", "d MMM yyyy", "dd MMM yyyy",
        "d/M/yyyy", "dd/MM/yyyy", "d/M/yy", "dd/MM/yy", "yyyy-MM-dd",
        "M/d/yyyy", "MM/dd/yyyy"
    ];

    private static readonly Regex ItemPattern = new(
        @"^(?<quantity>\d+)\s*(?:x|X)?\s*(?<product>.+)$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public OrderParseResult Parse(string orderText)
    {
        var order = new ParsedOrder();

        foreach (var line in orderText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            if (TryGetValue(line, "Nama", out var value))
                order.Name = value;
            else if (TryGetValue(line, "Alamat", out value))
                order.Address = value;
            else if (TryGetValue(line, "Tarikh", out value))
                order.ParsedDate = ParseDate(value);
            else if (TryGetValue(line, "Phone", out value))
                order.Phone = value;
            else if (TryGetValue(line, "Type", out value) && TryParseItem(value, out var item))
                order.Items.Add(item);
        }

        return new OrderParseResult(order, Validate(order));
    }

    private static bool TryGetValue(string line, string label, out string value)
    {
        var trimmed = line.Trim();
        if (!trimmed.StartsWith(label, StringComparison.OrdinalIgnoreCase))
        {
            value = string.Empty;
            return false;
        }

        var remainder = trimmed[label.Length..].TrimStart();
        if (!remainder.StartsWith(':'))
        {
            value = string.Empty;
            return false;
        }

        value = remainder[1..].Trim();
        return true;
    }

    private static DateOnly? ParseDate(string value)
    {
        foreach (var culture in new[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("ms-MY"), CultureInfo.GetCultureInfo("en-GB") })
        {
            if (DateOnly.TryParseExact(value, DateFormats, culture, DateTimeStyles.AllowWhiteSpaces, out var date))
                return date;
        }

        return null;
    }

    private static bool TryParseItem(string value, out OrderItem item)
    {
        var match = ItemPattern.Match(value.Trim());
        if (match.Success && int.TryParse(match.Groups["quantity"].Value, out var quantity))
        {
            item = new OrderItem
            {
                ProductName = match.Groups["product"].Value.Trim(),
                Quantity = quantity
            };
            return true;
        }

        item = new OrderItem { ProductName = value.Trim(), Quantity = 1 };
        return !string.IsNullOrWhiteSpace(item.ProductName);
    }

    private static List<string> Validate(ParsedOrder order)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(order.Name)) errors.Add("Name is required.");
        if (string.IsNullOrWhiteSpace(order.Address)) errors.Add("Address is required.");
        if (order.ParsedDate is null) errors.Add("A valid date is required.");
        if (string.IsNullOrWhiteSpace(order.Phone)) errors.Add("Phone is required.");
        if (order.Items.Count == 0) errors.Add("At least one item is required.");
        if (order.Items.Any(item => string.IsNullOrWhiteSpace(item.ProductName))) errors.Add("Each item needs a product name.");
        if (order.Items.Any(item => item.Quantity is < 1 or > 1000)) errors.Add("Each quantity must be between 1 and 1000.");
        return errors;
    }
}
using Bestella.Services;
using Xunit;

namespace Bestella.Tests;

public class OrderParserTests
{
    private readonly OrderParser parser = new();

    [Fact]
    public void Parse_valid_order_creates_preview_without_saving()
    {
        var result = parser.Parse("""
            Nama: Aina
            Alamat: 12 Jalan Mawar
            Tarikh: 16/09/2026
            Phone: 0123456789
            Type: 2 x Chocolate Cake
            Type: Cookies
            """);

        Assert.True(result.IsValid);
        Assert.Equal(new DateOnly(2026, 9, 16), result.Order.ParsedDate);
        Assert.Equal(2, result.Order.Items.Count);
        Assert.Equal(2, result.Order.Items[0].Quantity);
        Assert.Equal("Cookies", result.Order.Items[1].ProductName);
    }

    [Fact]
    public void Parse_invalid_order_returns_errors_and_does_not_mark_valid()
    {
        var result = parser.Parse("Nama: Aina\nTarikh: not-a-date\nType: 0 x Cake");

        Assert.False(result.IsValid);
        Assert.Contains("Address is required.", result.Errors);
        Assert.Contains("A valid date is required.", result.Errors);
        Assert.Contains("Phone is required.", result.Errors);
        Assert.Contains("Each quantity must be between 1 and 1000.", result.Errors);
    }

    [Fact]
    public void Parse_does_not_accept_similar_labels_without_a_colon()
    {
        var result = parser.Parse("NamaCustomer: Aina\nAlamat: 12 Jalan Mawar");

        Assert.Equal("", result.Order.Name);
        Assert.Contains("Name is required.", result.Errors);
    }
}
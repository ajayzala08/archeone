using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class CountryMst
{
    public int? CountryId { get; set; }

    public string? CountryName { get; set; }

    public string? Iso3 { get; set; }

    public string? Iso2 { get; set; }

    public string? NumericCode { get; set; }

    public string? PhoneCode { get; set; }

    public string? Capital { get; set; }

    public string? Currency { get; set; }

    public string? CurrencyName { get; set; }

    public string? CurrencySymbol { get; set; }

    public string? Tld { get; set; }

    public string? Native { get; set; }

    public string? Region { get; set; }

    public string? SubRegion { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }

    public string? Emoji { get; set; }

    public string? EmojiU { get; set; }
}

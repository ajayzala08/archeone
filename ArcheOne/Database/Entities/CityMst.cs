using System;
using System.Collections.Generic;

namespace ArcheOne.Database.Entities;

public partial class CityMst
{
    public int? CityId { get; set; }

    public int? StateId { get; set; }

    public string? CityName { get; set; }

    public string? Latitude { get; set; }

    public string? Longitude { get; set; }
}

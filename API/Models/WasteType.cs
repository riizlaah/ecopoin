using System;
using System.Collections.Generic;

namespace EcoPoinAPI.Models;

public partial class WasteType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int PointTariff { get; set; }

    public decimal Co2factor { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
}

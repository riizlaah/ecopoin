using System;
using System.Collections.Generic;

namespace EcoPoinAPI.Models;

public partial class Voucher
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int PointCost { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<RedemptionPoint> RedemptionPoints { get; set; } = new List<RedemptionPoint>();
}

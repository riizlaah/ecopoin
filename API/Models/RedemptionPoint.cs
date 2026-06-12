using System;
using System.Collections.Generic;

namespace EcoPoinAPI.Models;

public partial class RedemptionPoint
{
    public int Id { get; set; }

    public int ResidentId { get; set; }

    public int VoucherId { get; set; }

    public string Code { get; set; } = null!;

    public int Amount { get; set; }

    public int PointCost { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User Resident { get; set; } = null!;

    public virtual Voucher Voucher { get; set; } = null!;
}

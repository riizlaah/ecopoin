using System;
using System.Collections.Generic;

namespace EcoPoinAPI.Models;

public partial class DepositPoint
{
    public int Id { get; set; }

    public int ResidentId { get; set; }

    public int DepositId { get; set; }

    public decimal Amount { get; set; }

    public decimal PointTariff { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Deposit Deposit { get; set; } = null!;

    public virtual User Resident { get; set; } = null!;
}

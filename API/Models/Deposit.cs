using System;
using System.Collections.Generic;

namespace EcoPoinAPI.Models;

public partial class Deposit
{
    public int Id { get; set; }

    public int WasteTypeId { get; set; }

    public int ResidentId { get; set; }

    public int? OfficerId { get; set; }

    public decimal EstimatedWeight { get; set; }

    public decimal? ActualWeight { get; set; }

    public string? Notes { get; set; }

    public string PhotoPath { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? RejectionReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual DepositPoint? DepositPoint { get; set; } = null!;

    public virtual User? Officer { get; set; }

    public virtual User Resident { get; set; } = null!;

    public virtual WasteType WasteType { get; set; } = null!;
}

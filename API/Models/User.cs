using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;

namespace EcoPoinAPI.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Deposit> DepositOfficers { get; set; } = new List<Deposit>();

    public virtual ICollection<DepositPoint> DepositPoints { get; set; } = new List<DepositPoint>();

    public virtual ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();

    public virtual ICollection<RedemptionPoint> RedemptionPoints { get; set; } = new List<RedemptionPoint>();

    public virtual int Balance => DepositPoints.Sum(dp => dp.Amount) - RedemptionPoints.Sum(rp => rp.Amount);
    public virtual decimal ThisMonthBalance => DepositPoints.Where(dp => dp.CreatedAt.Month == DateTime.Today.Month).Sum(dp => dp.Amount) - RedemptionPoints.Where(rp => rp.CreatedAt.Month == DateTime.Today.Month).Sum(rp => rp.Amount);
    public virtual int ThisMonthTotalPoints => DepositPoints.Where(dp => dp.CreatedAt.Month == DateTime.Today.Month).Sum(dp => dp.Amount);
    public virtual decimal ThisMonthEnvImpact => Deposits.Where(d => d.Status == "Verified" && d.CreatedAt.Month == DateTime.Today.Month).Sum(d => (d.ActualWeight ?? 0m) * d.WasteType.Co2factor);
    public virtual decimal TotalSubmittedWeights => Deposits.Where(d => d.Status == "Verified").Sum(d => d.ActualWeight ?? 0m);
    public virtual decimal EnvironmentalImpact => Deposits.Where(d => d.Status == "Verified").Sum(d => (d.ActualWeight ?? 0m) * d.WasteType.Co2factor);
    public virtual int TotalPoints => DepositPoints.Sum(rp => rp.Amount);
    public virtual int RedeemedPoints => RedemptionPoints.Sum(rp => rp.Amount);
}

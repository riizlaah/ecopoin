using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcoPoinAPI.Models;

public partial class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("fullName")]
    [StringLength(64)]
    [Unicode(false)]
    public string FullName { get; set; } = null!;

    [Column("username")]
    [StringLength(64)]
    [Unicode(false)]
    public string Username { get; set; } = null!;

    [Column("email")]
    [StringLength(64)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Column("phone")]
    [StringLength(32)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    [Column("password")]
    [StringLength(256)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [Column("role")]
    [StringLength(256)]
    [Unicode(false)]
    public string Role { get; set; } = null!;

    [InverseProperty("Officer")]
    public virtual ICollection<Deposit> DepositOfficers { get; set; } = new List<Deposit>();

    [InverseProperty("Resident")]
    public virtual ICollection<DepositPoint> DepositPoints { get; set; } = new List<DepositPoint>();

    [InverseProperty("Resident")]
    public virtual ICollection<Deposit> DepositResidents { get; set; } = new List<Deposit>();

    [InverseProperty("Resident")]
    public virtual ICollection<RedemptionPoint> RedemptionPoints { get; set; } = new List<RedemptionPoint>();


    public virtual int TotalBalance => DepositPoints.Sum(dp => dp.Amount) - RedemptionPoints.Sum(rp => rp.Amount);
    public virtual int TotalPoints => DepositPoints.Sum(dp => dp.Amount);
    public virtual decimal TotalEnvImpact => DepositResidents.Where(dr => dr.Status == "Verified").Sum(dr => (dr.ActualWeight ?? 0) * dr.WasteType.Co2factor);
    public virtual decimal TotalSubmittedWeight => DepositResidents.Where(dr => dr.Status == "Verified").Sum(dr => dr.ActualWeight ?? 0);

    public virtual int ThisMonthBalance => DepositPoints.Where(dp => dp.CreatedAt.Month == DateTime.Now.Month && dp.CreatedAt.Year == DateTime.Now.Year).Sum(dp => dp.Amount) - RedemptionPoints.Where(rp => rp.CreatedAt.Month == DateTime.Now.Month && rp.CreatedAt.Year == DateTime.Now.Year).Sum(rp => rp.Amount);
    public virtual int ThisMonthPoints => DepositPoints.Where(dp => dp.CreatedAt.Month == DateTime.Now.Month && dp.CreatedAt.Year == DateTime.Now.Year).Sum(dp => dp.Amount);
    public virtual decimal ThisMonthEnvImpact => DepositResidents.Where(dr => dr.Status == "Verified" && dr.CreatedAt.Month == DateTime.Now.Month && dr.CreatedAt.Year == DateTime.Now.Year).Sum(dr => (dr.ActualWeight ?? 0) * dr.WasteType.Co2factor);
    public virtual decimal ThisMonthSubmittedWeight => DepositResidents.Where(dr => dr.Status == "Verified" && dr.CreatedAt.Month == DateTime.Now.Month && dr.CreatedAt.Year == DateTime.Now.Year).Sum(dr => dr.ActualWeight ?? 0);
}

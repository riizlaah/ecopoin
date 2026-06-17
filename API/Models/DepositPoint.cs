using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcopoinAPI.Models;

public partial class DepositPoint
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("residentId")]
    public int ResidentId { get; set; }

    [Column("depositId")]
    public int DepositId { get; set; }

    [Column("amount")]
    public int Amount { get; set; }

    [Column("pointTariff")]
    public int PointTariff { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("DepositId")]
    [InverseProperty("DepositPoint")]
    public virtual Deposit Deposit { get; set; } = null!;

    [ForeignKey("ResidentId")]
    [InverseProperty("DepositPoints")]
    public virtual User Resident { get; set; } = null!;
}

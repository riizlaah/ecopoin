using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcopoinAPI.Models;

public partial class RedemptionPoint
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("residentId")]
    public int ResidentId { get; set; }

    [Column("voucherId")]
    public int VoucherId { get; set; }

    [Column("code")]
    [StringLength(72)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("amount")]
    public int Amount { get; set; }

    [Column("pointCost")]
    public int PointCost { get; set; }

    [Column("isUsed")]
    public bool IsUsed { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updatedAt", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("ResidentId")]
    [InverseProperty("RedemptionPoints")]
    public virtual User Resident { get; set; } = null!;

    [ForeignKey("VoucherId")]
    [InverseProperty("RedemptionPoints")]
    public virtual Voucher Voucher { get; set; } = null!;
}

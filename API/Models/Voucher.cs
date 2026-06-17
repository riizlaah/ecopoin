using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcopoinAPI.Models;

public partial class Voucher
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(64)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("code")]
    [StringLength(24)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("pointCost")]
    public int PointCost { get; set; }

    [Column("isActive")]
    public bool IsActive { get; set; }

    [InverseProperty("Voucher")]
    public virtual ICollection<RedemptionPoint> RedemptionPoints { get; set; } = new List<RedemptionPoint>();
}

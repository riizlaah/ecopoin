using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcoPoinAPI.Models;

public partial class WasteType
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(64)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("code")]
    [StringLength(64)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("pointTariff")]
    public int PointTariff { get; set; }

    [Column("CO2Factor", TypeName = "decimal(18, 2)")]
    public decimal Co2factor { get; set; }

    [Column("isActive")]
    public bool IsActive { get; set; }

    [InverseProperty("WasteType")]
    public virtual ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
}

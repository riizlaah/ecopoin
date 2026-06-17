using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EcopoinAPI.Models;

public partial class Deposit
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("wasteTypeId")]
    public int WasteTypeId { get; set; }

    [Column("residentId")]
    public int ResidentId { get; set; }

    [Column("officerId")]
    public int? OfficerId { get; set; }

    [Column("estimatedWeight", TypeName = "decimal(18, 2)")]
    public decimal EstimatedWeight { get; set; }

    [Column("actualWeight", TypeName = "decimal(18, 2)")]
    public decimal? ActualWeight { get; set; }

    [Column("notes")]
    [StringLength(300)]
    [Unicode(false)]
    public string? Notes { get; set; }

    [Column("photoPath")]
    [StringLength(256)]
    [Unicode(false)]
    public string PhotoPath { get; set; } = null!;

    [Column("status")]
    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [Column("rejectionReason")]
    [StringLength(300)]
    [Unicode(false)]
    public string? RejectionReason { get; set; }

    [Column("createdAt", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updatedAt", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Deposit")]
    public virtual DepositPoint? DepositPoint { get; set; }

    [ForeignKey("OfficerId")]
    [InverseProperty("DepositOfficers")]
    public virtual User? Officer { get; set; }

    [ForeignKey("ResidentId")]
    [InverseProperty("DepositResidents")]
    public virtual User Resident { get; set; } = null!;

    [ForeignKey("WasteTypeId")]
    [InverseProperty("Deposits")]
    public virtual WasteType WasteType { get; set; } = null!;
}

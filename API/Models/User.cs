using System;
using System.Collections.Generic;

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

    public virtual ICollection<Deposit> DepositResidents { get; set; } = new List<Deposit>();

    public virtual ICollection<RedemptionPoint> RedemptionPoints { get; set; } = new List<RedemptionPoint>();
}

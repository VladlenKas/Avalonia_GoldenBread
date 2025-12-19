using Humanizer;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoldenBread.Domain.Models;

public partial class User 
{
    public int UserId { get; set; } 

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Patronymic { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public short Dismissed { get; set; }

    public string? CompanyName { get; set; }

    public string? CompanyInn { get; set; }

    public string? CompanyOgrn { get; set; }

    public string? CompanyPhone { get; set; }

    public string? CompanyAddress { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Favourite> Favourites { get; set; } = new List<Favourite>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}


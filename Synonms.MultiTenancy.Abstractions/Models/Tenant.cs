using System;

namespace Synonms.MultiTenancy.Abstractions.Models;

public class Tenant
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}
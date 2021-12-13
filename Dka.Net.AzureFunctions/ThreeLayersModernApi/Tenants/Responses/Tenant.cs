using System;

namespace ThreeLayersModernApi.Tenants.Responses;

public class Tenant
{
    public Guid Id { get; init; }
    public string Name { get; init; }
}
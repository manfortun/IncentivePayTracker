﻿namespace IncentivePayTracker.DTO;

public class Infraction
{
    public int Id { get; set; }
    public string Description { get; set; } = default!;
    public double Amount { get; set; }
}

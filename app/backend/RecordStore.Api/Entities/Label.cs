﻿namespace RecordStore.Api.Entities;

public class Label
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public virtual ICollection<Product> Products { get; set; } = null!;
}
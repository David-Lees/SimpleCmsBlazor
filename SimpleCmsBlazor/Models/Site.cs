﻿namespace SimpleCmsBlazor.Models;

public class Site : IPageList
{
    public string? Id { get; set; }
    public List<Page> Pages { get; set; } = [];
    public string? Name { get; set; }
    public string? HeaderBackground { get; set; }
    public string? HeaderColour { get; set; }
    public bool HasLogo { get; set; } = false;
    public bool? IsExpanded { get; set; }
}
using System;

namespace TestingControllersSample.Core.Model;

public class Idea
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public DateTimeOffset DateCreated { get; set; }
}

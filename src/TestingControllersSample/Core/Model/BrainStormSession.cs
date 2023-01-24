using System;
using System.Collections.Generic;

namespace TestingControllersSample.Core.Model;

public class BrainstormSession
{
    public int Id { get; set; }

    public string Name { get; set; }

    public DateTimeOffset DateCreated { get; set; }

    public List<Idea> Ideas { get; } = new();

    public void AddIdea(Idea idea)
    {
        Ideas.Add(idea);
    }
}

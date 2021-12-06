using Jobberwocky.Domain;
using System;

namespace Jobberwocky.Test.Helpers
{
  public static class TestDataCreator
  {
    public static Company Company(Guid? id = null, string name = null, string description = null)
    {
      return new Company()
      {
        Id = id ?? Guid.NewGuid(),
        Name = name ?? Guid.NewGuid().ToString().Substring(0, 20),
        Description = description ?? Guid.NewGuid().ToString(),
      };
    }
  }
}

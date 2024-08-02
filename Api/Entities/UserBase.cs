using System.ComponentModel.DataAnnotations;

namespace test.Api.Entities;

public abstract class UserBase
{
    public int Id { get; set; }
    public string? Username { get; set; }
}
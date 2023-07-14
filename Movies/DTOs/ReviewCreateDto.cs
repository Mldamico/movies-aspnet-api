using System.ComponentModel.DataAnnotations;

namespace Movies.DTOs;

public class ReviewCreateDto
{
    public string Comment { get; set; }
    [Range(1, 5)] public int Score { get; set; }
}
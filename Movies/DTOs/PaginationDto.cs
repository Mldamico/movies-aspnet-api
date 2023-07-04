namespace Movies.DTOs;

public class PaginationDto
{
    public int Page { get; set; } = 1;
    private int _amountPerPage = 10;
    private readonly int _maxAmountPerPage = 50;

    public int AmountPerPage
    {
        get => _amountPerPage;
        set => _amountPerPage = (value > _maxAmountPerPage) ? _maxAmountPerPage : value;
    }
}
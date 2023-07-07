namespace Movies.DTOs;

public class FilterMoviesDto
{
    public int Page { get; set; } = 1;
    public int RegisterPerPage { get; set; } = 10;

    public PaginationDto Pagination
    {
        get
        {
            return new PaginationDto() {Page = Page, AmountPerPage = RegisterPerPage};

        }
    }

    public string Title { get; set; }
    public int GenreId { get; set; }
    public bool Showcasing { get; set; }
    public bool NextRelease { get; set; }
    
}
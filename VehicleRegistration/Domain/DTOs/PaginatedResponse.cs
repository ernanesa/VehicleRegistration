namespace VehicleRegistration.Domain.DTOs;

public record PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; init; } = default!;
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public int TotalItems { get; init; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
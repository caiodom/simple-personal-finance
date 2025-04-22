using Microsoft.EntityFrameworkCore;
using SimplePersonalFinance.Shared.Contracts;

namespace SimplePersonalFinance.Shared.Extensions;

public static class PaginationExtensions
{
    public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(this IQueryable<T> query,
                                                                            int pageNumber,
                                                                            int pageSize,
                                                                            CancellationToken cancellation = default)
    {

        if (pageNumber < 1)
            throw new ArgumentException("Page number must be greater than zero", nameof(pageNumber));
        if (pageSize < 1)
            throw new ArgumentException("Page size must be greater than zero", nameof(pageSize));

        var totalItems = await query.CountAsync(cancellation);
        var items = await query.Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync(cancellation);

        return new PaginatedResult<T>(items, totalItems, pageNumber, pageSize);
    }
}

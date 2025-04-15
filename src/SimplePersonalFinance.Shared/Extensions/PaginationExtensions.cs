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
        var totalItems = await query.CountAsync(cancellation);

        var item = await query.Skip((pageNumber - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync(cancellation);

        return new PaginatedResult<T>(item, totalItems, pageNumber, pageSize);
    }

}

using System.Linq.Expressions;

namespace Texnokaktus.ProgOlymp.YandexContestIntegrationService.DataAccess.Extensions;

internal static class LinqExtensions
{
    public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source,
                                                       bool condition,
                                                       Expression<Func<TSource, bool>> predicate) =>
        condition ? source.Where(predicate) : source;
}

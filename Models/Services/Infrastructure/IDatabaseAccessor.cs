using System.Data;

namespace MyCourse.Models.Services.Infrastructure
{
    public interface IDatabaseAccessor
    {
        Task<int> CommandAsync(FormattableString formattableCommand);
        Task<DataSet> QueryAsync(FormattableString formattableScalarQuery);
        Task<T> QueryScalarAsync<T>(FormattableString formattableQuery);
    }
}
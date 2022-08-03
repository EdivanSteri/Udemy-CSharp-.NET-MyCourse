using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyCourse.Models.Services.Infrastructure
{
    public interface IDatabaseAccessor
    {
        Task<DataSet> QueryAsync(FormattableString query);
    }
}
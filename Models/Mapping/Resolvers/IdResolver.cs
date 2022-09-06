using AutoMapper;
using System.Data;

namespace MyCourse.Models.Mapping.Resolvers
{
    public class IdResolver : IValueResolver<DataRow, object, int>
    {
        public int Resolve(DataRow source, object destination, int destMember, ResolutionContext context)
        {
            return Convert.ToInt32(source["Id"]);
        }
    }
}

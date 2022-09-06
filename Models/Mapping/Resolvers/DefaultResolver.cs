using AutoMapper;
using System.Data;

namespace MyCourse.Models.Mapping.Resolvers
{
    public class DefaultResolver : IValueResolver<DataRow, object, object>
    {
        private readonly string memberName;
        public DefaultResolver(string memberName)
        {
            this.memberName = memberName;
        }
        public object Resolve(DataRow source, object destination, object destMember, ResolutionContext context)
        {
            return source[memberName];
        }
    }
}

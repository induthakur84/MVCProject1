using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVCProject1.DataAccess.Repository.Repository.IRepository
{
    public interface ISP_CALL : IDisposable
    {
        T Single<T>(string procName, DynamicParameters param = null);
        void Execute(string procName, DynamicParameters param = null);
        T OneRecord<T>(string procName, DynamicParameters param = null);
        IEnumerable<T> List<T>(string procName, DynamicParameters param = null);
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>
            (string procName, DynamicParameters param = null);

    }
}

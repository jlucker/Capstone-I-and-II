using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace CtrlZDataService
{
    [ServiceContract]
    public interface ICtrlZService
    {
        [OperationContract]
        bool AddTransaction(Transaction t1);

        [OperationContract]
        bool CheckUsername(string s1);

        [OperationContract]
        Users GetUsers();
    }
}

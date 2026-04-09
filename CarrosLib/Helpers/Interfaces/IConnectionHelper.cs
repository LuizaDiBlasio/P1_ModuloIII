using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrosLib.Helpers.Interfaces
{
    public interface IConnectionHelper
    {
        string getConnectionString(string tagRepo);
    }
}

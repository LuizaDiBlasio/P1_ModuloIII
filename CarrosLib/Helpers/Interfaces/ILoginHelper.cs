using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrosLib.DTOs;

namespace CarrosLib.Helpers.Interfaces
{
    public interface ILoginHelper
    {
        int Login(LoginDTO dto, string tagRepo);
    }
}

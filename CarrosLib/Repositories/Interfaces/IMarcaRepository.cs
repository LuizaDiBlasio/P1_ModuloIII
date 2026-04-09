using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrosLib.Models;

namespace CarrosLib.Repositories.Interfaces
{
    public interface IMarcaRepository
    {
        List<Marca> GetAll(string tagRepo);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrosLib.Models
{
    public class Carro
    {
        public int Carro_Id { get; set; }

        public int Marca { get; set; }

        public int Modelo { get; set; }

        public int Ano { get; set; }

        public DateTime? UltimaInspec { get; set; }

        public bool Vendido { get; set; }

    }
}

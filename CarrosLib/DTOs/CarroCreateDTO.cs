using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrosLib.DTOs
{
    public class CarroCreateDTO
    {
        public int Marca_Id { get; set; }

        public int Modelo_Id { get; set; }

        public int Ano { get; set; }

        public bool Vendido { get; set; }

        public DateTime? UltimaInspec { get; set; }

    }
}



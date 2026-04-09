using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrosLib.DTOs
{
    public class CarroDTO
    {
        public int Carro_Id { get; set; }

        public int Marca { get; set; }

        public string? NomeMarca { get; set; }

        public int Modelo { get; set; }

        public string? NomeModelo { get; set; }

        public int Ano { get; set; }

        public DateTime? UltimaInspec { get; set; }

        public bool Vendido { get; set; }
    }
}

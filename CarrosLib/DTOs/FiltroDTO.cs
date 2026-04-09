using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarrosLib.DTOs
{
    public class FiltroDTO
    {
        public int? Marca_Id {  get; set; }

        public int? Modelo_Id { get; set; }  

        public int? Ano { get; set; }    

        public bool? Vendido { get; set; }   
    }
}

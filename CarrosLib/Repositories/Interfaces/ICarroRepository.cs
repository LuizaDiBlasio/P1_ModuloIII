using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrosLib.DTOs;
using CarrosLib.Models;
using Microsoft.Data.SqlClient;

namespace CarrosLib.Repositories.Interfaces
{
    public interface ICarroRepository
    {
        List<CarroDTO> GetAll(string tagRepo);

        List<AnoDTO> GetAllAnos(string tagRepo);

        CarroDTO? GetById(int id, string tagRepo);

        List<CarroDTO> FilterCars(FiltroDTO filtro, string tagRepo);

        int Insert(CarroCreateDTO dto, string tagRepo); 

        void InsertAll(string tagRepo);

        bool Update(int id, CarroCreateDTO dto, string tagRepo);

        bool Delete(int id, string tagRepo);

        bool DeleteAll(string tagRepo);

    }
}

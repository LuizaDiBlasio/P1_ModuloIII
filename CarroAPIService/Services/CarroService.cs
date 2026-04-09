using System.Reflection;
using System.Text.RegularExpressions;
using CarroAPIService.Services.Interfaces;
using CarrosLib.DTOs;
using CarrosLib.Repositories.Interfaces;


namespace CarroAPIService.Services
{
        public class CarroService : ICarroService
        {
            private readonly ICarroRepository _repoCarro;
            private readonly IConfiguration _config;

            public CarroService(ICarroRepository repoCarro, IConfiguration config)
            {
                _repoCarro = repoCarro;
                _config = config;
            }

            public List<CarroDTO> GetAll()
            {
            return _repoCarro.GetAll(_config["Settings:ActiveTag"]);
                    
            }

            public List<AnoDTO> GetAllAnos()
            {
            return _repoCarro.GetAllAnos(_config["Settings:ActiveTag"]);
        }

            public CarroDTO? GetById(int id)
            {
                return _repoCarro.GetById(id, _config["Settings:ActiveTag"]);
             }

            public List<CarroDTO> FilterCars(FiltroDTO filtro)
            {
            return _repoCarro.FilterCars(filtro, _config["Settings:ActiveTag"]);  
            }


            public int Create(CarroCreateDTO dto)
            {
                return _repoCarro.Insert(dto, _config["Settings:ActiveTag"]);
            }

            public void ResetDB()
            {
                _repoCarro.InsertAll(_config["Settings:ActiveTag"]);

            }

        public bool Update(int id, CarroCreateDTO dto)
            {
             return _repoCarro.Update(id, dto, _config["Settings:ActiveTag"]);
            }

            public bool Delete(int id)
            {
               return _repoCarro.Delete(id, _config["Settings:ActiveTag"]);
            }

            public bool DeleteAll()
            {
                 return _repoCarro.DeleteAll(_config["Settings:ActiveTag"]);
            }

        }
    
}

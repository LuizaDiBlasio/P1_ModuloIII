
using CarrosLib.DTOs;

namespace CarroAPIService.Services.Interfaces
{
    public interface ICarroService
    {
        List<CarroDTO> GetAll();

        List<AnoDTO> GetAllAnos();

        CarroDTO GetById(int id);

        List<CarroDTO> FilterCars(FiltroDTO filtro);

        int Create(CarroCreateDTO dto);

        void ResetDB();

        bool Update(int id, CarroCreateDTO dto);

        bool Delete(int id);

        bool DeleteAll();
    }
}


using CarroAPIService.Services.Interfaces;
using CarrosLib.DTOs;
using CarrosLib.Repositories.Interfaces;

namespace CarroAPIService.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly IMarcaRepository _repoMarca;
        private readonly IConfiguration _config;

        public MarcaService(IMarcaRepository marcaRepository, IConfiguration config)
        {
            _repoMarca = marcaRepository;
            _config = config;
        }

        public List<MarcaDTO> GetAll()
        {
            return _repoMarca.GetAll(_config["Settings:ActiveTag"])
                .Select(m => new MarcaDTO
                {
                    Marca_Id = m.Marca_Id,
                    Nome = m.Nome
                }).ToList();
        }
    }
}

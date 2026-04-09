
using CarroAPIService.Services.Interfaces;
using CarrosLib.DTOs;
using CarrosLib.Repositories.Interfaces;

namespace CarroAPIService.Services
{
    public class ModeloService : IModeloService
    {
        private readonly IModeloRepository _repoModelo;
        private readonly IConfiguration _config;

        public ModeloService(IModeloRepository repoModelo, IConfiguration config)
        {
            _repoModelo = repoModelo;
            _config = config;
        }

        public List<ModeloDTO> GetAll()
        {
            return _repoModelo.GetAll(_config["Settings:ActiveTag"])
                .Select(m => new ModeloDTO
                {
                    Modelo_Id = m.Modelo_Id,
                    Nome = m.Nome
                }).ToList();
        }
    }
}


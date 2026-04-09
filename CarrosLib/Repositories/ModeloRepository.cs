using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrosLib.Helpers.Interfaces;
using CarrosLib.Models;
using CarrosLib.Repositories.Interfaces;
using DalProLib;
using Microsoft.Data.SqlClient;

namespace CarrosLib.Repositories
{
    public class ModeloRepository : IModeloRepository
    {
        private readonly IConnectionHelper _connectionHelper;

        public ModeloRepository(IConnectionHelper connection)
        {
            _connectionHelper = connection;
        }
        public List<Modelo> GetAll(string tagRepo)
        {

            string sql = "SELECT * FROM Modelos";
            string connection = _connectionHelper.getConnectionString(tagRepo);

            try
            {
                if (string.IsNullOrEmpty(connection))
                    throw new Exception($"Erro de Configuração: A tag '{tagRepo}' não possui uma ConnectionString válida.");

                DalPro.ConnectionString = connection;

                return DalPro.Query<Modelo>(sql);
            }
            catch (SqlException ex)
            {
                throw new Exception($"Erro de banco de dados: {ex}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado: {ex}");
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrosLib.DTOs;
using CarrosLib.Helpers.Interfaces;
using CarrosLib.Models;
using CarrosLib.Repositories.Interfaces;
using DalProLib;
using Microsoft.Data.SqlClient;



namespace CarrosLib.Repositories
{
    public class CarroRepository : ICarroRepository
    {
        private readonly IConnectionHelper _connectionHelper; 
        public CarroRepository(IConnectionHelper connection)
        {
            _connectionHelper = connection;
        }


        public List<CarroDTO> GetAll(string tagRepo)
        {
            string sql = @"
            SELECT 
                C.Carro_Id, 
                C.Marca,
                M.Nome AS NomeMarca, 
                C.Modelo,
                Mod.Nome AS NomeModelo, 
                C.Ano, 
                C.UltimaInspec, 
                C.Vendido 
            FROM Carros C
            INNER JOIN Marcas M ON M.Marca_Id = C.Marca
            INNER JOIN Modelos Mod ON Mod.Modelo_Id = C.Modelo";

            string connection = _connectionHelper.getConnectionString(tagRepo);

            try 
            {
                if (string.IsNullOrEmpty(connection))
                    throw new Exception($"Erro de Configuração: A tag '{tagRepo}' não possui uma ConnectionString válida.");

                DalPro.ConnectionString = connection;

                return DalPro.Query<CarroDTO>(sql);
            }
            catch(SqlException ex)
            {
                throw new Exception($"Erro de banco de dados: {ex}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro inesperado: {ex}");
            }
        }

        public List<AnoDTO> GetAllAnos(string tagRepo)
        {
            string sql = "SELECT DISTINCT Ano FROM Carros";

            string connection = _connectionHelper.getConnectionString(tagRepo);

            try
            {
                if (string.IsNullOrEmpty(connection))
                    throw new Exception($"Erro de Configuração: A tag '{tagRepo}' não possui uma ConnectionString válida.");

                DalPro.ConnectionString = connection;

                return DalPro.Query<AnoDTO>(sql);
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

        public CarroDTO? GetById(int id, string tagRepo)
        {
            string sql = @" SELECT 
                C.Carro_Id, 
                C.Marca,
                M.Nome AS NomeMarca, 
                C.Modelo,
                Mod.Nome AS NomeModelo, 
                C.Ano, 
                C.UltimaInspec, 
                C.Vendido 
            FROM Carros C
            INNER JOIN Marcas M ON M.Marca_Id = C.Marca
            INNER JOIN Modelos Mod ON Mod.Modelo_Id = C.Modelo 
            WHERE Carro_Id=@id";

            var param = new Dictionary<string, object>
        {
            {"@id", id}
        };

            string connection = _connectionHelper.getConnectionString(tagRepo);

            try
            {
                if (string.IsNullOrEmpty(connection))
                    throw new Exception($"Erro de Configuração: A tag '{tagRepo}' não possui uma ConnectionString válida.");

                DalPro.ConnectionString = connection;

                return DalPro.Query<CarroDTO>(sql, param).FirstOrDefault();
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

 

        public List<CarroDTO> FilterCars(FiltroDTO filtro, string tagRepo)
        {
            string sqlBase = @"
        SELECT 
            c.Carro_Id, 
            c.Marca as Marca, 
            ma.Nome as NomeMarca, 
            c.Modelo as Modelo, 
            mo.Nome as NomeModelo, 
            c.Ano, 
            c.UltimaInspec, 
            c.Vendido
        FROM Carros c
        INNER JOIN Marcas ma ON c.Marca = ma.Marca_Id
        INNER JOIN Modelos mo ON c.Modelo = mo.Modelo_Id
        WHERE 1=1";

            var condicoes = new List<string>();
            var parametros = new Dictionary<string, object>();


            if (filtro.Marca_Id != null)
            {
                sqlBase += " AND c.Marca = @MarcaId";
                parametros.Add("@MarcaId", filtro.Marca_Id);
            }

            if (filtro.Modelo_Id != null)
            {
                sqlBase += " AND c.Modelo = @ModeloId";
                parametros.Add("@ModeloId", filtro.Modelo_Id);
            }

            if (filtro.Ano != null)
            {
                sqlBase += " AND c.Ano = @Ano";
                parametros.Add("@Ano", filtro.Ano);
            }

            if (filtro.Vendido.HasValue)
            {
                sqlBase += " AND c.Vendido = @Vendido";
                parametros.Add("@Vendido", filtro.Vendido.Value);
            }

            var listaVeiculos = new List<CarroDTO>(); 

            string connection = _connectionHelper.getConnectionString(tagRepo);

            try
            {
                if (string.IsNullOrEmpty(connection))
                    throw new Exception($"Erro de Configuração: A tag '{tagRepo}' não possui uma ConnectionString válida.");

                DalPro.ConnectionString = connection;

                return DalPro.Query<CarroDTO>(sqlBase, parametros);
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

        public int Insert(CarroCreateDTO dto, string tagRepo)
        {
            string connection = _connectionHelper.getConnectionString(tagRepo);
            if (string.IsNullOrEmpty(connection))
                throw new Exception($"Erro de conexão com base de dados");

            DalPro.ConnectionString = connection;

            string sql = @"INSERT INTO Carros
            (Marca, Modelo, Ano, UltimaInspec, Vendido)
            VALUES
            (@Marca, @Modelo, @Ano, @UltimaInspec, @Vendido);
            SELECT SCOPE_IDENTITY();";

            var param = new Dictionary<string, object>
            {
                {"@Marca", dto.Marca_Id},
                {"@Modelo", dto.Modelo_Id},
                {"@Ano", dto.Ano},
                {"@UltimaInspec", dto.UltimaInspec},
                {"@Vendido", dto.Vendido ? 1 : 0}
            };


            try
            {
                if (string.IsNullOrEmpty(connection))
                    throw new Exception($"Erro de Configuração: A tag '{tagRepo}' não possui uma ConnectionString válida.");

                DalPro.ConnectionString = connection;

                int id = Convert.ToInt32(DalPro.ExecuteScalar(sql, param)); 

                return id;
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

        public void InsertAll(string tagRepo)
        {
            var trans = DalPro.BeginTransaction();

            string connection = _connectionHelper.getConnectionString(tagRepo);

            string sql = @"INSERT INTO Carros (Marca, Modelo, Ano, UltimaInspec, Vendido)
                            VALUES 
                                (1, 1, 2022, '2024-01-15 10:30:00', 0),
                                (2, 2, 2021, '2023-11-20 14:00:00', 1),
                                (3, 3, 2023, '2024-02-10 09:15:00', 0),
                                (1, 2, 2019, NULL, 1), -- Nulo permitido em UltimaInspec
                                (2, 1, 2020, '2023-05-05 16:45:00', 0)";

            try
            {
                if (string.IsNullOrEmpty(connection))
                    throw new Exception($"Erro de Configuração: A tag '{tagRepo}' não possui uma ConnectionString válida.");

                DalPro.ConnectionString = connection;

                DalPro.Execute(sql);

                DalPro.Commit(trans);
            }
            catch (SqlException ex)
            {
                throw new Exception($"Erro de banco de dados: {ex}");
            }
            catch (Exception ex)
            {
                DalPro.Rollback(trans);
                throw new Exception($"Erro inesperado: {ex}");
            }   
        }

        public bool Update(int id, CarroCreateDTO dto, string tagRepo)
        {
            string connection = _connectionHelper.getConnectionString(tagRepo);

            string sql = @"UPDATE Carros SET
            Marca = @Marca, Modelo = @Modelo, Ano = @Ano, UltimaInspec = @UltimaInspec, Vendido = @Vendido
            WHERE Carro_Id = @Id";

            var param = new Dictionary<string, object>
            {
                {"@Marca", dto.Marca_Id},
                {"@Modelo", dto.Modelo_Id },
                {"@Id", id },
                {"@Ano", dto.Ano },
                {"@UltimaInspec", dto.UltimaInspec },
                {"@Vendido", dto.Vendido }
            };

            try
            {
                if (string.IsNullOrEmpty(connection))
                    throw new Exception($"Erro de Configuração: A tag '{tagRepo}' não possui uma ConnectionString válida.");

                DalPro.ConnectionString = connection;

                int rows = DalPro.Execute(sql, param);

                return rows > 0;
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


        public bool Delete(int id, string tagRepo)
        {
            
            string connection = _connectionHelper.getConnectionString(tagRepo);

            string sql = "DELETE FROM Carros WHERE Carro_Id=@id";

            var param = new Dictionary<string, object>
            {
                {"@id", id}
            };

            try
            {
                if (string.IsNullOrEmpty(connection))
                    throw new Exception($"Erro de Configuração: A tag '{tagRepo}' não possui uma ConnectionString válida.");

                DalPro.ConnectionString = connection;

                int rows = DalPro.Execute(sql, param);

                return rows > 0;
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

        public bool DeleteAll(string tagRepo)
        {

            var trans = DalPro.BeginTransaction();

            string connection = _connectionHelper.getConnectionString(tagRepo);

            string sql = "DELETE FROM Carros";


            try
            {
                if (string.IsNullOrEmpty(connection))
                    throw new Exception($"Erro de Configuração: A tag '{tagRepo}' não possui uma ConnectionString válida.");

                DalPro.ConnectionString = connection;

                int rows = DalPro.Execute(sql, trans: trans);

                DalPro.Commit(trans);

                return rows > 0;
            }
            catch (SqlException ex)
            {
                throw new Exception($"Erro de banco de dados: {ex}");
            }
            catch (Exception ex)
            {
                DalPro.Rollback(trans);
                throw new Exception($"Erro inesperado: {ex}");
            }
        }

    }
}

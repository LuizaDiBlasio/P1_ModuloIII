using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarrosLib.DTOs;
using CarrosLib.Helpers.Interfaces;
using CarrosLib.Models;
using DalProLib;
using Microsoft.Data.SqlClient;

namespace CarrosLib.Helpers
{
    public class LoginHelper : ILoginHelper
    {
        private readonly IConnectionHelper _connectionHelper;

        public LoginHelper(IConnectionHelper connectionHelper)
        {
            _connectionHelper = connectionHelper;
        }

        public int Login(LoginDTO dto, string tagRepo)
        {
            
            string sql = "EXEC LoginUser @UserName, @Password";

            string connection = _connectionHelper.getConnectionString(tagRepo);

            var param = new Dictionary<string, object>
        {
            {"@UserName", dto.Username },
            {"@Password", dto.Password}
        };
           

            try
            {
                if (string.IsNullOrEmpty(connection))
                    throw new Exception($"Erro de Configuração: A tag '{tagRepo}' não possui uma ConnectionString válida.");

                DalPro.ConnectionString = connection;

                var trans = DalPro.BeginTransaction();

                var result = DalPro.ExecuteScalar(sql, param, trans);

                return Convert.ToInt32(result);
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

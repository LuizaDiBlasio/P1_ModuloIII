using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DalProLib
{
    public class DalPro
    {
        public static string ConnectionString; //connection string como propriedade estática global torna acesso mais prático
                                               // TODO não é perigoso colocar a connection string como uma propriedade pública global?

        private static readonly Dictionary<Type, PropertyInfo[]> _cacheProps =
            new Dictionary<Type, PropertyInfo[]>();
        //Usa Reflection para ler as propriedades de uma classe (ex: Territory) e guarda na memória (Cache).
        //Fazer Reflection toda hora é lento. Ao salvar no dicionário, na segunda vez o sistema já sabe quais são as propriedades


        public static SqlConnection GetConnection() //propriedade que cria uma sql connection
        {
            return new SqlConnection(ConnectionString); //Utiliza a propriedade estática ConnectionString
        }

        // --------------------------------------------------
        // CREATE COMMAND - método para criar comando
        // Parametros: sql - string de comando sql
        //             trans - transaction
        //             parameters - parametros do comando sql (se houverem)
        // Parametros trans e parameters são nulls por default, caso não exista simolesmente assume-se que são null
        // --------------------------------------------------
        private static SqlCommand CreateCommand(
            string sql,
            SqlTransaction trans = null,
            Dictionary<string, object> parameters = null) // Um Dicionário é uma coleção genérica armazena pares de chave-valor
                                                          // Neste caso: string - é o nome do parâmetro no SQL (ex: "@id").
                                                          //             object - é o pai de todos os tipos. Possibilita aceitar qualquer coisa (int, string, DateTime, bool etc) 
        {
            SqlCommand cmd;

            if (trans != null) //caso haja transação criar o comando com a transaction e a conexão da transaction
                               //O código usa a conexão que já vem "pendurada" na transação (trans.Connection).
                               //Ele assume que quem criou a transação (o método BeginTransaction) já abriu a conexão.

                cmd = new SqlCommand(sql, trans.Connection, trans);
            else
            {  ///caso não haja transaction, criar conexão e criar comando com a string sql e a conexão
                SqlConnection cn = GetConnection();
                cn.Open(); //devo abrir conexão manualmente caso não haja transaction
                cmd = new SqlCommand(sql, cn);
            }

            if (parameters != null) //se houver parametros, adicionar parametros ao comando  
            {
                //Ele percorre o dicionário de parâmetros e adiciona ao comando SQL,
                //tratando valores nulos com DBNull.Value (o "null" que o banco de dados entende).
                //Isso previne SQL Injection.
                foreach (var p in parameters)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value); //O C# entende null como "vazio na memória".
                                                                                 //O SQL Server entende DBNull como "vazio na tabela".
                                                                                 //Essa linha traduz o "vazio" do C# para o "vazio" do Banco.
            }

            return cmd;
        }

        // --------------------------------------------------
        // EXECUTE NON QUERY
        //Usado para INSERT, UPDATE ou DELETE. Ele retorna o número de linhas afetadas.
        //Uso de using, que garante que o comando seja destruído da memória após o uso.
        //São comandos de modificação, ou seja, não são uma busca (não há query), portanto se tutiliza o comando ExecuteNonQuery()
        // --------------------------------------------------
        public static int Execute(
            string sql,
            Dictionary<string, object> parameters = null,
            SqlTransaction trans = null)
        {
            using SqlCommand cmd = CreateCommand(sql, trans, parameters); //cria o comando

            int result = cmd.ExecuteNonQuery(); //executa o comando

            if (trans == null)
                cmd.Connection.Close(); //fechar conexão manualmente caso não haja transaction

            return result;
        }

        // --------------------------------------------------
        // EXECUTE SCALAR
        //Usado quando se quer apenas um valor (ex: SELECT COUNT(*) ... ou SELECT MAX(Id) ...).
        //Ele retorna apenas a primeira coluna da primeira linha.
        // --------------------------------------------------
        public static object ExecuteScalar(
            string sql,
            Dictionary<string, object> parameters = null,
            SqlTransaction trans = null)
        {
            using SqlCommand cmd = CreateCommand(sql, trans, parameters);

            object result = cmd.ExecuteScalar(); //executa o comando e atribui seu retorno a result

            if (trans == null)
                cmd.Connection.Close();

            return result;
        }

        // --------------------------------------------------
        // QUERY GENERIC
        // Método para realizar queries para qualquer tipo de classe
        // --------------------------------------------------
        public static List<T> Query<T>(
            string sql,
            Dictionary<string, object> parameters = null,
            SqlTransaction trans = null) where T : new() //"Aceita-se qualquer classe, desde que ela tenha um construtor público sem parâmetros"
                                                         // Isso é importante para quando for criado o objeto na linha 114
        {
            List<T> list = new(); //cria lista de objetos T para popular com o que for encontrado na DB

            using SqlCommand cmd = CreateCommand(sql, trans, parameters); //cria o comando
            using SqlDataReader dr = cmd.ExecuteReader(); //ler resultado da query

            PropertyInfo[] props; //É uma variável que guardará a "lista de nomes das propriedades" da sua classe
                                  //Pertence à classe Reflection

            if (!_cacheProps.TryGetValue(typeof(T), out props)) //se a memória não está vazia
            {
                props = typeof(T).GetProperties(); //busca as propriedades da classe T
                _cacheProps[typeof(T)] = props; //Melhoramento de performance. Buscar propriedades via Reflection é lento.
                                                //Aqui, o código guarda na memória (cache)
            }

            //Lê cada linha da DB (dr.Read()).
            while (dr.Read())
            {
                T obj = new T(); //cria um novo objeto (new T())

                foreach (var prop in props)
                {
                    try //tenta encontrar uma coluna na DB que tenha o mesmo nome da propriedade da sua classe (dr.GetOrdinal(prop.Name))
                    {
                        int idx = dr.GetOrdinal(prop.Name); //GetOrdinal busca o índice da coluna de um determinado nome 

                        if (!dr.IsDBNull(idx)) //Verifica se o valor naquela célula do banco não é nulo.
                            prop.SetValue(obj, dr[idx]); //Pega o valor que veio do banco (dr[idx]) e preenche a propriedade do objeto obj.
                    }
                    catch { }
                }

                list.Add(obj); //adiciona objetos criados à lista
            }

            if (trans == null)
                cmd.Connection.Close(); //fecha conexão caso não haja transaction

            return list;
        }

        // --------------------------------------------------
        // DATATABLE FOR UPDATE
        // Carrega dados em uma datatable para depois salvar as alterações.
        // --------------------------------------------------
        public static DataTable DataTableForUpdate(
            string sql,
            ref SqlDataAdapter da,
            SqlTransaction trans = null)
        {
            SqlConnection cn;

            if (trans != null)
                cn = trans.Connection;
            else
            {
                cn = GetConnection();
                cn.Open();
            }

            da = new SqlDataAdapter(sql, cn); //O DataAdapter é a "ponte". Ele pega o comando SQL e a conexão
                                              //e se prepara para buscar os dados e preencher o DataTable.

            if (trans != null)
                da.SelectCommand.Transaction = trans; //indicar ao data adapter qual a transação 

            da.MissingSchemaAction = MissingSchemaAction.AddWithKey; //Indica informações do esquema relacional da DB
                                                                     //Adiciona aos dados, a informação de qual coluna é a Chave Primária (ID)".
                                                                     //Sem isso, o SqlCommandBuilder não sabe qual linha deletar ou atualizar depois.

            SqlCommandBuilder cb = new SqlCommandBuilder(da); //Gera automaticamente os comandos de UPDATE e DELETE baseados no SELECT enviado,
                                                              //não precisa escrever o comando SQL de atualização.

            //Protege nomes de tabelas ou colunas que são palavras reservadas. Coloca a palavra em []
            cb.QuotePrefix = "[";
            cb.QuoteSuffix = "]";

            DataTable dt = new DataTable();
            da.Fill(dt);

            if (trans == null)
                cn.Close();

            return dt;
        }

        // --------------------------------------------------
        // STORED PROCEDURE - Método usado para chamar funções que já estão salvas dentro do servidor SQL,
        //                    retornando os resultados em um DataTable.
        // Parametros : spName - nome sa SP no SqlServer
        //              parameters - dicionário com parametros
        //              trans - transaction
        // --------------------------------------------------
        public static DataTable ExecuteSP(
            string spName,
            Dictionary<string, object> parameters = null,
            SqlTransaction trans = null)
        {
            SqlConnection cn;

            if (trans != null)
                cn = trans.Connection; //caso exista uma transaction, busca a connection através dela
            else
            {
                cn = GetConnection(); //se não busca a conecçao pelo método
                cn.Open();
            }

            SqlCommand cmd = new SqlCommand(spName, cn);
            cmd.CommandType = CommandType.StoredProcedure; //declara qual o tipo de comando sql (SP neste caso)

            if (trans != null)
                cmd.Transaction = trans; //atribui qual a transaction a propriedade Transaction do comando

            if (parameters != null) // caso haja parametros, atribui aos parametros do comando
            {
                foreach (var p in parameters)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt); //preeche o DataAdapter com a datatable criada (criação da tabela em memoria)

            if (trans == null)
                cn.Close(); //fechar conexão caso não haja trasaction

            return dt; //retorna a tabela criada em memória
        }

        //eu que adicionei esse método
        //Executa uma Stored Procedure e retorna um valor único (primeira coluna da primeira linha),
        //ideal para IDs ou contagens.
        public static object ExecuteScalarSP(
           string spName,
           Dictionary<string, object> parameters = null,
           SqlTransaction trans = null)
        {
            SqlConnection cn;

            if (trans != null)
                cn = trans.Connection; //caso exista uma transaction, busca a connection através dela
            else
            {
                cn = GetConnection(); //se não busca a conecçao pelo método
                cn.Open();
            }

            SqlCommand cmd = new SqlCommand(spName, cn);
            cmd.CommandType = CommandType.StoredProcedure; //declara qual o tipo de comando sql (SP neste caso)

            if (trans != null)
                cmd.Transaction = trans; //atribui qual a transaction a propriedade Transaction do comando

            if (parameters != null) // caso haja parametros, atribui aos parametros do comando
            {
                foreach (var p in parameters)
                    cmd.Parameters.AddWithValue(p.Key, p.Value ?? DBNull.Value);
            }

            object result = cmd.ExecuteScalar();

            if (trans == null)
                cn.Close(); //fechar conexão caso não haja trasaction

            return result;
        }



        // --------------------------------------------------
        // TRANSACTIONS
        // --------------------------------------------------
        public static SqlTransaction BeginTransaction() //Inicia esse processo "tudo ou nada"
        {
            SqlConnection cn = GetConnection();
            cn.Open();
            return cn.BeginTransaction(); //retorna a conexão aberta. Guarda a transaction em uma variável no seu Main
                                          //ou Program, para passá-lo como argumento para os outros métodos.
                                          // É como se fosse o "protocolo" da operação.
        }

        public static void Commit(SqlTransaction trans) //Confirma todas as alterações feitas desde o Begin.
        {
            SqlConnection cn = trans.Connection;

            trans.Commit();

            if (cn.State == ConnectionState.Open) // se estiver aberto, fechar a connection
                cn.Close();
        }

        public static void Rollback(SqlTransaction trans) //Cancela tudo e volta ao estado original caso tenha ocorrido um erro.
        {
            SqlConnection cn = trans.Connection;

            trans.Rollback();

            if (cn.State == ConnectionState.Open)
                cn.Close();
        }
    }
}


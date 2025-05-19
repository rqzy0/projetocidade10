
using projetocidade10.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace projetocidade10.Repositorio
{
    public class ProdutoRepositorio(IConfiguration configuration)
    {
        private readonly string _conexaoMySQL = configuration.GetConnectionString("ConexaoMySQL");

        public void Cadastrar(Produto produto)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("insert into Produto(Nome,Descricao,Preco,Quantidade) values (@nome, @descricao, @preco, @quantidade)", conexao);
                cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = produto.Nome;
                cmd.Parameters.Add("@descricao", MySqlDbType.VarChar).Value = produto.Descricao;
                cmd.Parameters.Add("@preco", MySqlDbType.Decimal).Value = produto.Preco;
                cmd.Parameters.Add("@quantidade", MySqlDbType.Int32).Value = produto.Quantidade;
                cmd.ExecuteNonQuery();

                conexao.Close();
            }
        }

        public bool Atualizar(Produto produto)
        {
            try
            {
                using (var conexao = new MySqlConnection(_conexaoMySQL))
                {
                    conexao.Open();

                    MySqlCommand cmd = new MySqlCommand("Update Produto set Nome=@nome, Descricao=@descricao, Preco=@preco, Quantidade=@quantidade where Id=@codigo", conexao);
                    cmd.Parameters.Add("@codigo", MySqlDbType.Int32).Value = produto.Id;
                    cmd.Parameters.Add("@nome", MySqlDbType.VarChar).Value = produto.Nome;
                    cmd.Parameters.Add("@descricao", MySqlDbType.VarChar).Value = produto.Descricao;
                    cmd.Parameters.Add("@preco", MySqlDbType.Decimal).Value = produto.Preco;
                    cmd.Parameters.Add("@quantidade", MySqlDbType.Int32).Value = produto.Quantidade;

                    int linhasAfetadas = cmd.ExecuteNonQuery();
                    return linhasAfetadas > 0;
                }
            }

            catch (MySqlException ex)
            {
                Console.WriteLine($"Erro ao atualizar produto: {ex.Message}");
                return false;
            }
        }

        public IEnumerable<Produto> TodosProdutos()
        {
            List<Produto> ProdutoLista = new List<Produto>();

            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select * from Produto", conexao);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                conexao.Close();

                foreach (DataRow dr in dt.Rows)
                {
                    ProdutoLista.Add(
                        new Produto
                        {
                            Id = Convert.ToInt32(dr["Id"]),
                            Nome = ((string)dr["Nome"]),
                            Descricao = ((string)dr["Descricao"]),
                            Preco = Convert.ToDecimal(dr["Preco"]),
                            Quantidade = Convert.ToInt32(dr["Quantidade"]),
                        });
                }
                return ProdutoLista;
            }
        }

        public Produto ObterProduto(int codigo)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("select * from Produto where Id = @codigo", conexao);
                cmd.Parameters.AddWithValue("@codigo", codigo);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                MySqlDataReader dr;
                Produto produto = new Produto();

                dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                while (dr.Read())
                {
                    produto.Id = Convert.ToInt32(dr["Id"]);
                    produto.Nome = (string)(dr["Nome"]);
                    produto.Descricao = (string)(dr["Descricao"]);
                    produto.Preco = Convert.ToDecimal(dr["Preco"]);
                    produto.Quantidade = Convert.ToInt32(dr["Quantidade"]);
                }
                return produto;
            }
        }

        public void Excluir(int Id)
        {
            using (var conexao = new MySqlConnection(_conexaoMySQL))
            {
                conexao.Open();

                MySqlCommand cmd = new MySqlCommand("delete from Produto where Id = @codigo", conexao);
                cmd.Parameters.AddWithValue("@codigo", Id);
                int linhasAfetadas = cmd.ExecuteNonQuery();

                conexao.Close();
            }
        }
    }
}

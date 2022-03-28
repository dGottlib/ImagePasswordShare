using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework51_ImageSharePassword.Data
{
    public class ImagesRepository
    {
        private string _connectionString;

        public ImagesRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Upload(string filePath, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO Images (Password, ImagePath, Views)
                                VALUES(@password, @path, @views)
                                SELECT SCOPE_IDENTITY()";
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@path", filePath);
            cmd.Parameters.AddWithValue("@views", 0);
            connection.Open();
            return (int)(decimal)cmd.ExecuteScalar();
        }
        public Image GetImage(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM Images WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            var reader = cmd.ExecuteReader();
            reader.Read();
            //if (reader == null)
            //{
            //    return null;
            //}

            return new Image
            {
                Id = (int)reader["Id"],
                Password = (string)reader["Password"],
                ImagePath = (string)reader["ImagePath"],
                Views = (int)reader["Views"]
            };
        }  
        public void IncrementView(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"UPDATE Images SET Views = Views + 1
                                WHERE Id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            cmd.ExecuteNonQuery();
        }

    }
}

using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CoursWork
{
    internal class DataBase
    {
        public MySqlConnection SqlConnection { get; set; }
        public List<ForeignKey> foreignKeys { get; set; } = new();
        public ObservableCollection<TestClass> TestTest { get; set; } = new();
        public bool Config() 
        {
            MySqlConnectionStringBuilder MSBuilder = new MySqlConnectionStringBuilder();
            MSBuilder.Server = "192.168.200.13";
            MSBuilder.UserID = "student";
            MSBuilder.Password = "student";
            MSBuilder.Database = "1125_2025_Nosikov_Ilya";
            MSBuilder.CharacterSet = "utf8mb4";
            SqlConnection = new(MSBuilder.ToString());

            return SqlConnection != null;
        }


        public ObservableCollection<T> SelectAll<T>()
        {
            if (SqlConnection == null)
            {
                if (!Config())
                {
                    MessageBox.Show("connection is null");
                    return new ObservableCollection<T>();
                }
            }
            SqlConnection.Open();
            ObservableCollection<T> list = new ObservableCollection<T>();
            var type = typeof(T);
            //var attribute = type.Assembly.GetName().Name;
            var attribute = type.GetCustomAttributes(typeof(MySqlTableAttribute), false).FirstOrDefault() as MySqlTableAttribute;
            var props = type.GetProperties();
            string table = attribute.TableName;
            using (MySqlCommand mySqlCommand = new($"select * from `{table}`", SqlConnection))
            using (MySqlDataReader dataReader = mySqlCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    T row = (T)Activator.CreateInstance(type);
                    list.Add(row);
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        string column = dataReader.GetName(i);
                        foreach (var prop in props)
                            if (column != null && !dataReader.IsDBNull(i))
                            {
                                var attributeProp = prop.GetCustomAttributes(typeof(MySqlColumnAttribute), false).FirstOrDefault() as MySqlColumnAttribute;
                                if (attributeProp.ColumnName == column)
                                {
                                    prop.SetValue(row, dataReader.GetValue(i));
                                    break;
                                }
                            }
                    }
                }
            }
            SqlConnection.Close();
            return list;
        }

        public bool Insert<T>(T row) where T : BaseDataClass
        {
            SqlConnection.Open();
            bool result = false;
            var type = row.GetType();
            var attribute = type.GetCustomAttributes(typeof(MySqlTableAttribute), false).FirstOrDefault() as MySqlTableAttribute;
            var props = type.GetProperties();
            StringBuilder sb = new StringBuilder($"insert into `{attribute.TableName}` (");
            List<string> columns = new();
            foreach (var prop in props)
            {
                var attributeProps = prop.GetCustomAttributes(typeof(MySqlColumnAttribute), false).FirstOrDefault() as MySqlColumnAttribute;
                if (attributeProps != null)
                    columns.Add(attributeProps.ColumnName);
            }
            sb.Append(string.Join(",", columns));
            sb.Append(") values (@");
            sb.Append(string.Join(",@", columns));
            sb.Append(");select LAST_INSERT_ID();");
            string sqlCommand = sb.ToString();
            using (var MySqlCmd = new MySqlCommand(sqlCommand, SqlConnection))
            {
                foreach (var prop in props)
                {
                    var attributeProp = prop.GetCustomAttributes(typeof(MySqlColumnAttribute), false).FirstOrDefault() as MySqlColumnAttribute;
                    if (attributeProp != null)
                        MySqlCmd.Parameters.Add(new MySqlParameter(attributeProp.ColumnName, prop.GetValue(row)));
                }
                int id = (int)(ulong)MySqlCmd.ExecuteScalar();
                if (id != 0)
                    result = (row.ID = id) != 0;
            }

            SqlConnection.Close();
            return result;
        }
        public bool Delete<T>(T row) where T : BaseDataClass
        {
            SqlConnection.Open();
            bool result = false;
            var type = row.GetType();
            var attribute = type.GetCustomAttributes(typeof(MySqlTableAttribute), false).FirstOrDefault() as MySqlTableAttribute;
            using (var MySqlCmd = new MySqlCommand($"Delete from `{attribute.TableName}` where `Id` = {row.ID}", SqlConnection))
                result = MySqlCmd.ExecuteNonQuery() != 0;
            SqlConnection.Close();
            return result;
        }
        public bool GetFKs<T>(T row) where T : BaseDataClass
        {
            SqlConnection.Open();
            bool result = false;
            var type = row.GetType();
            var attribute = type.GetCustomAttributes(typeof(MySqlTableAttribute), false).FirstOrDefault() as MySqlTableAttribute;
            using (var MySqlCmd = new MySqlCommand($"SELECT TABLE_NAME,COLUMN_NAME, REFERENCED_TABLE_NAME, REFERENCED_COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE REFERENCED_TABLE_SCHEMA = '{SqlConnection.Database}' AND REFERENCED_TABLE_NAME = '{attribute.TableName}';", SqlConnection))
            using (var DR = MySqlCmd.ExecuteReader())
                while (DR.Read())
                {
                    string tableName = string.Empty;
                    string columnName = string.Empty;
                    string referencedTableName = string.Empty;
                    string referencedColumnName = string.Empty;
                    if (!DR.IsDBNull(0))
                        tableName = DR.GetString(0);
                    else
                        return result;
                    if (!DR.IsDBNull(1))
                        columnName = DR.GetString(1);
                    else
                        return result;
                    if (!DR.IsDBNull(2))
                        referencedTableName = DR.GetString(2);
                    else
                        return result;
                    if (!DR.IsDBNull(3))
                        referencedColumnName = DR.GetString(3);
                    ForeignKey FK = new ForeignKey { TableName = tableName, ColumnName = columnName, ReferencedTableName = referencedTableName, ReferencedColumnName = referencedColumnName };
                    if (!foreignKeys.Contains(FK))
                        foreignKeys.Add(FK);
                }
            SqlConnection.Close();
            return result;
        }
        public struct ForeignKey
        {
            public string TableName { get; set; } //where foreign key
            public string ColumnName { get; set; } //name of foreign key
            public string ReferencedTableName { get; set; }
            public string ReferencedColumnName { get; set; }

        }


        static DataBase instance;
        public static DataBase DB 
        {
            get { if (instance != null) return instance; else instance = new DataBase(); return instance; }
        } 
    }
}

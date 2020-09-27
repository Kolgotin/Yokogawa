using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LogCollectorLibrary
{
    /// <summary>
    /// класс для взаимодействия с Базой данных
    /// </summary>
    public class DBMethods
    {
        DataTable Table = new DataTable();

        string ConnectionString;
        string DBName = "Yokogawa";

        public DBMethods(string сonnectionString)
        {
            ConnectionString = сonnectionString;
            CreateTable();
        }

        public List<ControllersAndStations> ReadControllersAndStationsTable()
        {
            try
            {
                List<ControllersAndStations> controllersAndStationsList = new List<ControllersAndStations>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT [ControllersAndStationsId],[ControllersAndStationsName] FROM [" +
                        DBName + "].[dbo].[LogControllersAndStationsTable]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                controllersAndStationsList.Add(new ControllersAndStations()
                                {
                                    ControllersAndStationsId = dataReader.GetInt32(0),
                                    ControllersAndStationsName = dataReader.GetString(1).Trim()
                                });
                            }
                        }
                    }
                }
                return controllersAndStationsList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Writers> ReadWriterTable()
        {
            try
            {
                List<Writers> writersList = new List<Writers>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT [WriterId],[WriterName] FROM [" + DBName + "].[dbo].[LogWriterTable]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                writersList.Add(new Writers()
                                {
                                    WriterId = dataReader.GetInt32(0),
                                    WriterName = dataReader.GetString(1).Trim()
                                });
                            }
                        }
                    }
                }
                return writersList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<UnknownColumn1> ReadUnknownColumn1Table()
        {
            try
            {
                List<UnknownColumn1> unknownColumn1List = new List<UnknownColumn1>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT [UnknownColumn1Id],[UnknownColumn1Type] FROM [" + DBName + "].[dbo].[LogUnknownColumn1Table]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                unknownColumn1List.Add(new UnknownColumn1()
                                {
                                    UnknownColumn1Id = dataReader.GetInt32(0),
                                    UnknownColumn1Type = dataReader.GetString(1).Trim()
                                });
                            }
                        }
                    }
                }
                return unknownColumn1List;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<ProductType> ReadProductTypeTable()
        {
            try
            {
                List<ProductType> productTypeList = new List<ProductType>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"SELECT [ProductTypeId],[ProductName] FROM [" + DBName + "].[dbo].[ProductTypeTable]";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataReader dataReader = command.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                productTypeList.Add(new ProductType()
                                {
                                    ProductTypeId = dataReader.GetInt32(0),
                                    ProductName = dataReader.GetString(1).Trim()
                                });
                            }
                        }
                    }
                }
                return productTypeList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Вставляет в таблицу ProductType запись value, возвращает новый ProductTypeId
        /// Возвращает ProductTypeId=0 если запись вставлена не была
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int InsertProductType(string value)
        {
            int productTypeId;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query;

                    query = "INSERT INTO [dbo].[ProductTypeTable]([ProductName])" +
                        " VALUES (@ProductName); SELECT SCOPE_IDENTITY()";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductName", value);
                        productTypeId = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                productTypeId = 0;
            }
            return productTypeId;
        }

        /// <summary>
        /// Вставляет в таблицу WriterTable запись value, возвращает новый WriterId
        /// Возвращает WriterId=0 если запись вставлена не была
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int InsertWriter(string value)
        {
            int writerId;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query;

                    query = "INSERT INTO [dbo].[LogWriterTable]([WriterName]) VALUES (@Writer); SELECT SCOPE_IDENTITY()";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Writer", value);
                        writerId = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                writerId = 0;
            }
            return writerId;
        }

        /// <summary>
        /// Вставляет в таблицу ControllersAndStationsTable запись value, возвращает новый ControllersAndStationsId
        /// Возвращает ControllersAndStationsId=0 если запись вставлена не была
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int InsertControllersAndStations(string value)
        {
            int controllersAndStationsId;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query;

                    query = "INSERT INTO [dbo].[LogControllersAndStationsTable]([ControllersAndStationsName])" +
                        " VALUES (@ControllersAndStations); SELECT SCOPE_IDENTITY()";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ControllersAndStations", value);
                        controllersAndStationsId = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                controllersAndStationsId = 0;
            }
            return controllersAndStationsId;
        }

        /// <summary>
        /// Вставляет в таблицу UnknownColumn1Table запись value, возвращает новый UnknownColumn1Id
        /// Возвращает UnknownColumn1Id=0 если запись вставлена не была
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int InsertUnknownColumn1(string value)
        {
            int unknownColumn1Id;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query;

                    query = "INSERT INTO [dbo].[LogUnknownColumn1Table]([UnknownColumn1Type])" +
                        " VALUES (@UnknownColumn1Type); SELECT SCOPE_IDENTITY()";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UnknownColumn1Type", value);
                        unknownColumn1Id = Convert.ToInt32(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                unknownColumn1Id = 0;
            }
            return unknownColumn1Id;
        }

        void CreateTable()
        {
            DataColumn column;

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "Id";
            column.AutoIncrement = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "ProductTypeId";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.DateTime");
            column.ColumnName = "DateTimeUMTEvent";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "UnknownColumn1Id";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "TypeEvent";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "IindexNumberEvent";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "WriterId";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.DateTime");
            column.ColumnName = "ExactDateTime";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.DateTime");
            column.ColumnName = "DateUMTEvent";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.TimeSpan");
            column.ColumnName = "TimeUMTEvent";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "UnknownColumn2";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "UnknownColumn3";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.Int32");
            column.ColumnName = "ControllersAndStationsId";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "PositionOrAlarmOrBlockName";
            column.ReadOnly = true;
            Table.Columns.Add(column);

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "MessageText";
            column.ReadOnly = true;
            Table.Columns.Add(column);
        }

        public int BulkCopyInsert(int productTypeId, List<YokogawaLog> logReaderCurerntDay)
        {
            Table.Rows.Clear();
            //LogReaderViewModelCollection.ForEach(currentParsedLine =>
            logReaderCurerntDay.Distinct().ToList().ForEach(currentParsedLine =>
            {
                DataRow row;
                row = Table.NewRow();
                row["ProductTypeId"] = productTypeId;
                row["DateTimeUMTEvent"] = currentParsedLine.DateTimeUMTEvent;
                if (currentParsedLine.UnknownColumn1Id != 0)
                    row["UnknownColumn1Id"] = currentParsedLine.UnknownColumn1Id;
                row["TypeEvent"] = currentParsedLine.TypeEvent;
                row["IindexNumberEvent"] = currentParsedLine.IindexNumberEvent;
                if (currentParsedLine.WriterId != 0)
                    row["WriterId"] = currentParsedLine.WriterId;
                row["ExactDateTime"] = currentParsedLine.ExactDateTime;
                row["DateUMTEvent"] = currentParsedLine.ExactDateTime.Date;
                row["TimeUMTEvent"] = currentParsedLine.ExactDateTime.TimeOfDay;
                row["UnknownColumn2"] = currentParsedLine.UnknownColumn2;
                row["UnknownColumn3"] = currentParsedLine.UnknownColumn3;
                if (currentParsedLine.ControllersAndStationsId != 0)
                    row["ControllersAndStationsId"] = currentParsedLine.ControllersAndStationsId;
                if (currentParsedLine.PositionOrAlarmOrBlockName == "")
                    row["PositionOrAlarmOrBlockName"] = currentParsedLine.PositionOrAlarmOrBlockName;
                row["MessageText"] = currentParsedLine.MessageText;
                Table.Rows.Add(row);
            });

            int addedRows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "dbo.LogYokogawa";
                        bulkCopy.BatchSize = 10000;
                        bulkCopy.WriteToServer(Table);
                    }
                }
                addedRows = Table.Rows.Count;
            }
            catch (Exception ex)
            {
                MessageShowMethod.ShowMethod(ex.Message);
            }
            return addedRows;
        }

        /// <summary>
        /// для построчной вставки чтобы найти ошибку
        /// </summary>
        /// <param name="productTypeId"></param>
        /// <param name="logReaderCurerntDay"></param>
        /// <returns></returns>
        private int SqlQueryInsert(int productTypeId, List<YokogawaLog> logReaderCurerntDay)
        {
            int addedRows = 0;
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command;
                logReaderCurerntDay.ForEach(x =>
                {
                    string zapros = "INSERT INTO [LogYokogawa] ([ProductTypeId],[DateTimeUMTEvent],[UnknownColumn1Id]" +
                    ",[TypeEvent],[IindexNumberEvent],[WriterId],[ExactDateTime],[DateUMTEvent],[TimeUMTEvent]" +
                    ",[UnknownColumn2],[UnknownColumn3],[ControllersAndStationsId],[PositionOrAlarmOrBlockName],[MessageText])" +
                    " VALUES (@ProductTypeId,@DateTimeUMTEvent,@UnknownColumn1Id,@TypeEvent" +
                    ",@IindexNumberEvent,@WriterId,@ExactDateTime,@DateUMTEvent,@TimeUMTEvent,@UnknownColumn2" +
                    ",@UnknownColumn3,@ControllersAndStationsId,@PositionOrAlarmOrBlockName,@MessageText)";

                    command = new SqlCommand(zapros, connection);
                    command.Parameters.AddWithValue($"ProductTypeId", productTypeId);
                    command.Parameters.AddWithValue($"DateTimeUMTEvent", x.DateTimeUMTEvent);
                    command.Parameters.AddWithValue($"UnknownColumn1Id", x.UnknownColumn1Id);
                    command.Parameters.AddWithValue($"TypeEvent", x.TypeEvent);
                    command.Parameters.AddWithValue($"IindexNumberEvent", x.IindexNumberEvent);
                    command.Parameters.AddWithValue($"WriterId", x.WriterId);
                    command.Parameters.AddWithValue($"ExactDateTime", x.ExactDateTime);
                    command.Parameters.AddWithValue($"DateUMTEvent", x.ExactDateTime.Date);
                    command.Parameters.AddWithValue($"TimeUMTEvent", x.ExactDateTime.TimeOfDay);
                    command.Parameters.AddWithValue($"UnknownColumn2", x.UnknownColumn2);
                    command.Parameters.AddWithValue($"UnknownColumn3", x.UnknownColumn3);
                    command.Parameters.AddWithValue($"ControllersAndStationsId", x.ControllersAndStationsId);
                    command.Parameters.AddWithValue($"PositionOrAlarmOrBlockName", x.PositionOrAlarmOrBlockName);
                    command.Parameters.AddWithValue($"MessageText", x.MessageText);

                    command.ExecuteNonQuery();
                    addedRows++;
                });
            }

            return addedRows;
        }
    }
}

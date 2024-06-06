using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace SistemaDeVentas.Datos
{
    public class ProductoDAO
    {
        // Cadena de conexion a la base de datos
        public static string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)" +
                                           "(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SID=XE)))" +
                                            ";User Id=USER_VENTAS;Password=1234;";

        // Este metodo sirve para agregar un producto nuevo a la base de datos
        public void InsertarProducto(string nombre, string descripcion, decimal precio, int stock)
        {
            // Abrimos la conexion a la base de datos usando la cadena de conexion
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Comando para ejecutar el procedimiento almacenado "SP_ABM_PRODUCTOS"
                using (OracleCommand command = new OracleCommand("SP_ABM_PRODUCTOS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregamos los parametros que necesita el procedimiento
                    command.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = "A";
                    // Ya que es un producto nuevo el ID se deja en nulo
                    command.Parameters.Add("P_ID_PRODUCTO", OracleDbType.Int32).Value = DBNull.Value;
                    // Datos del nuevo producto
                    command.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = nombre;
                    command.Parameters.Add("P_DESCRIPCION", OracleDbType.Varchar2).Value = descripcion;
                    command.Parameters.Add("P_PRECIO", OracleDbType.Decimal).Value = precio;
                    command.Parameters.Add("P_STOCK", OracleDbType.Int32).Value = stock;

                    // Ejecutamos el comando para agregar el nuevo producto
                    command.ExecuteNonQuery();
                }
            }
        }

        // Metodo para modifcar los datos de un producto existe
        public void ModificarProducto(int idProducto, string nombre, string descripcion, decimal precio, int stock)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Comando para ejecutar el procedimiento "SP_ABM_PRODUCTOS"
                using (OracleCommand command = new OracleCommand("SP_ABM_PRODUCTOS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // La accion "M" significa que queremos modificar un producto
                    command.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = "M";
                    // Se pasa el ID del producto que vamos a modifcar
                    command.Parameters.Add("P_ID_PRODUCTO", OracleDbType.Int32).Value = idProducto;
                    // datos del producto
                    command.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = nombre;
                    command.Parameters.Add("P_DESCRIPCION", OracleDbType.Varchar2).Value = descripcion;
                    command.Parameters.Add("P_PRECIO", OracleDbType.Decimal).Value = precio;
                    command.Parameters.Add("P_STOCK", OracleDbType.Int32).Value = stock;

                    // Ejecutamos para modificar el producto
                    command.ExecuteNonQuery();
                }
            }
        }

        // Metodo para eliminar un producto de la base de datos
        public void EliminarProducto(int idProducto)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("SP_ABM_PRODUCTOS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // La accion "B" significa que queremos eliminar un producto
                    command.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = "B";
                    // Pasamos el ID del producto que vamos a eliminar
                    command.Parameters.Add("P_ID_PRODUCTO", OracleDbType.Int32).Value = idProducto;
                    //Como los otros parametros no los necesitamos para eliminar, tendran valor nulo
                    command.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = DBNull.Value;
                    command.Parameters.Add("P_DESCRIPCION", OracleDbType.Varchar2).Value = DBNull.Value;
                    command.Parameters.Add("P_PRECIO", OracleDbType.Decimal).Value = DBNull.Value;
                    command.Parameters.Add("P_STOCK", OracleDbType.Int32).Value = DBNull.Value;

                    // Ejecutamos para eliminar el producto
                    command.ExecuteNonQuery();
                }
            }
        }

        // Este metodo sirve para obtener la lista de todos los productos que estan en la base de datos
        public DataTable ObtenerProductos()
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("SP_LISTAR_PRODUCTOS", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    DataTable dtProductos = new DataTable();
                    using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                    {
                        adapter.Fill(dtProductos);
                    }

                    return dtProductos;
                }
            }
        }
    }
}
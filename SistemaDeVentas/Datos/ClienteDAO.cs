using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace SistemaDeVentas.Datos
{
    internal class ClienteDAO
    {
        // se reutiliza la cadena de conexión de ProductoDAO.cs
        private static string connectionString = ProductoDAO.connectionString;

        // Metodo para obtener la lista de clientes desde la base de datos
        public DataTable ObtenerClientes()
        {
           
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Comando para ejecutar el procedimiento almacenado "SP_LISTAR_CLIENTES"
                using (OracleCommand command = new OracleCommand("SP_LISTAR_CLIENTES", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("P_CURSOR", OracleDbType.RefCursor).Direction = ParameterDirection.Output;

                    // Creamos un DataTable para almacenar los clientes obtenidos
                    DataTable dtClientes = new DataTable();

                    // Ejecutamos el comando y leemos los datos 
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        // Cargamos los datos del reader en el DataTable
                        dtClientes.Load(reader);
                    }

                    // Devolvemos el DataTable con los clientes
                    return dtClientes;
                }
            }
        }

        // Método para agregar un nuevo cliente a la base de datos
        public void AgregarCliente(string nombre, string apellido, string email, string telefono, string direccion)
        {
           
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Comando para ejecutar el procedimiento almacenado "SP_ABM_CLIENTES"
                using (OracleCommand command = new OracleCommand("SP_ABM_CLIENTES", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregamos los parámetros necesarios para agregar un nuevo cliente
                    command.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = "A"; // "A" = alta (agregar)
                    command.Parameters.Add("P_ID_CLIENTE", OracleDbType.Int32).Value = DBNull.Value; 
                    command.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = nombre;
                    command.Parameters.Add("P_APELLIDO", OracleDbType.Varchar2).Value = apellido;
                    command.Parameters.Add("P_EMAIL", OracleDbType.Varchar2).Value = email;
                    command.Parameters.Add("P_TELEFONO", OracleDbType.Varchar2).Value = telefono;
                    command.Parameters.Add("P_DIRECCION", OracleDbType.Varchar2).Value = direccion;

                    // Ejecutamos el comando para agregar el nuevo cliente
                    command.ExecuteNonQuery();
                }
            }
        }

        // Método para modificar un cliente existente en la base de datos
        public void ModificarCliente(int idCliente, string nombre, string apellido, string email, string telefono, string direccion)
        {
            
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Comando para ejecutar el procedimiento almacenado "SP_ABM_CLIENTES"
                using (OracleCommand command = new OracleCommand("SP_ABM_CLIENTES", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregamos los parámetros necesarios para modificar un cliente existente
                    command.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = "M"; // "M" = modificación
                    command.Parameters.Add("P_ID_CLIENTE", OracleDbType.Int32).Value = idCliente; // se pasa el ID del cliente a modificar
                    command.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = nombre;
                    command.Parameters.Add("P_APELLIDO", OracleDbType.Varchar2).Value = apellido;
                    command.Parameters.Add("P_EMAIL", OracleDbType.Varchar2).Value = email;
                    command.Parameters.Add("P_TELEFONO", OracleDbType.Varchar2).Value = telefono;
                    command.Parameters.Add("P_DIRECCION", OracleDbType.Varchar2).Value = direccion;

                    // Ejecutamos el comando para modificar el cliente
                    command.ExecuteNonQuery();
                }
            }
        }

        // Método para eliminar un cliente de la base de datos
        public void EliminarCliente(int idCliente)
        {
            
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                // Comando para ejecutar el procedimiento almacenado "SP_ABM_CLIENTES"
                using (OracleCommand command = new OracleCommand("SP_ABM_CLIENTES", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Agregamos los parámetros necesarios para eliminar un cliente
                    command.Parameters.Add("P_ACCION", OracleDbType.Varchar2).Value = "B"; // "B") = baja (eliminar)
                    command.Parameters.Add("P_ID_CLIENTE", OracleDbType.Int32).Value = idCliente; // se pasa el ID del cliente a eliminar
                    command.Parameters.Add("P_NOMBRE", OracleDbType.Varchar2).Value = DBNull.Value; 
                    command.Parameters.Add("P_APELLIDO", OracleDbType.Varchar2).Value = DBNull.Value;
                    command.Parameters.Add("P_EMAIL", OracleDbType.Varchar2).Value = DBNull.Value;
                    command.Parameters.Add("P_TELEFONO", OracleDbType.Varchar2).Value = DBNull.Value;
                    command.Parameters.Add("P_DIRECCION", OracleDbType.Varchar2).Value = DBNull.Value;

                    // Ejecutamos el comando para eliminar el cliente
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
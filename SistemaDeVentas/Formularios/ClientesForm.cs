using SistemaDeVentas.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaDeVentas.Formularios
{
    public partial class ClientesForm : Form
    {
        public ClientesForm()
        {
            InitializeComponent();
        }

        // Cuando carga el formulario
        private void ClientesForm_Load(object sender, EventArgs e)
        {
            ActualizarGrillaClientes();
            // se oculta el campo del ID del cliente
            txtIdCliente.Visible = false;
        }

        // Metodo para validar el formato del email
        private bool ValidarEmail(string email)
        {
            string pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, pattern);
        }

        // Metodo para validar que el teléfono contenga solo dígitos
        private bool ValidarTelefono(string telefono)
        {
            return telefono.All(char.IsDigit);
        }

        // Metodo para actualizar la grilla con los clientes
        private void ActualizarGrillaClientes()
        {
            ClienteDAO clienteDAO = new ClienteDAO();
            dgvClientes.DataSource = clienteDAO.ObtenerClientes();
        }

        // Metodo para limpiar los campos de texto
        private void LimpiarCampos()
        {
            txtIdCliente.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
        }

        // Cuando hacemos clic en el boton Agregar
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // se valida que el nombre no esté vacío
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El campo Nombre es requerido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // se valida el apellido no esté vacío
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El campo Apellido es requerido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // se valida que el email tenga un formato válido
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !ValidarEmail(txtEmail.Text))
            {
                MessageBox.Show("Ingrese un email válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // se valida que el teléfono contenga solo dígitos
            if (string.IsNullOrWhiteSpace(txtTelefono.Text) || !ValidarTelefono(txtTelefono.Text))
            {
                MessageBox.Show("Ingrese un número de teléfono válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // se valida que la dirección no esté vacía
            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MessageBox.Show("El campo Dirección es requerido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Se toman los datos de los campos de texto
            string nombre = txtNombre.Text;
            string apellido = txtApellido.Text;
            string email = txtEmail.Text;
            string telefono = txtTelefono.Text;
            string direccion = txtDireccion.Text;

            // Si pasan las validaciones, creamos un objeto ClienteDAO y lo usamos para agregar el cliente
            ClienteDAO clienteDAO = new ClienteDAO();
            clienteDAO.AgregarCliente(nombre, apellido, email, telefono, direccion);

            // Limpiamos los campos de texto
            LimpiarCampos();

            // Actualizamos la grilla con la lista de clientes
            ActualizarGrillaClientes();
        }

        // Cuando hacemos clic en el boton Modificar
        private void btnModificar_Click(object sender, EventArgs e)
        {
            // se valida que se haya seleccionado un cliente en la grilla
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Debe seleccionar un cliente para modificar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // se valida que el nombre no esté vacío
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El campo Nombre es requerido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // se valida que el apellido no esté vacío
            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("El campo Apellido es requerido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // se valida que el email tenga un formato válido
            if (!ValidarEmail(txtEmail.Text))
            {
                MessageBox.Show("Ingrese un email válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // se valida que el teléfono contenga solo dígitos (si no está vacío)
            if (!string.IsNullOrWhiteSpace(txtTelefono.Text) && !ValidarTelefono(txtTelefono.Text))
            {
                MessageBox.Show("Ingrese un número de teléfono válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // se valida que la dirección no esté vacía
            if (string.IsNullOrWhiteSpace(txtDireccion.Text))
            {
                MessageBox.Show("El campo Dirección es requerido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tomamos los datos de los campos de texto 
            int idCliente = Convert.ToInt32(txtIdCliente.Text);
            string nombre = txtNombre.Text;
            string apellido = txtApellido.Text;
            string email = txtEmail.Text;
            string telefono = txtTelefono.Text;
            string direccion = txtDireccion.Text;

            // Si pasan las validaciones, creamos un objeto ClienteDAO y lo usamos para modificar el cliente
            ClienteDAO clienteDAO = new ClienteDAO();
            clienteDAO.ModificarCliente(idCliente, nombre, apellido, email, telefono, direccion);

            // Limpiamos los campos de texto
            LimpiarCampos();

            // Actualizamos la grilla con la lista de clientes
            ActualizarGrillaClientes();
        }

        // Cuando hacemos clic en una celda de la grilla
        private void dgvClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Si se hizo clic en una fila válida
            if (e.RowIndex >= 0)
            {
                // Tomamos los datos de la fila seleccionada
                DataGridViewRow filaSeleccionada = dgvClientes.Rows[e.RowIndex];

                // Llenamos los campos de texto con los datos de la fila seleccionada
                txtIdCliente.Text = filaSeleccionada.Cells["ID_CLIENTE"].Value.ToString();
                txtNombre.Text = filaSeleccionada.Cells["NOMBRE"].Value.ToString();
                txtApellido.Text = filaSeleccionada.Cells["APELLIDO"].Value.ToString();
                txtEmail.Text = filaSeleccionada.Cells["EMAIL"].Value.ToString();
                txtTelefono.Text = filaSeleccionada.Cells["TELEFONO"].Value.ToString();
                txtDireccion.Text = filaSeleccionada.Cells["DIRECCION"].Value.ToString();

                // se muestra el campo del ID del cliente
                txtIdCliente.Visible = true;
            }
        }

        // Cuando hacemos clic en el boton Eliminar
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // se valida que se haya seleccionado un cliente en la grilla
            if (dgvClientes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Debe seleccionar un cliente para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Mostramos un cuadro de diálogo de confirmación antes de eliminar el cliente
            if (MessageBox.Show("¿Está seguro de eliminar el cliente seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Obtenemos el ID del cliente seleccionado
                int idCliente = Convert.ToInt32(dgvClientes.SelectedRows[0].Cells["ID_CLIENTE"].Value);

                // Creamos una instancia de ClienteDAO y llamamos al método EliminarCliente
                ClienteDAO clienteDAO = new ClienteDAO();
                clienteDAO.EliminarCliente(idCliente);

                // Limpiamos los campos de texto después de eliminar el cliente
                LimpiarCampos();

                // Actualizamos la grilla con la lista de clientes
                ActualizarGrillaClientes();
            }
        }
    }
}
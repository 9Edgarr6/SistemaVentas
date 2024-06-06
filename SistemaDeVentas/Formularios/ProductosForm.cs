using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SistemaDeVentas.Datos;

namespace SistemaDeVentas.Formularios
{
    public partial class ProductosForm : Form
    {
        public ProductosForm()
        {
            InitializeComponent();
        }

        // Cuando carga el formulario
        private void ProductosForm_Load(object sender, EventArgs e)
        {
            ActualizarGrillaProductos();
            // Ocultamos el campo del ID del producto
            txtIdProducto.Visible = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

     
        private void lblProductos_Click(object sender, EventArgs e)
        {

        }

        // Metodo para actualizar la grilla con los productos
        private void ActualizarGrillaProductos()
        {
            ProductoDAO productoDAO = new ProductoDAO();
            dgvProductos.DataSource = productoDAO.ObtenerProductos();
        }

        // Cuando hacemos clic en el boton Agregar
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            // Validamos que el nombre no este vacio
            //Esta validacion es para asegurarnos que el usuario
            // ingrese un nombre para el producto, ya que el nombre es un campo obligatorio
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre del producto es requerido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tomamos los datos de los campos de texto
            string nombre = txtNombre.Text;
            string descripcion = txtDescripcion.Text;

            // Validamos que el precio sea mayor a cero
            //para evitar que se ingresen precios negativos
            if (decimal.TryParse(txtPrecio.Text, out decimal precio) && precio <= 0)
            {
                MessageBox.Show("El precio del producto debe ser mayor a cero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

           
            // Validamos que el stock sea mayor o igual a cero
            // El stock no puede ser negativo
            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("El stock del producto debe ser mayor o igual a cero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Si pasan las validaciones creamos un objeto ProductoDAO y lo usamos para agregar el producto
            ProductoDAO productoDAO = new ProductoDAO();
            productoDAO.InsertarProducto(nombre, descripcion, precio, stock);

            // Limpiamos los campos de texto
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            txtStock.Clear();

            // Actualizamos la grilla con la lista de productos
            ActualizarGrillaProductos();
        }

        // Cuando hacemos clic en el boton Modificar
        private void btnModificar_Click(object sender, EventArgs e)
        {
            // Validamos que el nombre no este vacio
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre del producto es requerido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Tomamos los datos de los campos de texto, incluyendo el ID
            int idProducto = Convert.ToInt32(txtIdProducto.Text);
            string nombre = txtNombre.Text;
            string descripcion = txtDescripcion.Text;

            // Validamos que el precio sea mayor a cero
            if (decimal.TryParse(txtPrecio.Text, out decimal precio) && precio <= 0)
            {
                MessageBox.Show("El precio del producto debe ser mayor a cero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Validamos que el stock sea mayor o igual a cero
            if (!int.TryParse(txtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("El stock del producto debe ser mayor o igual a cero.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Si pasan las validaciones creamos un objeto ProductoDAO y lo usamos para modificar el producto
            ProductoDAO productoDAO = new ProductoDAO();
            productoDAO.ModificarProducto(idProducto, nombre, descripcion, precio, stock);

            // Limpiamos los campos de texto
            txtIdProducto.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtPrecio.Clear();
            txtStock.Clear();

            // Actualizamos la grilla con la lista de productos
            ActualizarGrillaProductos();
        }

        // Cuando hacemos clic en una celda de la grilla
        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Si se hizo clic en una fila valida
            if (e.RowIndex >= 0)
            {
                // Tomamos los datos de la fila seleccionada
                DataGridViewRow filaSeleccionada = dgvProductos.Rows[e.RowIndex];

                // Mostramos el campo del ID del producto
                txtIdProducto.Visible = true;

                // Llenamos los campos de texto con los datos de la fila seleccionada
                txtIdProducto.Text = filaSeleccionada.Cells["ID_PRODUCTO"].Value.ToString();
                txtNombre.Text = filaSeleccionada.Cells["NOMBRE"].Value.ToString();
                txtDescripcion.Text = filaSeleccionada.Cells["DESCRIPCION"].Value.ToString();
                txtPrecio.Text = filaSeleccionada.Cells["PRECIO"].Value.ToString();
                txtStock.Text = filaSeleccionada.Cells["STOCK"].Value.ToString();
            }
        }

        // Cuando hacemos clic en el boton Eliminar
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // Validar que se haya seleccionado un producto
            if (dgvProductos.SelectedRows.Count == 0)
            {
                MessageBox.Show("Debe seleccionar un producto para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Confirmar la eliminación del producto
            if (MessageBox.Show("¿Está seguro de eliminar el producto seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // Obtener el ID del producto seleccionado
                int idProducto = Convert.ToInt32(dgvProductos.SelectedRows[0].Cells["ID_PRODUCTO"].Value);

                // Crear instancia de ProductoDAO y llamar al método EliminarProducto
                ProductoDAO productoDAO = new ProductoDAO();
                productoDAO.EliminarProducto(idProducto);

                // Limpiar los campos de texto después de eliminar el producto
                txtIdProducto.Clear();
                txtNombre.Clear();
                txtDescripcion.Clear();
                txtPrecio.Clear();
                txtStock.Clear();

                // Actualizar la grilla con la lista de productos
                ActualizarGrillaProductos();
            }
        }
    }
}
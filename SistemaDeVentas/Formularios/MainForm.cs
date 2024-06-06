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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnProductos_Click(object sender, EventArgs e)
        {
            ProductosForm formProductos = new ProductosForm();
            formProductos.Show();
        }

        private void btnClientes_Click(object sender, EventArgs e)
        {
            ClientesForm formClientes = new ClientesForm();
            formClientes.Show();
        }
    }
}

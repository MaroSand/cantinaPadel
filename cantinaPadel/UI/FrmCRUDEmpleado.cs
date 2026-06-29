using System;
using System.Windows.Forms;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.UI
{
    public partial class FrmCRUDEmpleado : Form
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        private Empleado? _empleadoEdicion;

        // Constructor para cuando es un empleado nuevo
        public FrmCRUDEmpleado()
        {
            InitializeComponent();
            _empleadoRepository= new EmpleadoRepository();
            _empleadoEdicion = null;
            this.Text = "Nuevo Empleado";
        }

        // Constructor para modificar empleado existente
        public FrmCRUDEmpleado(Empleado empleado) : this()
        {
            _empleadoEdicion= empleado;
            this.Text = "Modificar Empleado";
        }
    }
}

using System.Drawing;
using System.Windows.Forms;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;

namespace cantinaPadel.UI;

public sealed class FrmReporteHorasPadel : Form
{
    private sealed class FilaCancelacion
    {
        public int IdTurno { get; init; }
        public string Cliente { get; init; } = "";
        public string Modalidad { get; init; } = "";
        public string Cancha { get; init; } = "";
        public string Horario { get; init; } = "";
        public int Cantidad { get; init; }
        public DateTime UltimaCancelacion { get; init; }
        public string Cancelo { get; init; } = "";
        public List<InstanciaTurno> Instancias { get; init; } = new();
    }

    private readonly DateTimePicker desde = new() { Format = DateTimePickerFormat.Short };
    private readonly DateTimePicker hasta = new() { Format = DateTimePickerFormat.Short };
    private readonly DataGridView grilla = new();
    private readonly Label resumen = new() { AutoSize = true, Padding = new Padding(8) };
    private List<FilaCancelacion> filas = new();
    private List<InstanciaTurno> datos = new();

    public FrmReporteHorasPadel()
    {
        Text = "Historial de turnos cancelados";
        BackColor = SystemColors.Info;
        Font = new Font("Segoe UI", 9F);
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1, Padding = new Padding(10) };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
        var filtros = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false, Padding = new Padding(0, 7, 0, 0) };
        filtros.Controls.Add(new Label { Text = "Canceladas desde", AutoSize = true, Padding = new Padding(0, 7, 4, 0) });
        filtros.Controls.Add(desde);
        filtros.Controls.Add(new Label { Text = "hasta", AutoSize = true, Padding = new Padding(8, 7, 4, 0) });
        filtros.Controls.Add(hasta);
        var buscar = new Button { Text = "Buscar", Width = 85, Height = 27, Margin = new Padding(10, 0, 0, 0) };
        filtros.Controls.Add(buscar);
        desde.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        hasta.Value = DateTime.Today;
        buscar.Click += (_, _) => Cargar();

        grilla.Dock = DockStyle.Fill;
        grilla.ReadOnly = true;
        grilla.AllowUserToAddRows = false;
        grilla.AllowUserToDeleteRows = false;
        grilla.RowHeadersVisible = false;
        grilla.MultiSelect = false;
        grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grilla.AutoGenerateColumns = false;
        grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        AddColumn(nameof(FilaCancelacion.Cliente), "Cliente", 18);
        AddColumn(nameof(FilaCancelacion.Modalidad), "Reserva", 10);
        AddColumn(nameof(FilaCancelacion.Cancha), "Cancha", 10);
        AddColumn(nameof(FilaCancelacion.Horario), "Horario", 13);
        AddColumn(nameof(FilaCancelacion.Cantidad), "Turnos cancelados", 10);
        AddColumn(nameof(FilaCancelacion.UltimaCancelacion), "Última cancelación", 18, "dd/MM/yyyy HH:mm");
        AddColumn(nameof(FilaCancelacion.Cancelo), "Canceló", 18);
        grilla.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex < 0 || grilla.Rows[e.RowIndex].DataBoundItem is not FilaCancelacion fila) return;
            using var detalle = new FrmHistorialTurnosCancelados(fila.Cliente, fila.Instancias);
            detalle.ShowDialog(this);
        };

        root.Controls.Add(filtros, 0, 0);
        root.Controls.Add(grilla, 0, 1);
        root.Controls.Add(resumen, 0, 2);
        Controls.Add(root);
        Cargar();
    }

    private void AddColumn(string property, string title, float weight, string? format = null)
    {
        grilla.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = property, HeaderText = title, FillWeight = weight,
            SortMode = DataGridViewColumnSortMode.NotSortable,
            DefaultCellStyle = format == null ? new DataGridViewCellStyle() : new DataGridViewCellStyle { Format = format }
        });
    }

    private void Cargar()
    {
        if (desde.Value.Date > hasta.Value.Date)
        {
            MessageBox.Show(this, "La fecha inicial no puede ser posterior a la fecha final.", "Rango inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        try
        {
            datos = new ReporteTurnosCanceladosRepository().ObtenerCancelaciones();
            var desdeHora = desde.Value.Date;
            var hastaHora = hasta.Value.Date.AddDays(1);
            filas = datos.Where(i => i.FechaHoraCancelacion >= desdeHora && i.FechaHoraCancelacion < hastaHora)
                .GroupBy(i => i.IdTurno)
                .Select(g =>
                {
                    var primero = g.OrderBy(i => i.Fecha).First();
                    var ultima = g.Max(i => i.FechaHoraCancelacion!.Value);
                    var actor = g.OrderByDescending(i => i.FechaHoraCancelacion).First().EmpleadoCancelo;
                    return new FilaCancelacion
                    {
                        IdTurno = g.Key,
                        Cliente = $"{primero.TurnoReservado.Cliente.Persona.Nombre} {primero.TurnoReservado.Cliente.Persona.Apellido}",
                        Modalidad = primero.TurnoReservado.Modalidad == TurnoReservado.ModalidadFijo ? "Fija" : "Por día",
                        Cancha = primero.HorarioCancha.Cancha?.Nombre ?? "",
                        Horario = $"{primero.HorarioCancha.HoraInicio:hh\\:mm}–{primero.HorarioCancha.HoraFin:hh\\:mm}",
                        Cantidad = g.Count(),
                        UltimaCancelacion = ultima,
                        Cancelo = actor == null ? "Usuario no disponible" : $"{actor.Persona.Nombre} {actor.Persona.Apellido} ({actor.NombreUsuario})",
                        Instancias = datos.Where(i => i.IdTurno == g.Key).OrderBy(i => i.Fecha).ToList()
                    };
                }).OrderByDescending(f => f.UltimaCancelacion).ToList();
            PaginadorGrilla.Mostrar(grilla, filas);
            resumen.Text = $"{filas.Count} reservas · {filas.Sum(f => f.Cantidad)} turnos cancelados. Doble clic para ver el historial completo de la reserva.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"No se pudo cargar el historial de turnos cancelados.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

public sealed class FrmHistorialTurnosCancelados : Form
{
    public FrmHistorialTurnosCancelados(string cliente, IReadOnlyList<InstanciaTurno> instancias)
    {
        Text = $"Cancelaciones - {cliente}";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(950, 560);
        MinimumSize = new Size(760, 420);
        BackColor = SystemColors.Info;
        Font = new Font("Segoe UI", 9F);
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 1, Padding = new Padding(12) };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        root.Controls.Add(new Label { Text = $"Historial de turnos cancelados\n{cliente} · {instancias.Count} fechas", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 12F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
        var grid = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoGenerateColumns = false, BackgroundColor = Color.White };
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Detalle.Fecha), HeaderText = "Fecha del turno", FillWeight = 17, DefaultCellStyle = new DataGridViewCellStyle { Format = "dddd dd/MM/yyyy" } });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Detalle.Horario), HeaderText = "Horario", FillWeight = 12 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Detalle.Cancha), HeaderText = "Cancha", FillWeight = 12 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Detalle.FechaCancelacion), HeaderText = "Cancelado el", FillWeight = 19, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" } });
        grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = nameof(Detalle.Actor), HeaderText = "Canceló", FillWeight = 28 });
        PaginadorGrilla.Mostrar(grid, instancias.Select(i => new Detalle
        {
            Fecha = i.Fecha,
            Horario = $"{i.HorarioCancha.HoraInicio:hh\\:mm}–{i.HorarioCancha.HoraFin:hh\\:mm}",
            Cancha = i.HorarioCancha.Cancha?.Nombre ?? "",
            FechaCancelacion = i.FechaHoraCancelacion,
            Actor = i.EmpleadoCancelo == null ? "Usuario no disponible" : $"{i.EmpleadoCancelo.Persona.Nombre} {i.EmpleadoCancelo.Persona.Apellido} ({i.EmpleadoCancelo.NombreUsuario})"
        }).ToList());
        root.Controls.Add(grid, 0, 1);
        var cerrar = new Button { Text = "Cerrar", Dock = DockStyle.Right, Width = 105, Height = 34 };
        cerrar.Click += (_, _) => Close();
        root.Controls.Add(cerrar, 0, 2);
        Controls.Add(root);
    }

    private sealed class Detalle
    {
        public DateTime Fecha { get; init; }
        public string Horario { get; init; } = "";
        public string Cancha { get; init; } = "";
        public DateTime? FechaCancelacion { get; init; }
        public string Actor { get; init; } = "";
    }
}

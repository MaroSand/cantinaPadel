using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace cantinaPadel.UI;

/// <summary>Pages a grid in groups of ten while keeping the complete filtered set in memory.</summary>
internal sealed class PaginadorGrilla
{
    private const int TamanoPagina = 10;
    private static readonly ConditionalWeakTable<DataGridView, PaginadorGrilla> Instancias = new();
    private readonly DataGridView _grilla;
    private readonly Label _estado = new() { AutoSize = true, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Padding = new Padding(8, 5, 8, 0) };
    private readonly Button _anterior = new() { Text = "‹ Anterior", AutoSize = true, Height = 28 };
    private readonly Button _siguiente = new() { Text = "Siguiente ›", AutoSize = true, Height = 28 };
    private List<object> _elementos = new();
    private Func<IEnumerable<object>, object>? _crearPaginaTipada;
    private int _pagina;
    private bool _barraInstalada;

    private PaginadorGrilla(DataGridView grilla)
    {
        _grilla = grilla;
        InstalarBarra();
        _anterior.Click += (_, _) => { if (_pagina > 0) { _pagina--; MostrarPagina(); } };
        _siguiente.Click += (_, _) => { if (_pagina + 1 < TotalPaginas) { _pagina++; MostrarPagina(); } };
    }

    public static void Mostrar<T>(DataGridView grilla, IEnumerable<T> elementos)
    {
        if (!Instancias.TryGetValue(grilla, out var paginador))
        {
            paginador = new PaginadorGrilla(grilla);
            Instancias.Add(grilla, paginador);
        }
        paginador._elementos = elementos.Cast<object>().ToList();
        paginador._crearPaginaTipada = pagina => pagina.Cast<T>().ToList();
        paginador._pagina = 0;
        paginador.MostrarPagina();
    }

    public static void Limpiar(DataGridView grilla)
    {
        if (!Instancias.TryGetValue(grilla, out var paginador))
        {
            paginador = new PaginadorGrilla(grilla);
            Instancias.Add(grilla, paginador);
        }
        paginador._elementos.Clear();
        paginador._pagina = 0;
        paginador.MostrarPagina();
    }

    public static void Refrescar(DataGridView grilla)
    {
        if (Instancias.TryGetValue(grilla, out var paginador)) paginador.MostrarPagina();
    }

    private int TotalPaginas => Math.Max(1, (int)Math.Ceiling(_elementos.Count / (double)TamanoPagina));

    private void MostrarPagina()
    {
        var elementosPagina = _elementos.Skip(_pagina * TamanoPagina).Take(TamanoPagina);
        _grilla.DataSource = _crearPaginaTipada?.Invoke(elementosPagina);
        _estado.Text = _elementos.Count == 0
            ? "Sin registros"
            : $"Página {_pagina + 1} de {TotalPaginas} · {_elementos.Count} registros";
        _anterior.Enabled = _pagina > 0;
        _siguiente.Enabled = _pagina + 1 < TotalPaginas;
    }

    private void InstalarBarra()
    {
        if (_barraInstalada) return;
        var grilla = _grilla;
        var padre = grilla.Parent;
        if (padre == null)
        {
            grilla.ParentChanged -= GrillaParentChanged;
            grilla.ParentChanged += GrillaParentChanged;
            return;
        }
        grilla.ParentChanged -= GrillaParentChanged;
        var indiceControl = padre.Controls.GetChildIndex(grilla);
        var celda = padre is TableLayoutPanel tabla ? tabla.GetCellPosition(grilla) : new TableLayoutPanelCellPosition(-1, -1);
        var columnaSpan = padre is TableLayoutPanel tlp ? tlp.GetColumnSpan(grilla) : 1;
        var filaSpan = padre is TableLayoutPanel tlp2 ? tlp2.GetRowSpan(grilla) : 1;
        var bounds = grilla.Bounds;
        var dock = grilla.Dock;
        var anchor = grilla.Anchor;
        var margin = grilla.Margin;
        var host = new Panel { Dock = dock, Anchor = anchor, Bounds = bounds, Margin = margin, Padding = Padding.Empty };
        padre.Controls.Remove(grilla);
        padre.Controls.Add(host);
        padre.Controls.SetChildIndex(host, Math.Min(indiceControl, padre.Controls.Count - 1));
        if (padre is TableLayoutPanel layout && celda.Row >= 0 && celda.Column >= 0)
        {
            layout.SetCellPosition(host, celda);
            layout.SetColumnSpan(host, columnaSpan);
            layout.SetRowSpan(host, filaSpan);
        }
        var contenedor = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = System.Drawing.SystemColors.Info
        };
        contenedor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contenedor.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        contenedor.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        contenedor.Controls.Add(grilla, 0, 0);
        grilla.Dock = DockStyle.Fill;
        grilla.Margin = new Padding(0);

        var barra = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = new Padding(6, 2, 6, 2),
            BackColor = System.Drawing.SystemColors.Info
        };
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        barra.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        barra.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        _anterior.Dock = DockStyle.Fill;
        _anterior.MinimumSize = new Size(106, 30);
        _siguiente.Dock = DockStyle.Fill;
        _siguiente.MinimumSize = new Size(106, 30);
        _estado.Dock = DockStyle.Fill;
        barra.Controls.Add(_anterior, 0, 0);
        barra.Controls.Add(_estado, 1, 0);
        barra.Controls.Add(_siguiente, 2, 0);
        contenedor.Controls.Add(barra, 0, 1);
        host.Controls.Add(contenedor);
        _barraInstalada = true;
    }

    private void GrillaParentChanged(object? sender, EventArgs e)
    {
        if (_grilla.Parent != null) InstalarBarra();
    }
}

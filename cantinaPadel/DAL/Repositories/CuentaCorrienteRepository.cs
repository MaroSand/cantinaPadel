using cantinaPadel.BLL;
using cantinaPadel.Models;
using Microsoft.EntityFrameworkCore;

namespace cantinaPadel.DAL.Repositories;

public class CuentaCorrienteRepository : ICuentaCorrienteRepository
{
    public List<ItemDeudaCliente> ObtenerPendientes(int idCliente)
    {
        using var ctx = new AppDbContext();
        return ConciliacionCuentaCorriente.ConsultarPendientes(ctx, idCliente)
            .Select(p => p.Item)
            .ToList();
    }

    public ResumenCuentaCorriente ObtenerResumen(int idCliente)
    {
        using var ctx = new AppDbContext();

        // Se lee el saldo de la base (no del objeto que tenga la pantalla) para no mostrar un valor desactualizado
        var cliente = ctx.Clientes.AsNoTracking().FirstOrDefault(c => c.IdCliente == idCliente)
            ?? throw new InvalidOperationException("No se encontró el cliente.");

        return new ResumenCuentaCorriente
        {
            Pendientes = ConciliacionCuentaCorriente.ConsultarPendientes(ctx, idCliente)
                .Select(p => p.Item)
                .ToList(),
            Credito = Math.Max(cliente.SaldoCuentaCorriente, 0m)
        };
    }

    public ResultadoPagoCuentaCorriente RegistrarPago(int idCliente, decimal monto, int idCaja, int idEmpleado)
    {
        if (monto <= 0)
            throw new ArgumentException("El monto a cobrar debe ser mayor a cero.");
        monto = Math.Round(monto, 2);

        using var ctx = new AppDbContext();
        using var transaccion = ctx.Database.BeginTransaction();

        var cliente = ctx.Clientes.Find(idCliente)
            ?? throw new InvalidOperationException("No se encontró el cliente.");

        var pendientes = ConciliacionCuentaCorriente.ConsultarPendientes(ctx, idCliente);
        ValidarQueNoSuperaLaDeuda(monto, pendientes, cliente);

        // El pago se suma al crédito previo y se aplica a las unidades más viejas: solo se saldan las que quedan cubiertas por completo
        cliente.SaldoCuentaCorriente = Math.Max(cliente.SaldoCuentaCorriente, 0m) + monto;
        var (saldados, siguenPendientes) = ConciliacionCuentaCorriente.Aplicar(cliente, pendientes);

        ctx.MovimientosCuentaCorriente.Add(new MovimientoCuentaCorriente
        {
            IdCliente = idCliente,
            IdCaja = idCaja,
            IdEmpleado = idEmpleado,
            Fecha = DateTime.Now,
            Tipo = MovimientoCuentaCorriente.TipoPago,
            Monto = monto,
            SaldoPosterior = cliente.SaldoCuentaCorriente
        });

        ctx.SaveChanges();
        transaccion.Commit();

        return new ResultadoPagoCuentaCorriente
        {
            MontoRecibido = monto,
            ItemsPagados = saldados,
            ItemsPendientes = siguenPendientes,
            CreditoResultante = cliente.SaldoCuentaCorriente
        };
    }

    // Cobrar más de lo que se debe generaría un "saldo a favor" que no corresponde. La pantalla ya limita el monto pero la regla se valida
    // acá porque la deuda pudo cambiar (otra terminal, cambio de precio)
    private static void ValidarQueNoSuperaLaDeuda(decimal monto, IReadOnlyList<PendienteCliente> pendientes, Cliente cliente)
    {
        decimal deudaNeta = CalculadorCuentaCorriente.CalcularDeudaNeta(
            pendientes.Sum(p => p.Item.Monto),
            Math.Max(cliente.SaldoCuentaCorriente, 0m));

        if (deudaNeta == 0m)
            throw new InvalidOperationException("El cliente no tiene deuda pendiente en cuenta corriente.");

        if (monto > deudaNeta)
            throw new InvalidOperationException(
                $"El monto ({monto:C2}) supera la deuda pendiente del cliente ({deudaNeta:C2}).");
    }
}

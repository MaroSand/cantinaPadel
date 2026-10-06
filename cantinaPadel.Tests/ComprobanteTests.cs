using cantinaPadel.BLL;
using cantinaPadel.DAL.Repositories;
using cantinaPadel.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cantinaPadel.Tests
{
    // Pruebas unitarias de LogicaComprobante (US-15)
    // No se testean acá los caminos Imprimir/Email de ConfirmarEmision: dependen de recursos del sistema operativo (impresora / SMTP) y no
    // tienen sentido en un test unitario. Lo que sí se cubre es todo lo que es lógica propia: numeración correlativa y validaciones
    [TestClass]
    public class ComprobanteTests
    {
        [TestMethod]
        public void ConfirmarEmision_PrimerComprobanteDeUnTipo_ArrancaEnUno()
        {
            var repo = new ComprobanteRepositoryFake();
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante { IdVenta = 1, Total = 1000m };

            var comprobante = logica.ConfirmarEmision(datos, TipoComprobante.Ticket, FormaEntrega.NoEmitir);

            Assert.AreEqual(1, comprobante.Numero);
        }

        [TestMethod]
        public void ConfirmarEmision_YaHayComprobantesDelMismoTipo_ContinuaElCorrelativo()
        {
            var repo = new ComprobanteRepositoryFake();
            repo.Cargar(new Comprobante { Tipo = TipoComprobante.Ticket, PuntoVenta = 1, Numero = 7 });
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante { IdVenta = 1, Total = 500m };

            var comprobante = logica.ConfirmarEmision(datos, TipoComprobante.Ticket, FormaEntrega.NoEmitir);

            Assert.AreEqual(8, comprobante.Numero);
        }

        [TestMethod]
        public void ConfirmarEmision_TiposDistintos_NumeranPorSeparado()
        {
            var repo = new ComprobanteRepositoryFake();
            repo.Cargar(new Comprobante { Tipo = TipoComprobante.Ticket, PuntoVenta = 1, Numero = 50 });
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante
            {
                IdVenta = 1,
                Total = 500m,
                CondicionIvaCliente = "Responsable Inscripto",
                CuitCliente = "20-12345678-9"
            };

            var facturaA = logica.ConfirmarEmision(datos, TipoComprobante.FacturaA, FormaEntrega.NoEmitir);

            Assert.AreEqual(1, facturaA.Numero);
        }

        [TestMethod]
        public void ConfirmarEmision_FormaEntregaEmailSinEmailCliente_LanzaExcepcion()
        {
            var repo = new ComprobanteRepositoryFake();
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante { IdVenta = 1, Total = 500m, EmailCliente = null };

            Assert.ThrowsExactly<ArgumentException>(
                () => logica.ConfirmarEmision(datos, TipoComprobante.Ticket, FormaEntrega.Email));
        }

        [TestMethod]
        public void ConfirmarEmision_NoEmitir_PersisteElComprobanteIgual()
        {
            var repo = new ComprobanteRepositoryFake();
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante { IdVenta = 42, Total = 1500m };

            logica.ConfirmarEmision(datos, TipoComprobante.FacturaB, FormaEntrega.NoEmitir);

            Assert.AreEqual(1, repo.Agregados.Count);
            Assert.AreEqual(42, repo.Agregados[0].IdVenta);
            Assert.AreEqual(FormaEntrega.NoEmitir, repo.Agregados[0].FormaEntrega);
        }

        [DataTestMethod]
        [DataRow(TipoComprobante.FacturaA)]
        [DataRow(TipoComprobante.FacturaB)]
        [DataRow(TipoComprobante.FacturaC)]
        public void ConfirmarEmision_CuentaCorrienteConFactura_LanzaExcepcion(TipoComprobante tipoFactura)
        {
            var repo = new ComprobanteRepositoryFake();
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante { IdVenta = 1, Total = 500m, MetodoPago = "Cuenta Corriente" };

            Assert.ThrowsExactly<ArgumentException>(
                () => logica.ConfirmarEmision(datos, tipoFactura, FormaEntrega.NoEmitir));
        }

        [TestMethod]
        public void ConfirmarEmision_CuentaCorrienteConRemito_Permite()
        {
            var repo = new ComprobanteRepositoryFake();
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante { IdVenta = 1, Total = 500m, MetodoPago = "Cuenta Corriente" };

            var comprobante = logica.ConfirmarEmision(datos, TipoComprobante.Remito, FormaEntrega.NoEmitir);

            Assert.AreEqual(TipoComprobante.Remito, comprobante.Tipo);
        }

        [TestMethod]
        public void GenerarTexto_Remito_ListaProductosSinPreciosYConTotalEstimado()
        {
            var logica = new LogicaComprobante(new ComprobanteRepositoryFake());
            var datos = new DatosVentaParaComprobante
            {
                IdVenta = 1,
                Total = 3000m,
                DeudaEstimada = 2750m,
                MetodoPago = "Cuenta Corriente",
                Items = new List<DetalleComprobante>
                {
                    new() { Nombre = "Coca Cola 500ml", Cantidad = 2, PrecioUnitario = 1500m }
                }
            };
            var comprobante = logica.ConfirmarEmision(datos, TipoComprobante.Remito, FormaEntrega.NoEmitir);

            string texto = logica.GenerarTexto(comprobante, datos);

            StringAssert.Contains(texto, "2 x Coca Cola 500ml");
            StringAssert.Contains(texto, "TOTAL ESTIMADO A PAGAR");
            StringAssert.Contains(texto, "estimativo");
            // No se muestran precios por producto ni el total bruto de la venta
            Assert.IsFalse(texto.Contains("1500") || texto.Contains("1.500") || texto.Contains("3000") || texto.Contains("3.000"));
        }

        [TestMethod]
        public void GenerarTexto_RemitoConPagoACuenta_MuestraLaCuentaCompletaSinDecirSaldoAFavor()
        {
            var logica = new LogicaComprobante(new ComprobanteRepositoryFake());
            var datos = new DatosVentaParaComprobante
            {
                IdVenta = 1,
                MetodoPago = "Cuenta Corriente",
                DeudaEstimada = 5201m,
                CreditoRestante = 9803m,
                Items = new List<DetalleComprobante>
                {
                    new() { Nombre = "Tubo de Pelotas", Cantidad = 1, PrecioUnitario = 13000m },
                    new() { Nombre = "Agua Mineral", Cantidad = 1, PrecioUnitario = 1000m }
                }
            };
            var comprobante = logica.ConfirmarEmision(datos, TipoComprobante.Remito, FormaEntrega.NoEmitir);

            string texto = logica.GenerarTexto(comprobante, datos);

            StringAssert.Contains(texto, "Deuda estimada");
            StringAssert.Contains(texto, "Pagos a cuenta");
            StringAssert.Contains(texto, "SALDO ESTIMADO");
            StringAssert.Contains(texto, "estimativos");
            Assert.IsFalse(texto.ToLower().Contains("saldo a favor"));
        }

        [TestMethod]
        public void GenerarTexto_RemitoConSaldoAFavorReal_LoIndica()
        {
            var logica = new LogicaComprobante(new ComprobanteRepositoryFake());
            var datos = new DatosVentaParaComprobante
            {
                IdVenta = 1,
                MetodoPago = "Cuenta Corriente",
                Total = 1000m,
                SaldoFavor = 500m,
                Items = new List<DetalleComprobante> { new() { Nombre = "Agua", Cantidad = 1, PrecioUnitario = 1000m } }
            };
            var comprobante = logica.ConfirmarEmision(datos, TipoComprobante.Remito, FormaEntrega.NoEmitir);

            StringAssert.Contains(logica.GenerarTexto(comprobante, datos), "Saldo a favor");
        }

        [TestMethod]
        public void ConfirmarEmision_FacturaAConsumidorFinal_LanzaExcepcion()
        {
            var repo = new ComprobanteRepositoryFake();
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante { IdVenta = 1, Total = 500m, NombreCliente = "Consumidor Final" };

            Assert.ThrowsExactly<ArgumentException>(
                () => logica.ConfirmarEmision(datos, TipoComprobante.FacturaA, FormaEntrega.NoEmitir));
        }

        [DataTestMethod]
        [DataRow("Monotributista")]
        [DataRow("IVA Exento")]
        [DataRow(null)]
        public void ConfirmarEmision_FacturaAClienteNoResponsableInscripto_LanzaExcepcion(string? condicionIva)
        {
            var repo = new ComprobanteRepositoryFake();
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante
            {
                IdVenta = 1,
                Total = 500m,
                NombreCliente = "Juan Pérez",
                CondicionIvaCliente = condicionIva,
                CuitCliente = "20-12345678-9"
            };

            Assert.ThrowsExactly<ArgumentException>(
                () => logica.ConfirmarEmision(datos, TipoComprobante.FacturaA, FormaEntrega.NoEmitir));
        }

        [TestMethod]
        public void ConfirmarEmision_FacturaAResponsableInscriptoSinCuit_LanzaExcepcion()
        {
            var repo = new ComprobanteRepositoryFake();
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante
            {
                IdVenta = 1,
                Total = 500m,
                NombreCliente = "Juan Pérez",
                CondicionIvaCliente = "Responsable Inscripto",
                CuitCliente = null
            };

            Assert.ThrowsExactly<ArgumentException>(
                () => logica.ConfirmarEmision(datos, TipoComprobante.FacturaA, FormaEntrega.NoEmitir));
        }

        [TestMethod]
        public void ConfirmarEmision_FacturaAResponsableInscriptoConCuit_Permite()
        {
            var repo = new ComprobanteRepositoryFake();
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante
            {
                IdVenta = 1,
                Total = 500m,
                NombreCliente = "Juan Pérez",
                CondicionIvaCliente = "Responsable Inscripto",
                CuitCliente = "20-12345678-9"
            };

            var comprobante = logica.ConfirmarEmision(datos, TipoComprobante.FacturaA, FormaEntrega.NoEmitir);

            Assert.AreEqual(TipoComprobante.FacturaA, comprobante.Tipo);
        }

        [TestMethod]
        public void ConfirmarEmision_FacturaBConsumidorFinal_Permite()
        {
            var repo = new ComprobanteRepositoryFake();
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante { IdVenta = 1, Total = 500m, NombreCliente = "Consumidor Final" };

            var comprobante = logica.ConfirmarEmision(datos, TipoComprobante.FacturaB, FormaEntrega.NoEmitir);

            Assert.AreEqual(TipoComprobante.FacturaB, comprobante.Tipo);
        }

        [TestMethod]
        public void ConfirmarEmision_Confirmada_UsaNumeroFormateadoConPuntoDeVenta()
        {
            var repo = new ComprobanteRepositoryFake();
            var logica = new LogicaComprobante(repo);
            var datos = new DatosVentaParaComprobante { IdVenta = 1, Total = 500m };

            var comprobante = logica.ConfirmarEmision(datos, TipoComprobante.Ticket, FormaEntrega.NoEmitir);

            Assert.AreEqual("0001-00000001", comprobante.NumeroFormateado);
        }
    }

    internal sealed class ComprobanteRepositoryFake : IComprobanteRepository
    {
        private readonly List<Comprobante> _comprobantes = new();
        private int _proximoId = 1;

        public List<Comprobante> Agregados { get; } = new();

        public void Cargar(params Comprobante[] comprobantes) => _comprobantes.AddRange(comprobantes);

        public long ObtenerUltimoNumero(TipoComprobante tipo, int puntoVenta) => _comprobantes
            .Where(c => c.Tipo == tipo && c.PuntoVenta == puntoVenta)
            .Select(c => (long?)c.Numero)
            .Max() ?? 0;

        public void Agregar(Comprobante comprobante)
        {
            comprobante.IdComprobante = _proximoId++;
            _comprobantes.Add(comprobante);
            Agregados.Add(comprobante);
        }

        public Comprobante? ObtenerPorId(int idComprobante) => _comprobantes
            .FirstOrDefault(c => c.IdComprobante == idComprobante);
    }
}
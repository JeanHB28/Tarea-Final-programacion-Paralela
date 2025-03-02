namespace Tarea_Final
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    class Order
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Cantidad { get; set; }
        public string ClienteInfo { get; set; }
    }

    static class Inventario
    {
        private static Dictionary<int, int> stock = new Dictionary<int, int>
        {
            {1, 10}, {2, 15}, {3, 5}, {4, 8}, {5, 20}
        };

        public static bool VerificarDisponibilidad(int productId, int cantidad)
        {
            return stock.ContainsKey(productId) && stock[productId] >= cantidad;
        }

        public static void ActualizarStock(int productId, int cantidad)
        {
            if (stock.ContainsKey(productId))
            {
                stock[productId] -= cantidad;
            }
        }
    }

    class OrderProcessor
    {
        private Random random = new Random();

        public async Task ProcessOrderAsync(Order order)
        {
            try
            {
                await ComprobarInventario(order);
                await ProcesoPago(order);
                await GenerarConfirmaciónEnvío(order);
                Console.WriteLine($"Orden {order.OrderId} procesada con exito.");
            }
            catch (Exception ex)
            {
                HandleError(order, ex);
            }
        }

        private async Task ComprobarInventario(Order order)
        {
            await Task.Delay(random.Next(100, 500)); // Simulate processing time
            if (!Inventario.VerificarDisponibilidad(order.ProductId, order.Cantidad))
            {
                throw new Exception("Inventario insuficiente");
            }
            Console.WriteLine($"Inventario verificado para la orden {order.OrderId}");
        }

        private async Task ProcesoPago(Order order)
        {
            await Task.Delay(random.Next(200, 1000)); 
            if (random.Next(10) == 0) 
            {
                throw new Exception("Error en el procesamiento del pago");
            }
            Console.WriteLine($"Pago procesado para la orden {order.OrderId}");
        }

        private async Task GenerarConfirmaciónEnvío(Order order)
        {
            await Task.Delay(random.Next(100, 300));
            Inventario.ActualizarStock(order.ProductId, order.Cantidad);
            Console.WriteLine($"Confirmacion de envio generada para la orden {order.OrderId}");
        }

        private void HandleError(Order order, Exception ex)
        {
            Console.WriteLine($"Error procesando la orden {order.OrderId}: {ex.Message}");
        }
    }

    class Program
    {
        static async Task Main()
        {
            var orders = GeneracionOrdenes(10);
            var processor = new OrderProcessor();
            var tasks = orders.Select(o => processor.ProcessOrderAsync(o)).ToList();

            while (tasks.Any())
            {
                var completedTask = await Task.WhenAny(tasks);
                tasks.Remove(completedTask);
                Console.WriteLine($"Orden procesada. Quedan {tasks.Count} ordenes.");
            }
        }

        private static List<Order> GeneracionOrdenes(int count)
        {
            var orders = new List<Order>();
            for (int i = 1; i <= count; i++)
            {
                orders.Add(new Order
                {
                    OrderId = i,
                    ProductId = new Random().Next(1, 6),
                    Cantidad = new Random().Next(1, 4),
                    ClienteInfo = $"Cliente {i}"
                });
            }
            return orders;
        }
    }
}
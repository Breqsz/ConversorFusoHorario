using System;

namespace ConversorFusoHorario
{
    public class AgendaEntrada : IAgendaEntrada
    {
        private readonly IConversorHora _conversor;

        /// <summary>
        /// Guardar em UTC (DateTimeKind.Utc).
        /// </summary>
        public DateTime DataHora { get; private set; }
        public string Titulo { get; private set; }

        public AgendaEntrada(DateTime dataHoraUtc, string titulo, IConversorHora conversor)
        {
            // Garante UTC
            DataHora = dataHoraUtc.Kind == DateTimeKind.Utc
                ? dataHoraUtc
                : DateTime.SpecifyKind(dataHoraUtc, DateTimeKind.Utc);

            Titulo = titulo;
            _conversor = conversor;
        }

        private DateTime ConverterParaDestino(string? idFusoDestino)
        {
            return string.IsNullOrWhiteSpace(idFusoDestino)
                ? DataHora
                : _conversor.ConverterParaFusoHorario(DataHora, idFusoDestino);
        }

        public void Imprimir(string? idFusoDestino = null)
        {
            var data = ConverterParaDestino(idFusoDestino);
            Console.WriteLine($"{data:G} - {Titulo}");
        }

        public void ImprimirHora(string? idFusoDestino = null)
        {
            var data = ConverterParaDestino(idFusoDestino);
            Console.WriteLine($"{data:HH:mm}");
        }

        public void ImprimirDia(string? idFusoDestino = null)
        {
            var data = ConverterParaDestino(idFusoDestino);
            Console.WriteLine($"{data:dd/MM/yyyy}");
        }

        public void ImprimirDiaSemana(string? idFusoDestino = null)
        {
            var data = ConverterParaDestino(idFusoDestino);
            Console.WriteLine($"{data:dddd}");
        }

        public Compromisso LerCompromisso()
        {
            // Constrói um Compromisso (UTC) a partir desta entrada.
            var dtoUtc = new DateTimeOffset(
                DateTime.SpecifyKind(DataHora, DateTimeKind.Utc),
                TimeSpan.Zero
            );
            return new Compromisso(Titulo, dtoUtc);
        }
    }
}

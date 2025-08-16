using System;

namespace ConversorFusoHorario
{
    public class Compromisso
    {
        public string Titulo { get; set; }
        public DateTimeOffset DataHoraUtc { get; set; } // Sempre em UTC

        public Compromisso(string titulo, DateTimeOffset dataHoraUtc)
        {
            Titulo = titulo;
            DataHoraUtc = dataHoraUtc;
        }

        public string Exibir(TimeZoneInfo fuso)
        {
            var horaConvertida = TimeZoneInfo.ConvertTime(DataHoraUtc, fuso);
            return $"{horaConvertida:G} - {Titulo}";
        }
    }
}

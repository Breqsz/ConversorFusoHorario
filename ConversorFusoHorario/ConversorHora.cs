using System;

namespace ConversorFusoHorario
{
    public class ConversorHora : IConversorHora
    {
        /// <summary>
        /// Converte uma data/hora em UTC para o fuso destino.
        /// </summary>
        public DateTime ConverterParaFusoHorario(DateTime dataHoraUtc, string idFusoDestino)
        {
            if (string.IsNullOrWhiteSpace(idFusoDestino))
                return dataHoraUtc;

            try
            {
                var destino = TimeZoneInfo.FindSystemTimeZoneById(idFusoDestino);

                // Garante que o DateTime seja tratado como UTC.
                var utc = dataHoraUtc.Kind == DateTimeKind.Utc
                    ? dataHoraUtc
                    : DateTime.SpecifyKind(dataHoraUtc, DateTimeKind.Utc);

                return TimeZoneInfo.ConvertTimeFromUtc(utc, destino);
            }
            catch
            {
                Console.WriteLine($"⚠️ Fuso horário '{idFusoDestino}' não encontrado. Usando horário original (UTC).");
                return dataHoraUtc;
            }
        }

        /// <summary>
        /// Converte uma data/hora local (no fuso informado) para UTC.
        /// </summary>
        public DateTimeOffset ConverterParaUtc(DateTime dataHoraLocal, string timezoneId)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            // Trata o DateTime informado como "hora no fuso tz".
            var unspecified = DateTime.SpecifyKind(dataHoraLocal, DateTimeKind.Unspecified);
            var offset = tz.GetUtcOffset(unspecified);
            var dtoLocal = new DateTimeOffset(unspecified, offset);
            return dtoLocal.ToUniversalTime();
        }

        // Mantido apenas se realmente precisar em outro ponto do seu projeto;
        // não é usado no fluxo principal.
        public string ObterFusoHorarioDaData(string dataHoraStr)
        {
            return DateTime.TryParse(dataHoraStr, out _)
                ? TimeZoneInfo.Local.Id
                : "Inválido";
        }
    }
}

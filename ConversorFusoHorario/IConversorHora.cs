using System;

namespace ConversorFusoHorario
{
    /// <summary>
    /// Conversões entre UTC e fusos de destino.
    /// </summary>
    public interface IConversorHora
    {
        // Recebe UTC e um timezoneId (Windows) e devolve a hora naquele fuso.
        DateTime ConverterParaFusoHorario(DateTime dataHoraUtc, string idFusoDestino);

        // Converte uma data/hora local (em um fuso informado) para UTC.
        DateTimeOffset ConverterParaUtc(DateTime dataHoraLocal, string timezoneId);
    }
}

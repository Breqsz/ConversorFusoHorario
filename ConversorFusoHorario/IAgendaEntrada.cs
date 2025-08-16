using System;

namespace ConversorFusoHorario
{
    /// <summary>
    /// Representa uma entrada de agenda (armazenada em UTC) com impressão opcional em outro fuso.
    /// </summary>
    public interface IAgendaEntrada
    {
        /// <summary>Data/hora em UTC.</summary>
        DateTime DataHora { get; }

        /// <summary>Título do compromisso.</summary>
        string Titulo { get; }

        void Imprimir(string? fusoDestino);
        Compromisso LerCompromisso();
    }
}

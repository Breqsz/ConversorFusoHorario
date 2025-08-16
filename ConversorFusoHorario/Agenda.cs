using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorFusoHorario
{
    public class Agenda
    {
        private readonly List<IAgendaEntrada> _compromissos = new();

        public void AdicionarCompromisso(IAgendaEntrada compromisso)
        {
            _compromissos.Add(compromisso);
        }

        /// <summary>
        /// Exibe os compromissos do DIA informado.
        /// Se fusoDestino for informado, a comparação e a visualização são feitas neste fuso.
        /// Saída em tabela, ordenada por horário.
        /// </summary>
        public void ExibirCompromissosPorDia(DateTime dia, string? fusoDestino = null)
        {
            var conversor = new ConversorHora();

            // Projeta cada item já no fuso de destino (ou UTC se não houver)
            var itens = _compromissos
                .Select(c =>
                {
                    var dataConvertida = string.IsNullOrWhiteSpace(fusoDestino)
                        ? c.DataHora              // UTC
                        : conversor.ConverterParaFusoHorario(c.DataHora, fusoDestino);

                    return new
                    {
                        Original = c,
                        Exibicao = dataConvertida
                    };
                })
                .Where(x => x.Exibicao.Date == dia.Date)
                .OrderBy(x => x.Exibicao.TimeOfDay)
                .ToList();

            var labelFuso = string.IsNullOrWhiteSpace(fusoDestino) ? "UTC" : fusoDestino;

            Console.WriteLine();
            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine($"Agenda - {dia:dd/MM/yyyy} [{labelFuso}]");
            Console.WriteLine("------------------------------------------------------------");

            if (itens.Count == 0)
            {
                Console.WriteLine("Nenhum compromisso encontrado para esta data.");
                return;
            }

            // Cabeçalho da tabela
            Console.WriteLine("{0,3}  {1,-5}  {2}", "#", "Hora", "Título");

            // Linhas
            for (int i = 0; i < itens.Count; i++)
            {
                var hora = itens[i].Exibicao.ToString("HH:mm");
                var titulo = itens[i].Original.Titulo;
                Console.WriteLine("{0,3}  {1,-5}  {2}", i + 1, hora, titulo);
            }

            Console.WriteLine("------------------------------------------------------------");
            Console.WriteLine($"Total: {itens.Count} compromisso(s).");
        }
    }
}

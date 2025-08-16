using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ConversorFusoHorario;

class Program
{
    private static readonly string FormatoData = "dd/MM/yyyy";
    private static readonly string FormatoHora = "HH:mm";

    // Cidades → Timezone (Windows). Pode ampliar à vontade.
    private static readonly Dictionary<string, string> CidadeParaTimeZone = new(StringComparer.OrdinalIgnoreCase)
    {
        { "brasilia", "E. South America Standard Time" },
        { "brasília", "E. South America Standard Time" },
        { "df",       "E. South America Standard Time" },
        { "sao paulo","E. South America Standard Time" },
        { "são paulo","E. South America Standard Time" },
        { "sp",       "E. South America Standard Time" },
        { "rio de janeiro","E. South America Standard Time" },
        { "rio","E. South America Standard Time" },
        { "curitiba","E. South America Standard Time" },
        { "belo horizonte","E. South America Standard Time" },
        { "salvador","E. South America Standard Time" },
        { "recife","E. South America Standard Time" },
        { "fortaleza","E. South America Standard Time" },
        { "manaus","SA Western Standard Time" },
        { "porto velho","SA Western Standard Time" },
        { "boa vista","SA Western Standard Time" },
        { "rio branco","SA Pacific Standard Time" },
        { "acre","SA Pacific Standard Time" },
        { "fernando de noronha","UTC-02" },
    };

    static void Main(string[] args)
    {
        // UTF-8 (se quiser usar emoji futuramente)
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        var conversor = new ConversorHora();
        var agenda = new Agenda();

        Console.WriteLine("===== AGENDA COM CONVERSOR DE FUSO HORÁRIO =====\n");

        int qtd = LerInteiroPositivo("Quantos compromissos deseja cadastrar? ");

        for (int i = 0; i < qtd; i++)
        {
            Console.WriteLine($"\n--- Compromisso {i + 1} ---");

            string titulo = LerTextoObrigatorio("Título: ");

            // Cadastro: DATA depois HORA
            DateTime data = LerData("Data (dd/MM/yyyy): ");
            DateTime hora = LerHora("Hora (HH:mm): ");

            DateTime dataHoraLocal = new DateTime(
                data.Year, data.Month, data.Day,
                hora.Hour, hora.Minute, 0
            );

            TimeZoneInfo tzOrigem = LerTimeZonePorCidade(
                "Cidade do fuso de ORIGEM (ex.: Brasilia, Sao Paulo) [Enter p/ local]: ");

            try
            {
                // Converte a data/hora informada (no fuso de origem) para UTC
                var dtoUtc = conversor.ConverterParaUtc(dataHoraLocal, tzOrigem.Id);
                var compromisso = new AgendaEntrada(dtoUtc.UtcDateTime, titulo, conversor);
                agenda.AdicionarCompromisso(compromisso);

                Console.WriteLine("[OK] Compromisso cadastrado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] Não foi possível cadastrar: {ex.Message}");
            }
        }

        Console.WriteLine("\n===== CONSULTA =====");

        TimeZoneInfo tzDestino = LerTimeZonePorCidade(
            "Cidade do fuso de DESTINO (ex.: Brasilia, Sao Paulo) [Enter p/ local]: ");

        // Consulta: DATA depois HORA (a hora aqui é só para referência visual; a listagem é por DIA)
        DateTime dataConsulta = LerData("Data para consultar (dd/MM/yyyy): ");
        DateTime? horaConsulta = LerHoraOpcional("Hora (HH:mm) [Enter p/ ignorar]: ");

        Console.WriteLine("\n--- Resultado ---");
        if (horaConsulta.HasValue)
        {
            Console.WriteLine($"Dia: {dataConsulta:dd/MM/yyyy}  •  Hora ref.: {horaConsulta.Value:HH:mm}  •  Fuso: {tzDestino.Id}");
        }
        else
        {
            Console.WriteLine($"Dia: {dataConsulta:dd/MM/yyyy}  •  Fuso: {tzDestino.Id}");
        }

        agenda.ExibirCompromissosPorDia(dataConsulta, tzDestino.Id);

        Console.WriteLine("\n[FIM] Pressione qualquer tecla para sair...");
        Console.ReadKey();
    }

    // ----------------- ENTRADAS -----------------

    static int LerInteiroPositivo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int v) && v > 0) return v;
            Console.WriteLine("Digite um número inteiro maior que zero.");
        }
    }

    static string LerTextoObrigatorio(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(s)) return s;
            Console.WriteLine("Campo obrigatório.");
        }
    }

    static DateTime LerData(string prompt)
    {
        var culture = new CultureInfo("pt-BR");
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine()?.Trim();
            if (DateTime.TryParseExact(s, FormatoData, culture, DateTimeStyles.None, out var dt))
                return dt;
            Console.WriteLine($"Data inválida. Use {FormatoData} (ex.: 15/08/2026).");
        }
    }

    static DateTime LerHora(string prompt)
    {
        var culture = new CultureInfo("pt-BR");
        while (true)
        {
            Console.Write(prompt);
            var s = Console.ReadLine()?.Trim();
            if (DateTime.TryParseExact(s, FormatoHora, culture, DateTimeStyles.None, out var dt))
                return dt;
            Console.WriteLine($"Hora inválida. Use {FormatoHora} (ex.: 14:30).");
        }
    }

    static DateTime? LerHoraOpcional(string prompt)
    {
        var culture = new CultureInfo("pt-BR");
        while (true)
        {
            Console.Write(prompt);
            var s = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrEmpty(s)) return null;
            if (DateTime.TryParseExact(s, FormatoHora, culture, DateTimeStyles.None, out var dt))
                return dt;
            Console.WriteLine($"Hora inválida. Use {FormatoHora} (ex.: 14:30) ou Enter para pular.");
        }
    }

    static TimeZoneInfo LerTimeZonePorCidade(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var input = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine($"(usando fuso local: {TimeZoneInfo.Local.Id})");
                return TimeZoneInfo.Local;
            }

            var chave = NormalizarCidade(input);

            if (CidadeParaTimeZone.TryGetValue(chave, out var windowsId))
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(windowsId);
                }
                catch { /* cai no fallback abaixo */ }
            }

            // Fallback: talvez o usuário digitou o ID do Windows
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(input);
            }
            catch
            {
                Console.WriteLine("Cidade/fuso inválido. Exemplos: Brasilia, Sao Paulo.");
            }
        }
    }

    // Normaliza acentos para casar com chaves
    static string NormalizarCidade(string s)
    {
        var normalized = s.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();
        foreach (var ch in normalized)
        {
            var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (cat != UnicodeCategory.NonSpacingMark) sb.Append(ch);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant().Trim();
    }
}

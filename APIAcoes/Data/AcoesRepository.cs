using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using APIAcoes.Models;

namespace APIAcoes.Data
{
    public class AcoesRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionMultiplexer _conexaoRedis;
        private readonly AcoesContext _context;
        private readonly TelemetryConfiguration _telemetryConfig;

        public AcoesRepository(
            IConfiguration configuration,
            ConnectionMultiplexer conexaoRedis,
            AcoesContext context,
            TelemetryConfiguration telemetryConfig)
        {
            _configuration = configuration;
            _conexaoRedis = conexaoRedis;
            _context = context;
            _telemetryConfig = telemetryConfig;
        }

        public UltimaCotacaoAcao Get(string codigo)
        {
            DateTimeOffset inicio = DateTime.Now;
            var watch = new Stopwatch();
            watch.Start();

            string strDadosAcao =
                _conexaoRedis.GetDatabase().StringGet(
                    $"{_configuration["Redis:PrefixoChave"]}-{codigo}");

            watch.Stop();
            TelemetryClient client = new (_telemetryConfig);
            client.TrackDependency(
                "Redis", "Get", strDadosAcao, inicio, watch.Elapsed, true);

            if (!String.IsNullOrWhiteSpace(strDadosAcao))
                return JsonSerializer.Deserialize<UltimaCotacaoAcao>(
                    strDadosAcao,
                    new ()
                    {
                        PropertyNameCaseInsensitive = true
                    });
            else
                return null;
        }

        public IEnumerable<HistoricoAcao> GetAll()
        {
            return _context.HistoricoAcoes.OrderByDescending(h => h.Id);
        }
    }
}
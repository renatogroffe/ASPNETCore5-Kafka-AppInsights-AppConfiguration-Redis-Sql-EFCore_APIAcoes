using System;

namespace APIAcoes.Models
{
    public class HistoricoAcao
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public DateTime? DataReferencia { get; set; }
        public decimal? Valor { get; set; }
        public string CodCorretora { get; set; }
        public string NomeCorretora { get; set; }
    }
}
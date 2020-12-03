using System;

namespace APIAcoes.Models
{
    public class UltimaCotacaoAcao
    {
        public string Codigo { get; set; }
        public DateTime? Data { get; set; }
        public double? Valor { get; set; }
        public string CodCorretora { get; set; }
        public string NomeCorretora { get; set; }
    }
}
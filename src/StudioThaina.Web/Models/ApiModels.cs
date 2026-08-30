namespace StudioThaina.Web.Models;
public enum StatusAgendamento { Agendado=1,Concluido=2,Cancelado=3 }
public sealed class ClienteModel { public int Id{get;set;} public string Nome{get;set;}=""; public string Telefone{get;set;}=""; public string? Observacao{get;set;} public bool Ativo{get;set;}=true; }
public sealed class ServicoModel { public int Id{get;set;} public string Nome{get;set;}=""; public string? Descricao{get;set;} public int DuracaoMinutos{get;set;}=60; public decimal Valor{get;set;} public bool Ativo{get;set;}=true; }
public sealed class AgendamentoModel { public int Id{get;set;} public int ClienteId{get;set;} public string Cliente{get;set;}=""; public int ServicoId{get;set;} public string Servico{get;set;}=""; public DateTime DataHora{get;set;}=DateTime.Today.AddHours(9); public decimal ValorCobrado{get;set;} public StatusAgendamento Status{get;set;} public string? Observacao{get;set;} }
public sealed class ResumoModel { public decimal Faturamento{get;set;} public int QuantidadeAtendimentos{get;set;} public decimal TicketMedio{get;set;} public List<ServicoRealizadoModel> ServicosMaisRealizados{get;set;}=[]; }
public sealed class ServicoRealizadoModel { public string Servico{get;set;}=""; public int Quantidade{get;set;} }
public sealed class ProjecaoModel { public decimal FaturamentoHistorico{get;set;} public int QuantidadeDiasAnalisados{get;set;} public decimal MediaDiaria{get;set;} public int QuantidadeDiasProjetados{get;set;} public decimal ValorProjetado{get;set;} public string Aviso{get;set;}=""; }

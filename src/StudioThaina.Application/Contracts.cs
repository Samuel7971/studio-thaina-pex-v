using StudioThaina.Domain;

namespace StudioThaina.Application;

public record ClienteInput(string Nome, string Telefone, string? Observacao, bool Ativo = true);
public record ServicoInput(string Nome, string? Descricao, int DuracaoMinutos, decimal Valor, bool Ativo = true);
public record AgendamentoInput(int ClienteId, int ServicoId, DateTime DataHora, decimal ValorCobrado, string? Observacao);
public record AlterarStatusInput(StatusAgendamento Status);
public record AgendaDto(int Id, int ClienteId, string Cliente, int ServicoId, string Servico, DateTime DataHora, decimal ValorCobrado, StatusAgendamento Status, string? Observacao);
public record ResumoDto(decimal Faturamento, int QuantidadeAtendimentos, decimal TicketMedio, IReadOnlyList<ServicoRealizadoDto> ServicosMaisRealizados);
public record ServicoRealizadoDto(int ServicoId, string Servico, int Quantidade);
public record ProjecaoDto(decimal FaturamentoHistorico, int QuantidadeDiasAnalisados, decimal MediaDiaria, int QuantidadeDiasProjetados, decimal ValorProjetado, string Aviso);

public interface IClienteRepository
{
    Task<IReadOnlyList<Cliente>> ListarAsync(); Task<Cliente?> ObterAsync(int id); Task<int> CriarAsync(Cliente item); Task<bool> AtualizarAsync(Cliente item);
}
public interface IServicoRepository
{
    Task<IReadOnlyList<Servico>> ListarAsync(); Task<Servico?> ObterAsync(int id); Task<int> CriarAsync(Servico item); Task<bool> AtualizarAsync(Servico item);
}
public interface IAgendamentoRepository
{
    Task<IReadOnlyList<AgendaDto>> ListarAsync(DateTime? inicio = null, DateTime? fim = null); 
    Task<AgendaDto?> ObterAsync(int id); 
    Task<int> CriarAsync(Agendamento item); 
    Task<bool> AlterarStatusAsync(int id, StatusAgendamento status); 
    Task<bool> ExisteConflitoAsync(DateTime dataHora); 
    Task<ResumoDto> ResumoAsync(DateTime inicio, DateTime fim);
}

public sealed class RegraNegocioException(string message) : Exception(message);

public sealed class StudioService(IClienteRepository clientes, IServicoRepository servicos, IAgendamentoRepository agendamentos)
{
    public Task<IReadOnlyList<Cliente>> ListarClientesAsync() => clientes.ListarAsync();
    public Task<Cliente?> ObterClienteAsync(int id) => clientes.ObterAsync(id);
    public async Task<int> CriarClienteAsync(ClienteInput x) { var e = new Cliente { Nome=x.Nome.Trim(), Telefone=x.Telefone.Trim(), Observacao=x.Observacao, Ativo=x.Ativo }; e.Validar(); return await clientes.CriarAsync(e); }
    public async Task<bool> AtualizarClienteAsync(int id, ClienteInput x) { var e = new Cliente { Id=id, Nome=x.Nome.Trim(), Telefone=x.Telefone.Trim(), Observacao=x.Observacao, Ativo=x.Ativo }; e.Validar(); return await clientes.AtualizarAsync(e); }
    public Task<IReadOnlyList<Servico>> ListarServicosAsync() => servicos.ListarAsync();
    public Task<Servico?> ObterServicoAsync(int id) => servicos.ObterAsync(id);
    public async Task<int> CriarServicoAsync(ServicoInput x) { var e = new Servico { Nome=x.Nome.Trim(), Descricao=x.Descricao, DuracaoMinutos=x.DuracaoMinutos, Valor=x.Valor, Ativo=x.Ativo }; e.Validar(); return await servicos.CriarAsync(e); }
    public async Task<bool> AtualizarServicoAsync(int id, ServicoInput x) { var e = new Servico { Id=id, Nome=x.Nome.Trim(), Descricao=x.Descricao, DuracaoMinutos=x.DuracaoMinutos, Valor=x.Valor, Ativo=x.Ativo }; e.Validar(); return await servicos.AtualizarAsync(e); }
    public Task<IReadOnlyList<AgendaDto>> ListarAgendamentosAsync(DateTime? i=null, DateTime? f=null) { ValidarPeriodoOpcional(i,f); return agendamentos.ListarAsync(i,f); }
    public Task<AgendaDto?> ObterAgendamentoAsync(int id) => agendamentos.ObterAsync(id);
    public async Task<int> CriarAgendamentoAsync(AgendamentoInput x)
    {
        var cliente = await clientes.ObterAsync(x.ClienteId); var servico = await servicos.ObterAsync(x.ServicoId);
        if (cliente is null || !cliente.Ativo) throw new RegraNegocioException("O cliente informado não existe ou está inativo.");
        if (servico is null || !servico.Ativo) throw new RegraNegocioException("O serviço informado não existe ou está inativo.");
        if (await agendamentos.ExisteConflitoAsync(x.DataHora)) throw new RegraNegocioException("O horário informado já está ocupado.");
        var e = new Agendamento { ClienteId=x.ClienteId, ServicoId=x.ServicoId, DataHora=x.DataHora, ValorCobrado=x.ValorCobrado, Observacao=x.Observacao }; e.Validar(); return await agendamentos.CriarAsync(e);
    }
    public Task<bool> AlterarStatusAsync(int id, StatusAgendamento status) { if (!Enum.IsDefined(status)) throw new ArgumentException("Status inválido."); return agendamentos.AlterarStatusAsync(id,status); }

    public Task<ResumoDto> ResumoAsync(DateTime i, DateTime f) 
    { 
        ValidarPeriodo(i,f); 

        return agendamentos.ResumoAsync(i,f);
    }

    public async Task<ProjecaoDto> ProjetarAsync(DateTime i, DateTime f, int dias)
    {
        ValidarPeriodo(i,f); if (dias <= 0) throw new ArgumentException("A quantidade de dias projetados deve ser maior que zero.");
        var resumo=await agendamentos.ResumoAsync(i,f); var analisados=(f.Date-i.Date).Days+1; var media=resumo.Faturamento/analisados;
        return new(resumo.Faturamento, analisados, media, dias, media*dias, "Projeção demonstrativa; não representa previsão financeira garantida.");
    }
    private static void ValidarPeriodo(DateTime i, DateTime f) { if (f < i) throw new ArgumentException("A data final deve ser maior ou igual à inicial."); }
    private static void ValidarPeriodoOpcional(DateTime? i, DateTime? f) { if (i.HasValue && f.HasValue) ValidarPeriodo(i.Value,f.Value); }
}

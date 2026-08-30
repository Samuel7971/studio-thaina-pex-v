using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudioThaina.Application;
using StudioThaina.Domain;

namespace StudioThaina.Infrastructure;

public interface IDbConnectionFactory { IDbConnection Create(); }
internal sealed class SqlConnectionFactory(IConfiguration config) : IDbConnectionFactory
{
    public IDbConnection Create() => new SqlConnection(config.GetConnectionString("StudioThaina") ?? throw new InvalidOperationException("Connection string 'StudioThaina' não configurada."));
}
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) => services
        .AddSingleton<IDbConnectionFactory, SqlConnectionFactory>()
        .AddScoped<IClienteRepository, ClienteRepository>().AddScoped<IServicoRepository, ServicoRepository>().AddScoped<IAgendamentoRepository, AgendamentoRepository>();
}

internal sealed class ClienteRepository(IDbConnectionFactory db) : IClienteRepository
{
    public async Task<IReadOnlyList<Cliente>> ListarAsync() { using var c=db.Create(); return (await c.QueryAsync<Cliente>("SELECT Id,Nome,Telefone,Observacao,Ativo FROM Cliente ORDER BY Nome")).AsList(); }
    public async Task<Cliente?> ObterAsync(int id) { using var c=db.Create(); return await c.QuerySingleOrDefaultAsync<Cliente>("SELECT Id,Nome,Telefone,Observacao,Ativo FROM Cliente WHERE Id=@id",new{id}); }
    public async Task<int> CriarAsync(Cliente x) { using var c=db.Create(); return await c.ExecuteScalarAsync<int>("INSERT Cliente(Nome,Telefone,Observacao,Ativo) OUTPUT INSERTED.Id VALUES(@Nome,@Telefone,@Observacao,@Ativo)",x); }
    public async Task<bool> AtualizarAsync(Cliente x) { using var c=db.Create(); return await c.ExecuteAsync("UPDATE Cliente SET Nome=@Nome,Telefone=@Telefone,Observacao=@Observacao,Ativo=@Ativo WHERE Id=@Id",x)>0; }
}
internal sealed class ServicoRepository(IDbConnectionFactory db) : IServicoRepository
{
    public async Task<IReadOnlyList<Servico>> ListarAsync() { using var c=db.Create(); return (await c.QueryAsync<Servico>("SELECT Id,Nome,Descricao,DuracaoMinutos,Valor,Ativo FROM Servico ORDER BY Nome")).AsList(); }
    public async Task<Servico?> ObterAsync(int id) { using var c=db.Create(); return await c.QuerySingleOrDefaultAsync<Servico>("SELECT Id,Nome,Descricao,DuracaoMinutos,Valor,Ativo FROM Servico WHERE Id=@id",new{id}); }
    public async Task<int> CriarAsync(Servico x) { using var c=db.Create(); return await c.ExecuteScalarAsync<int>("INSERT Servico(Nome,Descricao,DuracaoMinutos,Valor,Ativo) OUTPUT INSERTED.Id VALUES(@Nome,@Descricao,@DuracaoMinutos,@Valor,@Ativo)",x); }
    public async Task<bool> AtualizarAsync(Servico x) { using var c=db.Create(); return await c.ExecuteAsync("UPDATE Servico SET Nome=@Nome,Descricao=@Descricao,DuracaoMinutos=@DuracaoMinutos,Valor=@Valor,Ativo=@Ativo WHERE Id=@Id",x)>0; }
}
internal sealed class AgendamentoRepository(IDbConnectionFactory db) : IAgendamentoRepository
{
    private const string Select="""SELECT a.Id,a.ClienteId,c.Nome Cliente,a.ServicoId,s.Nome Servico,a.DataHora,a.ValorCobrado,a.Status,a.Observacao FROM Agendamento a JOIN Cliente c ON c.Id=a.ClienteId JOIN Servico s ON s.Id=a.ServicoId""";
    public async Task<IReadOnlyList<AgendaDto>> ListarAsync(DateTime? inicio=null,DateTime? fim=null) { using var c=db.Create(); return (await c.QueryAsync<AgendaDto>(Select+" WHERE (@inicio IS NULL OR a.DataHora>=@inicio) AND (@fim IS NULL OR a.DataHora<DATEADD(day,1,CAST(@fim AS date))) ORDER BY a.DataHora",new{inicio,fim})).AsList(); }
    public async Task<AgendaDto?> ObterAsync(int id) { using var c=db.Create(); return await c.QuerySingleOrDefaultAsync<AgendaDto>(Select+" WHERE a.Id=@id",new{id}); }
    public async Task<int> CriarAsync(Agendamento x) { using var c=db.Create(); return await c.ExecuteScalarAsync<int>("INSERT Agendamento(ClienteId,ServicoId,DataHora,ValorCobrado,Status,Observacao) OUTPUT INSERTED.Id VALUES(@ClienteId,@ServicoId,@DataHora,@ValorCobrado,@Status,@Observacao)",x); }
    public async Task<bool> AlterarStatusAsync(int id,StatusAgendamento status) { using var c=db.Create(); return await c.ExecuteAsync("UPDATE Agendamento SET Status=@status WHERE Id=@id",new{id,status})>0; }
    public async Task<bool> ExisteConflitoAsync(DateTime dataHora) { using var c=db.Create(); return await c.ExecuteScalarAsync<bool>("SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM Agendamento WHERE DataHora=@dataHora AND Status<>3) THEN 1 ELSE 0 END AS bit)",new{dataHora}); }
    public async Task<ResumoDto> ResumoAsync(DateTime inicio,DateTime fim)
    {
        using var c=db.Create(); const string filtro="a.Status=2 AND a.DataHora>=@inicio AND a.DataHora<DATEADD(day,1,CAST(@fim AS date))";
        var total=await c.QuerySingleAsync<(decimal Faturamento,int Quantidade)>("SELECT COALESCE(SUM(a.ValorCobrado),0) Faturamento,COUNT(*) Quantidade FROM Agendamento a WHERE "+filtro,new{inicio,fim});
        var ranking=(await c.QueryAsync<ServicoRealizadoDto>("SELECT s.Id ServicoId,s.Nome Servico,COUNT(*) Quantidade FROM Agendamento a JOIN Servico s ON s.Id=a.ServicoId WHERE "+filtro+" GROUP BY s.Id,s.Nome ORDER BY Quantidade DESC,s.Nome",new{inicio,fim})).AsList();
        return new(total.Faturamento,total.Quantidade,total.Quantidade==0?0:total.Faturamento/total.Quantidade,ranking);
    }
}

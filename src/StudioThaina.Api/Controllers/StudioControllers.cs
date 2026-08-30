using Microsoft.AspNetCore.Mvc;
using StudioThaina.Application;

namespace StudioThaina.Api.Controllers;

[ApiController, Route("api/clientes")]
public sealed class ClientesController(StudioService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await service.ListarClientesAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
        => await service.ObterClienteAsync(id) is { } x ? Ok(x) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Criar(ClienteInput x)
    {
        var id = await service.CriarClienteAsync(x);
        return CreatedAtAction(
            nameof(Obter),
            new
            {
                id
            },
            await service.ObterClienteAsync(id));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, ClienteInput x)
        => await service.AtualizarClienteAsync(id, x) ? NoContent() : NotFound();
}

[ApiController, Route("api/servicos")]
public sealed class ServicosController(StudioService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await service.ListarServicosAsync());
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
        => await service.ObterServicoAsync(id) is { } x
        ? Ok(x)
        : NotFound();

    [HttpPost]
    public async Task<IActionResult> Criar(ServicoInput x)
    {
        var id = await service.CriarServicoAsync(x);
        return CreatedAtAction(
            nameof(Obter),
            new
            {
                id
            },
            await service.ObterServicoAsync(id));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, ServicoInput x)
        => await service.AtualizarServicoAsync(id, x)
        ? NoContent()
        : NotFound();
}

[ApiController, Route("api/agendamentos")]
public sealed class AgendamentosController(StudioService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar()
        => Ok(await service.ListarAgendamentosAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obter(int id)
        => await service.ObterAgendamentoAsync(id) is { } x ? Ok(x) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Criar(AgendamentoInput x)
    {
        try
        {
            var id = await service.CriarAgendamentoAsync(x);
            return CreatedAtAction(nameof(Obter),
                new { id }, await service.ObterAgendamentoAsync(id));
        }
        catch (RegraNegocioException e)
        {
            return Conflict(new { mensagem = e.Message });
        }
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> Status(int id, AlterarStatusInput x)
        => await service.AlterarStatusAsync(id, x.Status) ? NoContent() : NotFound();
}

[ApiController, Route("api/agenda")]
public sealed class AgendaController(StudioService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Consultar(DateTime dataInicio, DateTime dataFim)
        => Ok(await service.ListarAgendamentosAsync(dataInicio, dataFim));
}


[ApiController, Route("api/relatorios")]
public sealed class RelatoriosController(StudioService service) : ControllerBase
{
    [HttpGet("resumo")]
    public async Task<IActionResult> Resumo(DateTime dataInicio, DateTime dataFim)
           => Ok(await service.ResumoAsync(dataInicio, dataFim));

    [HttpGet("faturamento")]
    public async Task<IActionResult> Faturamento(DateTime dataInicio, DateTime dataFim)
    {
        var x = await service.ResumoAsync(dataInicio, dataFim); return Ok(new { x.Faturamento });
    }

    [HttpGet("servicos-mais-realizados")]
    public async Task<IActionResult> Servicos(DateTime dataInicio, DateTime dataFim)
    {
        var x = await service.ResumoAsync(dataInicio, dataFim); return Ok(x.ServicosMaisRealizados);
    }
}

[ApiController, Route("api/projecoes")]
public sealed class ProjecoesController(StudioService service) : ControllerBase
{
    [HttpGet("ganhos")]
    public async Task<IActionResult> Ganhos(DateTime dataInicioHistorico, DateTime dataFimHistorico, int quantidadeDiasProjetados)
        => Ok(await service.ProjetarAsync(dataInicioHistorico, dataFimHistorico, quantidadeDiasProjetados));
}

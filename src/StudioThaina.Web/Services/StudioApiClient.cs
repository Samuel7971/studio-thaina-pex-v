using System.Net.Http.Json;
using StudioThaina.Web.Models;
namespace StudioThaina.Web.Services;
public sealed class StudioApiClient(HttpClient http)
{
 public async Task<List<ClienteModel>> Clientes()=>await http.GetFromJsonAsync<List<ClienteModel>>("api/clientes")??[];
 public async Task Salvar(ClienteModel x){var r=x.Id==0?await http.PostAsJsonAsync("api/clientes",x):await http.PutAsJsonAsync($"api/clientes/{x.Id}",x);r.EnsureSuccessStatusCode();}
 public async Task<List<ServicoModel>> Servicos()=>await http.GetFromJsonAsync<List<ServicoModel>>("api/servicos")??[];
 public async Task Salvar(ServicoModel x){var r=x.Id==0?await http.PostAsJsonAsync("api/servicos",x):await http.PutAsJsonAsync($"api/servicos/{x.Id}",x);r.EnsureSuccessStatusCode();}
 public async Task<List<AgendamentoModel>> Agendamentos(DateTime? i=null,DateTime? f=null){var url=i.HasValue?$"api/agenda?dataInicio={i:O}&dataFim={f:O}":"api/agendamentos";return await http.GetFromJsonAsync<List<AgendamentoModel>>(url)??[];}
 public async Task Criar(AgendamentoModel x){var r=await http.PostAsJsonAsync("api/agendamentos",x);r.EnsureSuccessStatusCode();}
 public async Task Status(int id,StatusAgendamento s){var r=await http.PutAsJsonAsync($"api/agendamentos/{id}/status",new{Status=s});r.EnsureSuccessStatusCode();}
 public async Task<ResumoModel> Resumo(DateTime i,DateTime f)=>await http.GetFromJsonAsync<ResumoModel>($"api/relatorios/resumo?dataInicio={i:O}&dataFim={f:O}")??new();
 public async Task<ProjecaoModel> Projecao(DateTime i,DateTime f,int d)=>await http.GetFromJsonAsync<ProjecaoModel>($"api/projecoes/ganhos?dataInicioHistorico={i:O}&dataFimHistorico={f:O}&quantidadeDiasProjetados={d}")??new();
}

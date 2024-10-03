using Newtonsoft.Json;
using RepositorioGitHub.Business.Contract;
using RepositorioGitHub.Dominio;
using RepositorioGitHub.Dominio.Interfaces;
using RepositorioGitHub.Infra.Contract;
using RepositorioGitHub.Infra.Repositorio;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;


namespace RepositorioGitHub.Business
{
    public class GitHubApiBusiness: IGitHubApiBusiness
    {
        private readonly IContextRepository _context;
        private readonly IGitHubApi _gitHubApi;
		private readonly IContextRepository _contextRepository;

		public GitHubApiBusiness(IContextRepository context, IGitHubApi gitHubApi)
        {
            _context = context;
            _gitHubApi = gitHubApi;
			_contextRepository = new ContextRepository();
		}

		public ActionResult<GitHubRepositoryViewModel> Get()
		{
			string url = "https://api.github.com/users/darcyneto/repos";

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

				var response = client.GetAsync(url).Result;

				if (!response.IsSuccessStatusCode)
				{
					return new ActionResult<GitHubRepositoryViewModel>
					{
						IsValid = false,
						Message = "Erro ao buscar repositórios."
					};
				}

				var content = response.Content.ReadAsStringAsync().Result;

				// desserializa a resposta como uma lista
				var repositories = JsonConvert.DeserializeObject<List<GitHubRepositoryViewModel>>(content);

				// preenche a ViewModel com a lista de resultados
				return new ActionResult<GitHubRepositoryViewModel>
				{
					IsValid = true,
					Message = "Repositórios obtidos com sucesso.",
					Results = repositories
				};
			}
		}

		public ActionResult<GitHubRepositoryViewModel> GetById(long id)
		{			
			string url = $"https://api.github.com/repositories/{id}";

			// cria um HttpClient para fazer a requisição
			using (var client = new HttpClient())
			{
				// adicionando o cabeçalho "User-Agent"
				client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

				// faz a requisição HTTP GET
				var response = client.GetAsync(url).Result;

				// verifica se a resposta foi bem-sucedida
				if (!response.IsSuccessStatusCode)
				{
					return new ActionResult<GitHubRepositoryViewModel>
					{
						IsValid = false,
						Message = "Erro ao buscar o repositório."
					};
				}

				// lê o conteúdo da resposta
				var content = response.Content.ReadAsStringAsync().Result;

				// deserializa a resposta Json para o modelo
				var repository = JsonConvert.DeserializeObject<GitHubRepositoryViewModel>(content);

				// retorna o modelo preenchido
				return new ActionResult<GitHubRepositoryViewModel>
				{
					Result = repository,
					IsValid = true,
					Message = "Repositório encontrado com sucesso."
				};
			}
		}

		public ActionResult<RepositoryViewModel> GetByName(string name)
		{
			
			string url = $"https://api.github.com/users/{name}/repos";
		
			using (var client = new HttpClient())
			{
				// adiciona User-Agent para evitar erro de requisição sem cabeçalhos
				client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

				// faz a requisição HTTP
				var response = client.GetAsync(url).Result;

				// cria e preenchendo o ViewModel
				var repositoryViewModel = new RepositoryViewModel();

				// verifica se a resposta é bem-sucedida
				if (!response.IsSuccessStatusCode)
				{
					// preenche as propriedades para validação
					repositoryViewModel.IsValid = false;
					repositoryViewModel.Message = "Erro ao buscar os repositórios. Verifique o nome do usuário ou tente novamente.";

					// retorna o ViewModel encapsulada no ActionResult
					return new ActionResult<RepositoryViewModel>
					{
						Result = repositoryViewModel
					};
				}

				// lê o conteúdo da resposta como string
				var content = response.Content.ReadAsStringAsync().Result;

				// deserializa a resposta para o array de repositórios
				var repositories = JsonConvert.DeserializeObject<GitHubRepository[]>(content);

				// preenche os dados do ViewModel
				repositoryViewModel.TotalCount = repositories.Length;
				repositoryViewModel.Repositories = repositories;
				repositoryViewModel.Name = name;
				repositoryViewModel.Description = $"Repositórios do usuário {name} encontrados com sucesso.";

				// sinaliza que a operação foi bem-sucedida
				repositoryViewModel.IsValid = true;
				repositoryViewModel.Message = "Repositórios encontrados com sucesso.";

				// retorna o viewModel encapsulado no ActionResult
				return new ActionResult<RepositoryViewModel>
				{
					Result = repositoryViewModel
				};
			}
		}

		public ActionResult<FavoriteViewModel> GetFavoriteRepository()
		{
			// puxa todos os favoritos
			var favorites = _contextRepository.GetAll();

			// se não houver favoritos, retorne um modelo inválido
			if (favorites.Count == 0)
			{
				return new ActionResult<FavoriteViewModel>
				{
					IsValid = false,
					Message = "Nenhum favorito encontrado."
				};
			}

			var favoriteViewModels = favorites.Select(f => new FavoriteViewModel
			{
				Name = f.Name,
				Description = f.Description,
				Language = f.Language,
				LastUpdate = f.LastUpdate,
				Owner = f.Owner,			
			}).ToList();

			// retorna a lista preenchida na propriedade "Results"
			return new ActionResult<FavoriteViewModel>
			{
				Results = favoriteViewModels,
				IsValid = true,
				Message = "Favoritos encontrados com sucesso."
			};
		}

		public ActionResult<GitHubRepositoryViewModel> GetRepository(string owner, long id)
		{
			// monta a URL da API do GitHub
			string url = $"https://api.github.com/repositories/{id}";

			using (var client = new HttpClient())
			{
				// adiciona User-Agent para evitar erro de requisição sem cabeçalhos
				client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

				// faz a requisição HTTP GET
				var response = client.GetAsync(url).Result;

				// verifica se a resposta foi bem-sucedida
				if (!response.IsSuccessStatusCode)
				{
					return new ActionResult<GitHubRepositoryViewModel>
					{
						IsValid = false,
						Message = "Erro ao buscar o repositório."
					};
				}

				// lê o conteúdo da resposta
				var content = response.Content.ReadAsStringAsync().Result;

				// deserializa a resposta Json para o modelo
				var repository = JsonConvert.DeserializeObject<GitHubRepositoryViewModel>(content);

				// retorna o modelo preenchido
				return new ActionResult<GitHubRepositoryViewModel>
				{
					Result = repository,
					IsValid = true,
					Message = "Repositório encontrado com sucesso."
				};
			}
		}

		public ActionResult<FavoriteViewModel> SaveFavoriteRepository(FavoriteViewModel view)
		{
			// cria uma instância de Favorite a partir do FavoriteViewModel
			var favorite = new Favorite
			{
				Description = view.Description,
				Language = view.Language,
				Owner = view.Owner,
				LastUpdate = view.LastUpdate,
				Name = view.Name,			
				
			};

			// usa o repositório para salvar o favorito
			var result = _contextRepository.Insert(favorite);

			if (result)
			{
				return new ActionResult<FavoriteViewModel>
				{
					IsValid = true,
					Message = "Repositório favoritado com sucesso.",
					Result = view
				};
			}
			else
			{
				return new ActionResult<FavoriteViewModel>
				{
					IsValid = false,
					Message = "Este repositório já está favoritado."
				};
			}
		}

	}
}

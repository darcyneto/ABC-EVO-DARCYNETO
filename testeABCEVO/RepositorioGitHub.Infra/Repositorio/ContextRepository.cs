using RepositorioGitHub.Dominio;
using RepositorioGitHub.Infra.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositorioGitHub.Infra.Repositorio
{

	public class ContextRepository : IContextRepository
	{
		private static List<Favorite> _favorites = new List<Favorite>(); // lista estática para armazenar favoritos

		public bool ExistsByCheckAlready(Favorite favorite)
		{
			return _favorites.Any(f => f.Name == favorite.Name && f.Owner == favorite.Owner);
		}

		public List<Favorite> GetAll()
		{
			return _favorites; // retorna a lista de favoritos
		}

		public bool Insert(Favorite favorite)
		{
			if (!ExistsByCheckAlready(favorite))
			{
				_favorites.Add(favorite); // adiciona o novo favorito à lista
				return true;
			}

			return false; // retorna false se o favorito já existir
		}		
	}

}

using RepositorioGitHub.Dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositorioGitHub.Dominio
{
    public class GitHubRepositoryViewModel
    {     
        public long Id { get; set; }
        public string Name { get; set; }
        public string Full_Name { get; set; }
        public Owner Owner { get; set; }
        public string Description { get; set; }
        public Uri Url { get; set; }      
        public DateTimeOffset Updated_At { get; set; }        
        public string Homepage { get; set; }       
        public string Language { get; set; }	
	}
}

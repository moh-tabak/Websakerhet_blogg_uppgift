using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Uppgift2.Data;
using Uppgift2.Services;

namespace Uppgift2.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IDbService _dbService;

        [BindProperty]
        public Blog NewPost { get; set; }

        public List<Blog> Blogs { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IDbService dbService)
        {
            _logger = logger;
            _dbService = dbService;
            Blogs = new();
            NewPost = new();
        }

        public async Task OnGetAsync()
        {
            Blogs = await _dbService.GetAllAsync<Blog>(_dbService.CollectionNames["blogs"]);
        }

        public void OnPost()
        {
            if (string.IsNullOrEmpty(NewPost.Title) || string.IsNullOrEmpty(NewPost.HtmlText))
            {
                this.Response.Redirect(this.Request.Path, true);
                return;
            }
            NewPost.DateTimeCreated = DateTime.UtcNow;
            //TODO: change to user name when users and authentication are done.
            NewPost.AuthorName = "Användare#1";

            //Add to database
            _dbService.Add<Blog>(NewPost, _dbService.CollectionNames["blogs"]);

            //Refresh page
            this.Response.Redirect(this.Request.Path, true);
        }

    }

}


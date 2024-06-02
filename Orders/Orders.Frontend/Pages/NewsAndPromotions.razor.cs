namespace Orders.Frontend.Pages
{
    public partial class NewsAndPromotions
    {
        private List<NewsArticle> newsArticles = new()
        {
            new NewsArticle { Id = 1, Title = "Noticia 1", Summary = "Resumen de la noticia 1", ImageUrl = "https://via.placeholder.com/200" },
            new NewsArticle { Id = 2, Title = "Noticia 2", Summary = "Resumen de la noticia 2", ImageUrl = "https://via.placeholder.com/200" },
            new NewsArticle { Id = 3, Title = "Noticia 3", Summary = "Resumen de la noticia 3", ImageUrl = "https://via.placeholder.com/200" },
            new NewsArticle { Id = 4, Title = "Noticia 4", Summary = "Resumen de la noticia 4", ImageUrl = "https://via.placeholder.com/200" },
            new NewsArticle { Id = 5, Title = "Noticia 5", Summary = "Resumen de la noticia 5", ImageUrl = "https://via.placeholder.com/200" }
        };

        private void ViewDetails(int id)
        {
            // Aqu� puedes implementar la navegaci�n a una p�gina de detalles o mostrar un di�logo con m�s informaci�n.
        }
    }

    public class NewsArticle
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
    }
}
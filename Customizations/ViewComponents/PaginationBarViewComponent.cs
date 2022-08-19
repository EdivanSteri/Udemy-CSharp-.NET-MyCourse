using Microsoft.AspNetCore.Mvc;
using MyCourse.Models.ViewModels;

namespace MyCourse.Customizations.ViewComponents
{
    public class PaginationBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IPaginationInfo model)
        {
            //il numero di pagina corrente
            //il numero di risultati totali
            //il numero di risultati per pagina
            //Search, oredrby e Ascending
            return View(model);
        }
    }
}

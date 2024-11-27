using System.Threading.Tasks;
using ChemDec.Api.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChemDec.Api.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserResolver userResolver;

        public HomeController(UserResolver userResolver)
        {
            this.userResolver = userResolver;
        }

        /*     private readonly ICacheHelper cacheHelper;
 private readonly IUserUtils userUtils;
 private readonly SptsContext db;

public HomeController(ICacheHelper cacheHelper, IUserUtils userUtils, SptsContext db)
 {
     this.cacheHelper = cacheHelper;
     this.userUtils = userUtils;
     this.db = db;
 }*/

        [HttpGet]
       // [Authorize(AuthenticationSchemes = Program.Schemes)]
        public  IActionResult Index()
        {
           // var user = await userResolver.GetUser(User);
            //var isAdmin = user.Roles.Any(w => w.Code == "Appl_SPTS_Internal_Coordinator");
          
           // ViewData["user"] = user.Username;
           // ViewData["userName"] = user.Name;
           // ViewData["isAdmin"] = isAdmin;
          //  ViewData["isBoundToSystem"] = user.MmtSystemAccess.Any();
           // ViewData["systems"] = JsonConvert.SerializeObject(user.MmtSystemAccess.Select(s => new { code = s.Key, description = s.Value }).ToList() ?? new object());

            return View();
        }


       /* [Authorize(AuthenticationSchemes = Program.Schemes)]
        [HttpGet]
        [Route("flush")]
        public IActionResult Flush()
        {
            foreach (var cat in Enum.GetNames(typeof(CacheCategories)))
            {
                cacheHelper.ClearCache(cat);
            }
            return Json(new { Res = "Ok" });
        }

        [Authorize("MmtAdmin", AuthenticationSchemes = Program.Schemes)]
        [HttpGet]
        [Route("activeUsers")]
        public IActionResult AllActiveUsers()
        {
            var res = cacheHelper.GetAll(Enum.GetName(typeof(CacheCategories), CacheCategories.User));

            return Json(new { Users = res });
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }*/
    }
}
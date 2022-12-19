using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Nasa_Application.Data;
using NasaAuthenticationProject.Models;
using System.Data;
using System.Globalization;
using System.Web;


namespace NasaAuthenticationProject.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {

            _db = db;
        }

        public JsonResult isEmailUnique(string email)
        {
            var validateEmail = _db.Users.FirstOrDefault(u => u.Email == email);
            if (validateEmail != null)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        public string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }


        //Get
        public IActionResult Register()
        {
            var cookie = Request.Cookies["LoggedIn"];
            if (cookie != null && cookie.Equals("True"))
            {
                ViewData["LoggedIn"] = "True";
                return View();
            }
            ViewData["LoggedIn"] = "False";
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User obj)
        {
            var uppercase = 0;
            var lowercase = 0;
            var symbols = 0;
            var numbers = 0;
            foreach (char c in obj.Password)
            {
                if (Char.IsSymbol(c))
                {
                    symbols++;
                }
                else if (Char.IsUpper(c))
                {
                    uppercase++;
                }else if (Char.IsLower(c))
                {
                    lowercase++;
                }else if (Char.IsNumber(c))
                {
                    numbers++;
                }
            }
            if (symbols < 2 || uppercase < 3 || lowercase < 3 || numbers < 2)
            {
                ModelState.AddModelError("password", "Password must have at least two symbols, two numbers, three uppercase letters and three lowercase letters");
                ViewData["LoggedIn"] = "False";
            }
            if (obj.Password.Length != 10)
            {
                ModelState.AddModelError("password", "The Password is not 10 characters long");
                ViewData["LoggedIn"] = "False";
            }
            if (ModelState.IsValid)
            {
                obj.Password = EncodePasswordToBase64(obj.Password);
                _db.Users.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(obj);
        }

        //Get
        public IActionResult Login()
        {
            var cookie = Request.Cookies["LoggedIn"];
            if (cookie != null && cookie.Equals("True"))
            {
                ViewData["LoggedIn"] = "True";
                return View();
            }
            ViewData["LoggedIn"] = "False";
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(User obj)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == obj.Email);
            if (user == null)
            {
                ViewData["Error"] = "Incorrect combination of username or password";
            }
            if (EncodePasswordToBase64(obj.Password) == user.Password)
            {
                var cookieOptions = new CookieOptions();
                cookieOptions.Expires = DateTime.Now.AddDays(1);
                cookieOptions.Path = "/";
                Response.Cookies.Append("LoggedIn", "True", cookieOptions);
                ViewData["LoggedIn"] = "True";
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewData["LoggedIn"] = "False";
                ViewData["Error"] = "Incorrect combination of username or password";
            }
            return View(obj);
        }

        //Get
        public IActionResult Dashboard()
        {
            var cookie = Request.Cookies["LoggedIn"];
            if (cookie != null && cookie.Equals("True"))
            {
                ViewData["LoggedIn"] = "True";
                var objUserList = _db.Users.ToList();

                return View(objUserList);
            }
            ViewData["LoggedIn"] = "False";
            return RedirectToAction("Login");
        }

        //Get
        public IActionResult LogOut()
        {
            ViewData["LoggedIn"] = "False";
            Response.Cookies.Delete("LoggedIn");

            return View();

        }

        public IActionResult ExportToExcel()
        {
            var cookie = Request.Cookies["LoggedIn"];
            if (cookie != null && cookie.Equals("True"))
            {
                ViewData["LoggedIn"] = "True";
            }
            else
            {
                ViewData["LoggedIn"] = "False";
                return RedirectToAction("Login");
            }

            var objUserList = _db.Users.ToList();


            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[5] { new DataColumn("Id"),
                                     new DataColumn("Email"),new DataColumn("Password"), new DataColumn("Created Time"), new DataColumn("Type")});

            foreach (var user in objUserList)
            {
                dt.Rows.Add(user.Id, user.Email, user.Password, user.CreatedDateTime, user.Type);
            }
            //using ClosedXML.Excel;
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
        }
    }
}

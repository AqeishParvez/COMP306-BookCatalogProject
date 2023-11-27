using BookCatalogClient.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace BookCatalogClient.Controllers
{
    public class BooksController : Controller
    {
        Uri Uri = new Uri("http://localhost:5200/api/");
        private readonly HttpClient _httpClient;
        public BooksController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = Uri;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: BooksController
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<BookModel> booksList = new List<BookModel>();
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress+"Books/GetAllBooks");

            if(response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                booksList = JsonConvert.DeserializeObject<List<BookModel>>(json);
            }
            
            return View(booksList);
        }

        // GET: BooksController/Details/5
        /*[HttpGet]
        public async Task<IActionResult> Details(string id)
        {
			BookModel bookModel = new BookModel();
			HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "Books/GetBookById/" + id);

			if (response.IsSuccessStatusCode)
            {
				string json = await response.Content.ReadAsStringAsync();
				bookModel = JsonConvert.DeserializeObject<BookModel>(json);
			}

			return View(bookModel);
		}*/

        // GET: BooksController/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: BooksController/Create
        [HttpPost]
        public async Task<IActionResult> Create(BookModel bookModel)
        {
            try
            {
                
                string json = JsonConvert.SerializeObject(bookModel);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "Books/AddBook", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Book Added Successfully";
                    return RedirectToAction("Index");
                }
                                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }

            return View();

        }

        // GET: BooksController/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
			BookModel bookModel = new BookModel();
			HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "Books/GetBookById/" + id);

			if (response.IsSuccessStatusCode)
            {
				string json = await response.Content.ReadAsStringAsync();
				bookModel = JsonConvert.DeserializeObject<BookModel>(json);
			}

			return View(bookModel);
		}

        // POST: BooksController/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(BookModel bookModel)
        {
			try
            {
				string json = JsonConvert.SerializeObject(bookModel);
				StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
				HttpResponseMessage response = await _httpClient.PutAsync(_httpClient.BaseAddress + "Books/UpdateBook/"+bookModel.Id, content);

				if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Book Updated Successfully";
					return RedirectToAction("Index");
				}
			}
			catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
				return View();
			}

			return View();
		}

        // GET: BooksController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                BookModel bookModel = new BookModel();
                HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "Books/GetBookById/" + id);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    bookModel = JsonConvert.DeserializeObject<BookModel>(json);
                }
                return View(bookModel);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View();
            }

        }

        // POST: BooksController/Delete/5
        [HttpPost,ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
			try
            {
				HttpResponseMessage response = await _httpClient.DeleteAsync(_httpClient.BaseAddress + "Books/DeleteBook/" + id);
				if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Book Deleted Successfully";
					return RedirectToAction("Index");
				}
				return View();
			}
			catch (Exception ex)
            {
                TempData["errorMessage"] = ex.Message;
				return View();
			}
		}

        // GET: BooksController/ViewPdf/5
        [HttpGet]
        public async Task<IActionResult> ViewPdf(string id)
        {
            try
            {
                // Retrieve the PdfFilePath for the given book ID
                HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "Books/GetBookById/" + id);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var bookModel = JsonConvert.DeserializeObject<BookModel>(json);

                    // Pass the PdfFilePath to the view
                    return Redirect(bookModel.PdfFilePath);
                }

                TempData["ErrorMessage"] = "Error retrieving book information.";
                return View("PdfViewer", string.Empty);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View("PdfViewer", string.Empty);
            }
        }

    }
}

using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using TorgovayaPloshadka.Data;
using TorgovayaPloshadka.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Linq;

namespace TorgovayaPloshadka.Controllers
{
    public class OtchetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OtchetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OtchetController
        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Import(IFormFile fileExcel)
        {
            using (XLWorkbook workbook = new XLWorkbook(fileExcel.OpenReadStream()))
            {
                List<Customer_ImpExp> Customer_ImpExps = new List<Customer_ImpExp>();
                List<Product_ImpExp> Product_ImpExps = new List<Product_ImpExp>();

                foreach (IXLWorksheet worksheet in workbook.Worksheets)
                {
                    if (worksheet.Name == "Customer")
                    {
                        foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                        {
                            Customer customer = new Customer();
                            var range = worksheet.RangeUsed();

                            var table = range.AsTable();

                            customer.Surname = row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "Surname").RangeAddress.FirstAddress.ColumnNumber).Value.ToString();
                            customer.Name = row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "Name").RangeAddress.FirstAddress.ColumnNumber).Value.ToString();
                            customer.Midname = row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "Midname").RangeAddress.FirstAddress.ColumnNumber).Value.ToString();
                            customer.Phone = row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "Phone").RangeAddress.FirstAddress.ColumnNumber).Value.ToString();
                            customer.Address = row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "Address").RangeAddress.FirstAddress.ColumnNumber).Value.ToString();

                            _context.Customers.Add(customer);

                            _context.SaveChanges();

                            Customer_ImpExps.Add(new Customer_ImpExp { CustomerSubd = customer.CustomerId, CustomerExcel = int.Parse(row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "CustomerId").RangeAddress.FirstAddress.ColumnNumber).Value.ToString()) }); ;
                        }
                    }

                    if (worksheet.Name == "Product")
                    {
                        foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                        {
                            Product product = new Product();
                            var range = worksheet.RangeUsed();

                            var table = range.AsTable();

                            product.ProductName = row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "ProductName").RangeAddress.FirstAddress.ColumnNumber).Value.ToString();
                            product.CategoryId = Convert.ToInt32(row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "CategoryId").RangeAddress.FirstAddress.ColumnNumber).Value.ToString());
                            product.ManufacturerId = Convert.ToInt32(row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "ManufacturerId").RangeAddress.FirstAddress.ColumnNumber).Value.ToString());
                            product.SupplierId = Convert.ToInt32(row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "SupplierId").RangeAddress.FirstAddress.ColumnNumber).Value.ToString());
                            product.Price = Convert.ToInt32(row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "Price").RangeAddress.FirstAddress.ColumnNumber).Value.ToString());
                            product.Weight = Convert.ToInt32(row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "Weight").RangeAddress.FirstAddress.ColumnNumber).Value.ToString());
                            product.Proportions = row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "Proportions").RangeAddress.FirstAddress.ColumnNumber).Value.ToString();

                            _context.Products.Add(product);

                            _context.SaveChanges();

                            Product_ImpExps.Add(new Product_ImpExp { ProductSubd = product.ProductId, ProductExcel = int.Parse(row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "ProductId").RangeAddress.FirstAddress.ColumnNumber).Value.ToString()) }); ;
                        }
                    }

                    if (worksheet.Name == "Order")
                    {
                        foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                        {
                            Order orders = new Order();
                            var range = worksheet.RangeUsed();

                            var table = range.AsTable();

                            orders.CustomerId = Customer_ImpExps.FirstOrDefault(c => c.CustomerExcel == int.Parse(row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "CustomerId").RangeAddress.FirstAddress.ColumnNumber).Value.ToString())).CustomerSubd;
                            orders.ProductId = Product_ImpExps.FirstOrDefault(c => c.ProductExcel == int.Parse(row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "ProductId").RangeAddress.FirstAddress.ColumnNumber).Value.ToString())).ProductSubd;
                            orders.Date_Ordered = DateTime.Parse(row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "Date_Ordered").RangeAddress.FirstAddress.ColumnNumber).Value.ToString());
                            orders.Count = Convert.ToInt32(row.Cell(table.FindColumn(c => c.FirstCell().Value.ToString() == "Count").RangeAddress.FirstAddress.ColumnNumber).Value.ToString());

                            _context.Orders.Add(orders);

                            _context.SaveChanges();
                        }
                    }
                }
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: OtchetController/Details/5
        public ActionResult Export(int? id)
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Otchet");
                worksheet.Cell(1, 1).Value = "Категория";
                worksheet.Cell(1, 2).Value = "Количество товаров";

                worksheet.Row(1).Style.Font.Bold = true;

                var otch = _context.Set<Order_CountOtchet>().FromSqlInterpolated($"EXEC Otchet").ToList();
                int i = 2;
                foreach (Order_CountOtchet item in otch)
                {
                    worksheet.Cell(i, 1).Value = item.nm;
                    worksheet.Cell(i, 2).Value = item.kol;
                    i++;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreedsheetml.sheet")
                    {
                        FileDownloadName = $"Otchet_{DateTime.UtcNow.ToLongDateString()}.xlsx"
                    };
                }
            }
        }

        public ActionResult Export2()
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var worksheet1 = workbook.Worksheets.Add("Customer");
                worksheet1.Cell(1, 1).Value = "CustomerId";
                worksheet1.Cell(1, 2).Value = "Surname";
                worksheet1.Cell(1, 3).Value = "Name";
                worksheet1.Cell(1, 4).Value = "Midname";
                worksheet1.Cell(1, 5).Value = "Phone";
                worksheet1.Cell(1, 6).Value = "Address";

                int i1 = 2;

                worksheet1.Row(1).Style.Font.Bold = true;

                foreach (Customer item in _context.Customers)
                {
                    worksheet1.Cell(i1, 1).Value = item.CustomerId;
                    worksheet1.Cell(i1, 2).Value = item.Surname;
                    worksheet1.Cell(i1, 3).Value = item.Name;
                    worksheet1.Cell(i1, 4).Value = item.Midname;
                    worksheet1.Cell(i1, 5).Value = item.Phone;
                    worksheet1.Cell(i1, 6).Value = item.Address;

                    i1++;
                }

                var worksheet2 = workbook.Worksheets.Add("Product");
                worksheet2.Cell(1, 1).Value = "ProductId";
                worksheet2.Cell(1, 2).Value = "ProductName";
                worksheet2.Cell(1, 3).Value = "CategoryId";
                worksheet2.Cell(1, 4).Value = "ManufacturerId";
                worksheet2.Cell(1, 5).Value = "SupplierId";
                worksheet2.Cell(1, 6).Value = "Price";
                worksheet2.Cell(1, 7).Value = "Weight";
                worksheet2.Cell(1, 8).Value = "Proportions";

                int i2 = 2;

                worksheet2.Row(1).Style.Font.Bold = true;

                foreach (Product item in _context.Products)
                {
                    worksheet2.Cell(i2, 1).Value = item.ProductId;
                    worksheet2.Cell(i2, 2).Value = item.ProductName;
                    worksheet2.Cell(i2, 3).Value = item.CategoryId;
                    worksheet2.Cell(i2, 4).Value = item.ManufacturerId;
                    worksheet2.Cell(i2, 5).Value = item.SupplierId;
                    worksheet2.Cell(i2, 6).Value = item.Price;
                    worksheet2.Cell(i2, 7).Value = item.Weight;
                    worksheet2.Cell(i2, 8).Value = item.Proportions;

                    i2++;
                }

                var worksheet = workbook.Worksheets.Add("Order");
                worksheet.Cell(1, 1).Value = "OrderId";
                worksheet.Cell(1, 2).Value = "CustomerId";
                worksheet.Cell(1, 3).Value = "ProductId";
                worksheet.Cell(1, 4).Value = "Date_Ordered";
                worksheet.Cell(1, 5).Value = "Count";

                worksheet.Row(1).Style.Font.Bold = true;

                int i = 2;
                foreach (Order item in _context.Orders)
                {
                    worksheet.Cell(i, 1).Value = item.OrderId;
                    worksheet.Cell(i, 2).Value = item.CustomerId;
                    worksheet.Cell(i, 3).Value = item.ProductId;
                    worksheet.Cell(i, 4).Value = item.Date_Ordered;
                    worksheet.Cell(i, 5).Value = item.Count;

                    i++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return new FileContentResult(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreedsheetml.sheet")
                    {
                        FileDownloadName = $"Export_{DateTime.UtcNow.ToLongDateString()}.xlsx"
                    };
                }
            }
        }
    }
}

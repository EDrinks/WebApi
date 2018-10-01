using System;
using System.Data;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using EDrinks.QueryHandlers.Model;

namespace EDrinks.WebApi.Utils
{
    public static class SettlementTransformer
    {
        public static MemoryStream SettlementToXlsxStream(Settlement settlement)
        {
            var wb = new XLWorkbook();
            var dt = new DataTable();
            dt.TableName = "Overview";
            dt.Columns.Add("Tab", typeof(string));
            dt.Columns.Add("Sum", typeof(decimal));
            foreach (var tabToOrders in settlement.TabToOrders)
            {
                var sum = Math.Round(tabToOrders.Orders.Sum(e => e.Quantity * e.ProductPrice), 2);
                dt.Rows.Add(tabToOrders.Tab.Name, sum);
            }

            wb.Worksheets.Add(dt);
            var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Position = 0;

            return stream;
        }
    }
}
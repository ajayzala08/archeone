using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Res;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ArcheOne.Controllers
{
    public class ProductTypeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ProductTypeList()
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                List<GetProductTypeListResModel> List = new List<GetProductTypeListResModel>();
                for (int i = 1; i <= 15; i++)
                {
                    GetProductTypeListResModel getProductTypeListResModel = new GetProductTypeListResModel();
                    getProductTypeListResModel.ProductTypeId = i;
                    getProductTypeListResModel.ProductTypeName = "Product Type " + i.ToString();
                    getProductTypeListResModel.ProductTypeCode = "PT" + i.ToString();
                    getProductTypeListResModel.IsActive = i == 2 || i == 3 ? false : true;
                    List.Add(getProductTypeListResModel);
                }
                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Success!";
                commonResponse.Data = List;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return View(commonResponse);
        }

        public IActionResult AddEditProductType(int ProductTypeId)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                AddEditProductTypeResModel addEditProductTypeResModel = new AddEditProductTypeResModel();
                if (ProductTypeId > 0)
                {
                    addEditProductTypeResModel.ProductTypeId = 2;
                    addEditProductTypeResModel.ProductTypeName = "Product Type 2";
                    addEditProductTypeResModel.ProductTypeCode = "PT2";
                    addEditProductTypeResModel.IsActive = false;
                }

                commonResponse.Status = true;
                commonResponse.StatusCode = HttpStatusCode.OK;
                commonResponse.Message = "Success!";
                commonResponse.Data = addEditProductTypeResModel;
            }
            catch (Exception ex)
            {
                commonResponse.Message = ex.Message;
            }
            return View(commonResponse);
        }
    }
}

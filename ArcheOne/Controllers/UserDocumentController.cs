using System.Net;
using ArcheOne.Database.Entities;
using ArcheOne.Helper.CommonHelpers;
using ArcheOne.Helper.CommonModels;
using ArcheOne.Models.Req;
using ArcheOne.Models.Res;
using Azure.Core;
using Azure;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNet.SignalR.Hosting;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Wordprocessing;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Font = iTextSharp.text.Font;
using Document = iTextSharp.text.Document;
using Microsoft.CodeAnalysis.Elfie.Extensions;

namespace ArcheOne.Controllers
{
	public class UserDocumentController : Controller
	{
		private readonly DbRepo _dbRepo;
		private readonly CommonHelper _commonHelper;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ArcheOneDbContext _dbContext;
		private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostEnvironment;
		public UserDocumentController(DbRepo dbRepo, CommonHelper commonHelper, IWebHostEnvironment webHostEnvironment, ArcheOneDbContext dbContext, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostEnvironment)
		{
			_dbRepo = dbRepo;
			_commonHelper = commonHelper;
			_webHostEnvironment = webHostEnvironment;
			_dbContext = dbContext;
			_hostEnvironment = hostEnvironment;
		}

		public async Task<IActionResult> UserDocument(int Id)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				AddEditUserDocumentReqModel addEditUserDocumentReqModel = new AddEditUserDocumentReqModel();
				addEditUserDocumentReqModel.AddEditUserDocuments = new AddEditUserDocument();
				addEditUserDocumentReqModel.UserList = _dbRepo.AllUserMstList().ToList();
				addEditUserDocumentReqModel.DocumentTypeList = _dbRepo.DocumentTypeList().ToList();
				if (Id > 0)
				{
					var userDocuments = await _dbRepo.UserDocumentList().FirstOrDefaultAsync(x => x.Id == Id);
					if (userDocuments != null)
					{
						addEditUserDocumentReqModel.AddEditUserDocuments.Id = userDocuments.Id;
						addEditUserDocumentReqModel.AddEditUserDocuments.UserId = userDocuments.UserId;
						addEditUserDocumentReqModel.AddEditUserDocuments.DocumentTypeId = userDocuments.DocumentTypeId;
						addEditUserDocumentReqModel.AddEditUserDocuments.Document = userDocuments.Document;
						addEditUserDocumentReqModel.AddEditUserDocuments.IsActive = userDocuments.IsActive;
					}
				}
				commonResponse.Status = true;
				commonResponse.StatusCode = HttpStatusCode.OK;
				commonResponse.Message = "Success!";
				commonResponse.Data = addEditUserDocumentReqModel;
			}
			catch (Exception ex)
			{
				commonResponse.Data = ex.Message;
				commonResponse.Status = false;
			}
			return View(commonResponse.Data);
		}

		public async Task<CommonResponse> SaveUpdateUserDocument(SaveUpdateUserDocumentsReqModel saveUpdateUserDocumentsReqModel)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				IFormFile file;
				string fileName = string.Empty;
				bool validateFileExtension = false;
				bool validateFileSize = false;
				string filePath = string.Empty;
				UserDocumentMst userDocumentMst = new UserDocumentMst();
				if (saveUpdateUserDocumentsReqModel.Document != null)
				{
					file = saveUpdateUserDocumentsReqModel.Document;
					fileName = file.FileName;
					FileInfo fileInfo = new FileInfo(fileName);
					string fileExtension = fileInfo.Extension;
					long fileSize = file.Length;

					string[] allowedFileExtensions = { CommonConstant.pdf };
					long allowedFileSize = 1 * 1024 * 1024 * 10; // 10MB
					validateFileExtension = allowedFileExtensions.Contains(fileExtension) ? true : false;
					validateFileSize = fileSize <= allowedFileSize ? true : false;
					fileName = saveUpdateUserDocumentsReqModel.UserId + saveUpdateUserDocumentsReqModel.Id + fileExtension;
					if (validateFileExtension && validateFileSize)
					{
						var imageFile = _commonHelper.UploadFile(saveUpdateUserDocumentsReqModel.Document, @"UserDocument", fileName, false, true, false);
						filePath = imageFile.Data.RelativePath;
					}
					else
					{
						commonResponse.Message = "Only pdf and jpg files are Allowed !";
					}
					int userId = _commonHelper.GetLoggedInUserId();
					if (saveUpdateUserDocumentsReqModel.Id == 0) // Add New Project
					{
						if (!await _dbRepo.UserDocumentList().AnyAsync(x => x.UserId == saveUpdateUserDocumentsReqModel.UserId && x.DocumentTypeId == saveUpdateUserDocumentsReqModel.DocumentTypeId))
						{
							UserDocumentMst userDocument = new UserDocumentMst()
							{
								UserId = saveUpdateUserDocumentsReqModel.UserId,
								DocumentTypeId = saveUpdateUserDocumentsReqModel.DocumentTypeId,
								Document = filePath,
								IsActive = true,
								IsDelete = false,
								CreatedBy = userId,
								UpdatedBy = userId,
								CreatedDate = _commonHelper.GetCurrentDateTime(),
								UpdatedDate = _commonHelper.GetCurrentDateTime(),
							};

							await _dbContext.AddAsync(userDocument);
							await _dbContext.SaveChangesAsync();

							commonResponse.Status = true;
							commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
							commonResponse.Message = "UserDocument added successfully!";
						}
						else
						{
							commonResponse.Message = "UserDocument is already exist!";
						}
					}
					else
					{
						// Edit Mode
						var duplicateUserDocs = await _dbRepo.UserDocumentList().FirstOrDefaultAsync(x => x.Id == saveUpdateUserDocumentsReqModel.Id);
						if (duplicateUserDocs != null)
						{
							if (duplicateUserDocs.DocumentTypeId != saveUpdateUserDocumentsReqModel.DocumentTypeId)
							{

								duplicateUserDocs.UserId = saveUpdateUserDocumentsReqModel.UserId;
								duplicateUserDocs.DocumentTypeId = saveUpdateUserDocumentsReqModel.DocumentTypeId;
								if (!string.IsNullOrEmpty(filePath))
								{
									duplicateUserDocs.Document = filePath;
								}
								duplicateUserDocs.UpdatedDate = _commonHelper.GetCurrentDateTime();
								duplicateUserDocs.UpdatedBy = _commonHelper.GetLoggedInUserId();

								_dbContext.Entry(duplicateUserDocs).State = EntityState.Modified;
								 await _dbContext.SaveChangesAsync();

								commonResponse.Status = true;
								commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
								commonResponse.Message = "Document updated successfully!";
							}
							else
							{
								commonResponse.Message = "Document is already exist!";
							}
						}
						else
						{
							commonResponse.Message = "Document is already exist!";
						}
					}
				}
				else
				{
					commonResponse.Message = "Please select any document!";
				}
			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
			}
			return commonResponse;
		}

		public async Task<CommonResponse> UserDocumentList()
	{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				dynamic userDocumentList;
				userDocumentList = (from UD in await _dbRepo.UserDocumentList().ToListAsync()
									join U in _dbRepo.AllUserMstList()
													   on UD.UserId equals U.Id
									join D in _dbRepo.DocumentTypeList()
									on UD.DocumentTypeId equals D.Id
									join f in _dbRepo.UserDetailList() on U.Id equals f.UserId
									select new { UD, U, D,f })
					 .Select(x => new UserDocumentResModel
					 {
						 Id = x.UD.Id,
						 EmployeeName = x.U.FirstName +" "+x.U.LastName,
						 EmployeeCode = x.f.EmployeeCode,
						 DocumentTypeId = x.D.DocumentType,
						 Document = System.IO.File.Exists(Path.Combine(_commonHelper.GetPhysicalRootPath(false), x.UD.Document)) ? Path.Combine(@"\", x.UD.Document) :
						  @"\Theme\Logo\default_user_profile.png"
					 }).ToList();

				if (userDocumentList != null && userDocumentList.Count > 0)
				{
					commonResponse.Data = userDocumentList;
					commonResponse.Status = true;
					commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
					commonResponse.Message = "Data found successfully!";
				}
				else
				{
					commonResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
					commonResponse.Message = "Data not found!";
				}
			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
			}
			return commonResponse;
		}

		public async Task<CommonResponse> GetUserDocumentById(int Id)
		{
			CommonResponse response = new CommonResponse();
			try
			{
				var userDocsDetails = await _dbRepo.UserDocumentList().FirstOrDefaultAsync(x => x.Id == Id);
				if (userDocsDetails != null)
				{
					response.Data = userDocsDetails;
					response.Status = true;
					response.StatusCode = System.Net.HttpStatusCode.OK;
					response.Message = "UserDocument found successfully!";
				}
				else
				{
					response.StatusCode = System.Net.HttpStatusCode.NotFound;
					response.Message = "UserDocument not found!";
				}
			}
			catch (Exception ex)
			{
				response.Message = ex.Message;
			}
			return response;
		}

		public async Task<CommonResponse> DeleteUserDocument(int id)
		{
			CommonResponse commonResponse = new CommonResponse();
			try
			{
				var userDocumentMst = await _dbRepo.UserDocumentList().FirstOrDefaultAsync(x => x.Id == id);
				if (userDocumentMst != null)
				{
					userDocumentMst.IsDelete = true;
					userDocumentMst.UpdatedBy = _commonHelper.GetLoggedInUserId();
					userDocumentMst.UpdatedDate = _commonHelper.GetCurrentDateTime();

					_dbContext.Entry(userDocumentMst).State = EntityState.Modified;
					await _dbContext.SaveChangesAsync();

					commonResponse.Status = true;
					commonResponse.StatusCode = System.Net.HttpStatusCode.OK;
					commonResponse.Message = "User Document deleted successfully!";
				}
				else
				{
					commonResponse.StatusCode = System.Net.HttpStatusCode.NotFound;
					commonResponse.Message = "Data not found!";
				}
			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
			}
			return commonResponse;
		}

		public async Task<FileResult> GetUserDocument(int? id)
		{
			CommonResponse commonResponse = new CommonResponse();
			string userDocument = "Files\\DefaultPolicyDocument\\HRPolicy0.pdf";
			byte[] FileBytes = System.IO.File.ReadAllBytes(Path.Combine(_commonHelper.GetPhysicalRootPath(false), userDocument));
			try
			{
				if (id > 0)
				{
					var docsList = await _dbRepo.UserDocumentList().FirstOrDefaultAsync(x => x.Id == id);

					string ReportURL = docsList.Document;
					FileBytes = System.IO.File.ReadAllBytes(Path.Combine(_commonHelper.GetPhysicalRootPath(false), ReportURL));
				}
				else
				{
					commonResponse.StatusCode = HttpStatusCode.NotFound;
					commonResponse.Message = "Data Not Found";
				}
			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
				commonResponse.Data = ex;
			}
			return File(FileBytes, "application/pdf");
		}

		public async Task<FileResult> GetUserOfferLetter(int id)
		{
			CommonResponse commonResponse = new CommonResponse();
			string userDocument = "Files\\DefaultPolicyDocument\\HRPolicy0.pdf";
			byte[] FileBytes = System.IO.File.ReadAllBytes(Path.Combine(_commonHelper.GetPhysicalRootPath(false), userDocument));
			try
			{
				if (id > 0)
				{
					var docsList = await _dbRepo.UserDocumentList().FirstOrDefaultAsync(x => x.Id == id);

					string ReportURL = docsList.Document;
					FileBytes = System.IO.File.ReadAllBytes(Path.Combine(_commonHelper.GetPhysicalRootPath(false), ReportURL));
				}
				else
				{
					commonResponse.StatusCode = HttpStatusCode.NotFound;
					commonResponse.Message = "Data Not Found";
				}
			}
			catch (Exception ex)
			{
				commonResponse.Message = ex.Message;
				commonResponse.Data = ex;
			}
			return File(FileBytes, "application/pdf");
		}

		[HttpGet]
		public async Task<IActionResult> DownloadConfirmationLetter(int id)
		{
			string filePath = string.Empty;
			string pdfFileName = string.Empty;

			Rectangle pageSize = new Rectangle(iTextSharp.text.PageSize.A4);
			//pageSize.BackgroundColor = new BaseColor(234, 244, 251);
			Document document = new Document(pageSize);

			var user = await _dbRepo.AllUserMstList().FirstOrDefaultAsync(x => x.Id == id);
			var userDetails = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == id);
			using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
			{

				PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
				document.Open();
				var fontTableHeader = FontFactory.GetFont("https://fonts.googleapis.com/css?family=Poppins", 12, Font.NORMAL, BaseColor.BLACK);
				var fontTableRow = FontFactory.GetFont("https://fonts.googleapis.com/css?family=Poppins", 10, Font.NORMAL, BaseColor.GRAY);
				PdfContentByte content = writer.DirectContentUnder;

				#region image
				PdfPTable table = new PdfPTable(4);
				table.WidthPercentage = 90;

				table.HorizontalAlignment = Rectangle.ALIGN_LEFT;
				PdfPCell cell2 = new PdfPCell((iTextSharp.text.Image.GetInstance("D:\\ArcheProjects\\ArcheOne\\DS\\archeone\\ArcheOne\\wwwroot\\Files\\UserDocument\\Reyna.jpg")));
				cell2.FixedHeight = 50;

				cell2.Border = Rectangle.NO_BORDER;
				cell2.PaddingLeft = 0;
				cell2.PaddingTop = 10;
				cell2.PaddingBottom = 10;
				cell2.PaddingRight = 0;
				cell2.HorizontalAlignment = Rectangle.ALIGN_LEFT;
				table.AddCell(cell2);

				PdfPCell cell = new PdfPCell(new Phrase("Arche Softronix "));
				cell2.HorizontalAlignment = Rectangle.ALIGN_LEFT;
				cell.Colspan = 6;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase(DateTime.Now.ToString()));
				cell.Colspan = 6;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("Dear " + user.FirstName + ' ' + user.MiddleName + ' ' + user.LastName + ","));
				cell.Colspan = 6;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("With reference to the review of your performance during the probation period from  " + userDetails.JoinDate + ' ' + "to" + userDetails.JoinDate.AddDays(60) + "We are grateful to inform you that your employment is being confirmed as " + userDetails.Designation + "effective from " + userDetails.JoinDate.AddDays(60)));
				cell.Colspan = 12;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 0;
				cell.PaddingBottom = 0;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("The terms and conditions as per mentioned in your appointment letter will remain unchanged."));
				cell.Colspan = 12;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("We look forward to your valuable contribution and wish you all the very best for a fruitful career with our company."));
				cell.Colspan = 12;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("Please sign the duplicate copy of this letter as a token of acceptance of the same"));
				cell.Colspan = 12;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				#endregion

				//main table create
				PdfPTable MainTable = new PdfPTable(3);
				MainTable.WidthPercentage = 100;

				#region summary
				PdfPCell MainTableCell_1 = new PdfPCell(new Phrase("Thank You!"));
				MainTableCell_1.Colspan = 3;
				MainTableCell_1.PaddingBottom = 10;
				MainTableCell_1.BorderWidthBottom = 0;
				MainTableCell_1.BorderWidthLeft = 0;
				MainTableCell_1.BorderWidthTop = 0;
				MainTableCell_1.BorderWidthRight = 0;
				MainTableCell_1.PaddingBottom = 10;
				//MainTableCell_1.Border = PdfPCell.NO_BORDER;
				MainTableCell_1.HorizontalAlignment = 1;
				MainTable.AddCell(MainTableCell_1);
				#endregion

				document.Add(table);
				document.Add(MainTable);
				document.Close();
				byte[] bytes = memoryStream.ToArray();
				memoryStream.Close();
				var filename = "Employee_Confirmation_Letter";
				pdfFileName = filename.ToString() + ".pdf";
				filePath = (filename + ".pdf");
				return File(bytes, "application/pdf", filePath);
			}
		}

		[HttpGet]
		public async Task<IActionResult> DownloadExperienceLetter(int id)
		{
			string filePath = string.Empty;
			string pdfFileName = string.Empty;

			Rectangle pageSize = new Rectangle(iTextSharp.text.PageSize.A4);
			Document document = new Document(pageSize);

			var user = await _dbRepo.AllUserMstList().FirstOrDefaultAsync(x => x.Id == id);
			var userDetails = await _dbRepo.UserDetailList().FirstOrDefaultAsync(x => x.UserId == id);
			using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
			{

				PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
				document.Open();
				var fontTableHeader = FontFactory.GetFont("https://fonts.googleapis.com/css?family=Poppins", 12, Font.NORMAL, BaseColor.BLACK);
				var fontTableRow = FontFactory.GetFont("https://fonts.googleapis.com/css?family=Poppins", 10, Font.NORMAL, BaseColor.GRAY);
				PdfContentByte content = writer.DirectContentUnder;

				#region image
				PdfPTable table = new PdfPTable(4);
				table.WidthPercentage = 90;

				table.HorizontalAlignment = Rectangle.ALIGN_LEFT;
				PdfPCell cell2 = new PdfPCell((iTextSharp.text.Image.GetInstance("D:\\ArcheProjects\\ArcheOne\\DS\\archeone\\ArcheOne\\wwwroot\\Files\\UserDocument\\Reyna.jpg")));
				cell2.FixedHeight = 50;

				cell2.Border = Rectangle.NO_BORDER;
				cell2.PaddingLeft = 0;
				cell2.PaddingTop = 10;
				cell2.PaddingBottom = 10;
				cell2.PaddingRight = 0;
				cell2.HorizontalAlignment = Rectangle.ALIGN_LEFT;
				table.AddCell(cell2);

				PdfPCell cell = new PdfPCell(new Phrase("Arche Softronix "));
				cell2.HorizontalAlignment = Rectangle.ALIGN_LEFT;
				cell.Colspan = 6;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase(DateTime.Now.ToString()));
				cell.Colspan = 6;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("Dear " + user.FirstName + ' ' + user.MiddleName + ' ' + user.LastName + ","));
				cell.Colspan = 6;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("With reference to the review of your performance during the probation period from  " + userDetails.JoinDate + ' ' + "to" + userDetails.JoinDate.AddDays(60) + "We are grateful to inform you that your employment is being confirmed as " + userDetails.Designation + "effective from " + userDetails.JoinDate.AddDays(60)));
				cell.Colspan = 12;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 0;
				cell.PaddingBottom = 0;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("The terms and conditions as per mentioned in your appointment letter will remain unchanged."));
				cell.Colspan = 12;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("We look forward to your valuable contribution and wish you all the very best for a fruitful career with our company."));
				cell.Colspan = 12;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				cell = new PdfPCell(new Phrase("Please sign the duplicate copy of this letter as a token of acceptance of the same"));
				cell.Colspan = 12;
				cell.Border = Rectangle.NO_BORDER;
				cell.PaddingLeft = 0;
				cell.PaddingTop = 10;
				cell.PaddingBottom = 10;
				cell.PaddingRight = 0;
				table.AddCell(cell);

				#endregion

				//main table create
				PdfPTable MainTable = new PdfPTable(3);
				MainTable.WidthPercentage = 100;

				#region summary
				PdfPCell MainTableCell_1 = new PdfPCell(new Phrase("Thank You!"));
				MainTableCell_1.Colspan = 3;
				MainTableCell_1.PaddingBottom = 10;
				MainTableCell_1.BorderWidthBottom = 0;
				MainTableCell_1.BorderWidthLeft = 0;
				MainTableCell_1.BorderWidthTop = 0;
				MainTableCell_1.BorderWidthRight = 0;
				MainTableCell_1.PaddingBottom = 10;
				//MainTableCell_1.Border = PdfPCell.NO_BORDER;
				MainTableCell_1.HorizontalAlignment = 1;
				MainTable.AddCell(MainTableCell_1);
				#endregion

				document.Add(table);
				document.Add(MainTable);
				document.Close();
				byte[] bytes = memoryStream.ToArray();
				memoryStream.Close();
				var filename = "Employee_Experience_Letter";
				pdfFileName = filename.ToString() + ".pdf";
				filePath = (filename + ".pdf");
				return File(bytes, "application/pdf", filePath);
			}
		}

		[HttpPost]
		public IActionResult SendConfirmationMail()
		{
			return View();
		}
	}
}

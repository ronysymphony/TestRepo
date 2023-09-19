using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Shampan.Services
{
	public static class MailService
	{

		
		public static bool SendAuditApprovalMail(string to,string Approvurl)
		{
			; string subject = "Approval For Audit"; string mailBody; string url = Approvurl; string userFullName = "Sir"; Attachment pdfLink;
			//Uri returnurl = new Uri(Request.Host.ToString());
			var sent = false;
			subject = "Approval For Audit";
			

			var bodyString = "<tr>" +
								 "<td style = 'background-color:#005daa;height:50px;font-size:30px;color:#fff;'>Approval For Audit </td>" +
							"</tr>" +
						  "<tr> " +
								"<td style = 'text-align:left;'>" +
									"<div style = 'margin-left:10px;color:black;'> " +

											"<div style='margin:10px 0 20px 0'><font size = '2' ><span style='font-size:10pt'> Dear " + userFullName + ",</span></font></div>" +

											"<div><font face = 'Calibri' size = '2'><span style = 'font-size:10pt'> Approval for Audit has been pending at your end in Audit Software and requires your attention. </span></font></div>" +
											  "<div><font face = 'Calibri' size = '2'><span style = 'font-size:10pt'> Please review this request and choose to approve or deny it, as appropriate. View and respond to your Approvals in your system.</span></font></div>" +
											   "<div><font face = 'Calibri' size = '2'><span style = 'font-size:10pt'> &nbsp;</span></font><div>" +
											   "<div><font size = '2' color = 'red'><span style = 'font-size:10pt'><b> N.B.This email is system generated.</b></span></font></div>" +
											   "<div> &nbsp;</div> " +
									   "</div> " +
								   "</td> " +
							"</tr> " +
							"<tr>" +
								"<td  style='height:50px;'> " +
										 "<a href='" + url + "' style = 'text-decoration: none;margin:10px 0px 30px 0px;border-radius:4px;padding:10px 20px;border: 0;color:#fff;background-color:#005daa;'> Click Here For Approve</a>" +
								"</td>" +
							"</tr> ";

			var message = new MailMessage();
			message.To.Add(new MailAddress(to));
			message.Subject = subject;

			sent = SendMail(to, bodyString, message);

			return sent;
		}

		public static bool SendMail(string To, string body, MailMessage message)
		{


			var content = "<html>" +
								"<head>" +
								"</head>" +
								"<body>" +
									"<table border='0' width='100%' style='margin:auto;padding:10px;background-color: #F3F3F3;border:1px solid #0C143B;'>" +
										"<tr>" +
											"<td>" +
												"<table border='0' width='100%'>" +
													"<tr>" +
														"<td style='text-align: center;'>" +
															"<h1>" +
																"<img src='cid:companylogo'  width='350px' height='80px'/> " +
															"</h1>" +
														"</td>" +
													"</tr>" +
												"</table>" +
											"</td>" +
										"</tr>" +
										"<tr>" +
											"<td>" +
												"<table border='0' cellpadding='0' cellspacing='0' style='text-align:center;width:100%;background-color: #FFFF;'>" +
												body
												+ "</table>" +
											"</td>" +
										"</tr>" +
										"<tr>" +
											"<td>" +
												"<table border='0' width='100%' style='border-radius: 5px;text-align: center;'>" +


													"<tr>" +
														"<td>" +
															"<div style='margin-top: 20px;color:black;'>" +


															"</div>" +
														"</td>" +
													"</tr>" +
												"</table>" +
											"</td>" +
										"</tr>" +
									"</table>" +
								"</body>" +
								"</html>";


			AlternateView av1 = AlternateView.CreateAlternateViewFromString(content, null, MediaTypeNames.Text.Html);




			message.IsBodyHtml = true;




			using (MailMessage mail = new MailMessage())
			{
				//mail.From = new MailAddress("hassanuzzaman.rony@symphonysoftt.com");
				mail.From = new MailAddress("auditgdic@gmail.com");
				mail.To.Add(To);
				mail.Subject = "Approval For Audit";
				mail.Body = body;
				mail.IsBodyHtml = true;
				

				using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
				{
                    //smtp.Credentials = new NetworkCredential("hassanuzzaman.rony@symphonysoftt.com", "H123456_");
                    //smtp.Credentials = new NetworkCredential("auditgdic@gmail.com", "auditgdic@123456");
                    smtp.Credentials = new NetworkCredential("auditgdic@gmail.com", "hbdf subq yxfl tboj");
                    smtp.EnableSsl = true;

                    //add new
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    //smtp.Timeout = 10000; 

                    smtp.Send(mail);
					return true;
				}
			}






		}
	}
}

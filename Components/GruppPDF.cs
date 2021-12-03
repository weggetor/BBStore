using Newtonsoft.Json;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bitboxx.DNNModules.BBStore.Components
{
	public static class GruppPDF
	{
		public static byte[] printPDF(int orderId)
		{

			const int RECHADRESSTYPE = 173;
			const int LIEFERADRESSTYPE = 174;
			const int EINSATZPRODUCTID = 8212;
			const int MONTEURPRODUCTID = 9385;
			const int KUNDEPRODUCTID = 8213;
			const int MAXYPOS = 710;
			const string TEMPLATEPATH = @"D:\0009_gruppapp\dnn\Portals\0\Images\mail\Montagebericht_leer.png";

			BBStoreController controller = new BBStoreController();

			OrderInfo order = controller.GetOrder(orderId);
			List<OrderAddressInfo> addresses = controller.GetOrderAddresses(orderId);
			OrderAddressInfo rechAddress = addresses.Where(a => a.SubscriberAddressTypeId == RECHADRESSTYPE).FirstOrDefault();
			OrderAddressInfo lieferAddress = addresses.Where(a => a.SubscriberAddressTypeId == LIEFERADRESSTYPE).FirstOrDefault();

			List<OrderProductInfo> products = controller.GetOrderProducts(orderId);
			List<OrderProductInfo> einsatzProducts = products.Where(p => p.ProductId == EINSATZPRODUCTID).OrderBy(p => p.OrderProductId).ToList();
			OrderProductInfo monteurProduct = products.Where(p => p.ProductId == MONTEURPRODUCTID).FirstOrDefault();
			GruppMonteur monteur = new GruppMonteur(controller.GetOrderProductOptions(monteurProduct.OrderProductId));
			OrderProductInfo kundeProduct = products.Where(p => p.ProductId == KUNDEPRODUCTID).FirstOrDefault();
			GruppKunde kunde = new GruppKunde(controller.GetOrderProductOptions(kundeProduct.OrderProductId));

			List<GruppEinsatz> einsaetze = new List<GruppEinsatz>();
			foreach (var einsatzProduct in einsatzProducts)
			{
				var einsatz = new GruppEinsatz(controller.GetOrderProductOptions(einsatzProduct.OrderProductId));
				if (einsatz.EntfernungRueckfahrt.Trim() == String.Empty && einsatz.Rueckfahrzeit.Trim() == String.Empty)
				{
					einsatz.EntfernungRueckfahrt = monteur.EntfernungRueckfahrt;
					einsatz.Rueckfahrzeit = monteur.ZeitRueckfahrt;
				}
				einsaetze.Add(einsatz);
			}

			PdfDocument doc = new PdfDocument();
			PdfPage page = doc.AddPage();
			XGraphics gfx = XGraphics.FromPdfPage(page);

			drawBackground(gfx, TEMPLATEPATH);

			drawAddress(gfx, 124, rechAddress, lieferAddress);
			drawDateblock(gfx, 124, order);

			writeMontageHeader(gfx, 250, order);
			writeHeaderRed(gfx, "Montagedetails", 272);

			var yPos = 296;
			foreach (var einsatz in einsaetze)
			{
				writeMachineHeader(gfx, yPos, einsatz);
				yPos += 22;
				writeTableHeader(gfx, yPos);
				yPos += 10;
				writeTableRow(gfx, yPos, einsatz);
				yPos += 34;

				string taetigkeit = einsatz.Taetigkeit;
				if (taetigkeit.Trim() != String.Empty)
				{
					var height = measureTableAddInfo(gfx, taetigkeit);
					if (yPos + height >= MAXYPOS)
					{
						page = doc.AddPage();
						gfx = XGraphics.FromPdfPage(page);
						drawBackground(gfx, TEMPLATEPATH);
						yPos = 130;
					}
					writeTableAddInfo(gfx, "Tätigkeit", taetigkeit, yPos);
					yPos += height;
				}

				string notizen = einsatz.NotizenMonteur;
				if (notizen.Trim() != String.Empty)
				{
					var height = measureTableAddInfo(gfx, notizen);
					if (yPos + height >= MAXYPOS)
					{
						page = doc.AddPage();
						gfx = XGraphics.FromPdfPage(page);
						drawBackground(gfx, TEMPLATEPATH);
						yPos = 130;
					}
					writeTableAddInfo(gfx, "Notizen Monteur", notizen, yPos);
					yPos += height;
				}

				string ersatzEingebaut = einsatz.EingebauteErsatzteile;
				if (ersatzEingebaut.Trim() != String.Empty)
				{
					var height = measureTableAddInfo(gfx, ersatzEingebaut);
					if (yPos + height >= MAXYPOS)
					{
						page = doc.AddPage();
						gfx = XGraphics.FromPdfPage(page);
						drawBackground(gfx, TEMPLATEPATH);
						yPos = 130;
					}
					writeTableAddInfo(gfx, "Eingebaute Ersatzteile", ersatzEingebaut, yPos);
					yPos += height;
				}

				string ersatzBenoetigt = einsatz.BenoetigteErsatzteile;
				if (ersatzBenoetigt.Trim() != String.Empty)
				{
					var height = measureTableAddInfo(gfx, ersatzBenoetigt);
					if (yPos + height >= MAXYPOS)
					{
						page = doc.AddPage();
						gfx = XGraphics.FromPdfPage(page);
						drawBackground(gfx, TEMPLATEPATH);
						yPos = 130;
					}
					writeTableAddInfo(gfx, "Benötigte Ersatzteile", ersatzBenoetigt, yPos);
					yPos += height;
				}

				string angebotBestellung = einsatz.AngebotBestellung;
				if (!String.IsNullOrWhiteSpace(angebotBestellung))
				{
					var height = measureTableAddInfo(gfx, angebotBestellung);
					if (yPos + height >= MAXYPOS)
					{
						page = doc.AddPage();
						gfx = XGraphics.FromPdfPage(page);
						drawBackground(gfx, TEMPLATEPATH);
						yPos = 130;
					}
					writeTableAddInfo(gfx, "Angebot oder Bestellung", angebotBestellung, yPos);
					yPos += height;
				}


			}

			if (kunde.Bemerkung != String.Empty)
			{
				var height = 90;
				if (yPos + height >= MAXYPOS)
				{
					page = doc.AddPage();
					gfx = XGraphics.FromPdfPage(page);
					drawBackground(gfx, TEMPLATEPATH);
					yPos = 130;
				}
				writeHeaderRed(gfx, "Anmerkungen", yPos);
				writeHeaderGrey(gfx, "Bemerkung Kunde", yPos + 24);
				writeInfo(gfx, kunde.Bemerkung, yPos + 56);
				yPos += height;
			}


			var signatureHeight = 190;
			if (yPos + signatureHeight >= MAXYPOS)
			{
				page = doc.AddPage();
				gfx = XGraphics.FromPdfPage(page);
				drawBackground(gfx, TEMPLATEPATH);
				yPos = 130;
			}
			writeSignature(gfx, yPos, kunde, monteur);

			// DrawHelpLines(gfx, 10);

			MemoryStream stream = new MemoryStream();
			doc.Save(stream, false);
			byte[] bytes = stream.ToArray();
			return bytes;
		}

		private static void drawBackground(XGraphics gfx, string imagePath)
		{
			XImage img = XImage.FromFile(imagePath);
			gfx.DrawImage(img, new XPoint(0, 0));
		}

		private static void drawAddress(XGraphics gfx, int yPos, OrderAddressInfo rechAddress, OrderAddressInfo lieferAddress)
		{
			const int xPos = 50;
			XFont font = new XFont("Century Gothic", 9);
			if (rechAddress != null)
			{
				gfx.DrawString(rechAddress.Company.Trim(), font, XBrushes.Black, new XPoint(xPos, yPos));
				gfx.DrawString((rechAddress.Firstname.Trim() + " " + rechAddress.Lastname.Trim()).Trim(), font, XBrushes.Black, new XPoint(xPos, yPos + 10));
				gfx.DrawString(rechAddress.Street.Trim(), font, XBrushes.Black, new XPoint(xPos, yPos + 20));
				gfx.DrawString((rechAddress.PostalCode.Trim() + " " + rechAddress.City.Trim()).Trim(), font, XBrushes.Black, new XPoint(xPos, yPos + 30));
			}

			gfx.DrawString("LIEFERADRESSE:", new XFont("Century Gothic", 9), new XSolidBrush(XColor.FromArgb(89, 89, 89)), new XPoint(xPos, yPos + 50));

			if (lieferAddress != null && rechAddress.Street != lieferAddress.Street)
			{
				gfx.DrawString(rechAddress.Company.Trim(), font, XBrushes.Black, new XPoint(xPos, yPos + 60));
				gfx.DrawString((lieferAddress.Firstname.Trim() + " " + lieferAddress.Lastname.Trim()).Trim(), font, XBrushes.Black, new XPoint(xPos, yPos + 70));
				gfx.DrawString(lieferAddress.Street.Trim(), font, XBrushes.Black, new XPoint(xPos, yPos + 80));
				gfx.DrawString((lieferAddress.PostalCode.Trim() + " " + lieferAddress.City.Trim()).Trim(), font, XBrushes.Black, new XPoint(xPos, yPos + 90));
			}
			else
			{
				gfx.DrawString("wie Rechnungsadresse", new XFont("Arial", 9), XBrushes.Black, new XPoint(xPos, yPos + 60));
			}
		}

		private static void drawDateblock(XGraphics gfx, int yPos, OrderInfo order)
		{
			var orderExtra = JsonConvert.DeserializeObject<dynamic>(order.Comment);

			const int xPosCaption = 375;
			const int xPosContent = 445;
			XFont font = new XFont("Century Gothic", 9);
			var greyBrush = new XSolidBrush(XColor.FromArgb(89, 89, 89));

			gfx.DrawString("Datum", font, greyBrush, new XPoint(xPosCaption, yPos));
			gfx.DrawString("Ihre Kd.-Nr.", font, greyBrush, new XPoint(xPosCaption, yPos + 10));
			gfx.DrawString("Ihre Daten", font, greyBrush, new XPoint(xPosCaption, yPos + 20));
			gfx.DrawString("Ihr Telefon", font, greyBrush, new XPoint(xPosCaption, yPos + 30));
			gfx.DrawString("Zuständig", font, greyBrush, new XPoint(xPosCaption, yPos + 40));
			gfx.DrawString("Telefon", font, greyBrush, new XPoint(xPosCaption, yPos + 50));
			gfx.DrawString("Fax", font, greyBrush, new XPoint(xPosCaption, yPos + 60));
			gfx.DrawString("E-Mail", font, greyBrush, new XPoint(xPosCaption, yPos + 70));


			gfx.DrawString(order.OrderTime.ToShortDateString(), font, XBrushes.Black, new XPoint(xPosContent, yPos));
			gfx.DrawString(orderExtra.IhreKundenNummer.ToString(), font, XBrushes.Black, new XPoint(xPosContent, yPos + 10));
			gfx.DrawString(orderExtra.IhreDaten.ToString(), font, XBrushes.Black, new XPoint(xPosContent, yPos + 20));
			gfx.DrawString(orderExtra.IhrTelefon.ToString(), font, XBrushes.Black, new XPoint(xPosContent, yPos + 30));
			gfx.DrawString(orderExtra.Zuständig.ToString(), font, XBrushes.Black, new XPoint(xPosContent, yPos + 40));
			gfx.DrawString(orderExtra.Telefon.ToString(), font, XBrushes.Black, new XPoint(xPosContent, yPos + 50));
			gfx.DrawString(orderExtra.Fax.ToString(), font, XBrushes.Black, new XPoint(xPosContent, yPos + 60));
			gfx.DrawString(orderExtra["E-Mail"].ToString(), font, XBrushes.Black, new XPoint(xPosContent, yPos + 70));
		}

		private static void writeMontageHeader(XGraphics gfx, double yPos, OrderInfo order)
		{
			const int xPos = 50;
			var redBrush = new XSolidBrush(XColor.FromArgb(232, 32, 32));
			var font = new XFont("Century Gothic", 18, XFontStyle.Bold);

			var orderExtra = JsonConvert.DeserializeObject<dynamic>(order.Comment);
			gfx.DrawString("MONTAGEBERICHT " + orderExtra.Auftrag.ToString(), font, redBrush, new XPoint(xPos, yPos));
		}

		private static void writeHeaderRed(XGraphics gfx, String text, double yPos)
		{
			const int xPos = 50;
			var font = new XFont("Century Gothic", 16, XFontStyle.Bold);
			gfx.DrawRectangle(new XSolidBrush(XColor.FromArgb(232, 32, 32)), new XRect(xPos, yPos, 500, 20));
			gfx.DrawString(text, font, XBrushes.White, new XPoint(xPos + 5, yPos + 16));
		}

		private static void writeHeaderGrey(XGraphics gfx, String text, double yPos)
		{
			const int xPos = 50;
			var greyBrush = new XSolidBrush(XColor.FromArgb(89, 89, 89));
			var font = new XFont("Century Gothic", 10, XFontStyle.Bold);
			gfx.DrawRectangle(greyBrush, new XRect(xPos, yPos, 500, 18));
			gfx.DrawString(text, font, XBrushes.White, new XPoint(xPos + 5, yPos + 13));
		}

		private static void writeMachineHeader(XGraphics gfx, double yPos, GruppEinsatz einsatz)
		{
			string text = einsatz.Fabrikat + " " + einsatz.Typ + ", " + einsatz.Baujahr + ", " + einsatz.Maschinennummer;

			const int xPos = 350;
			writeHeaderGrey(gfx, text, yPos);
			gfx.DrawRectangle(XBrushes.White, new XRect(xPos, yPos + 4, 10, 10));
			gfx.DrawString("Kleinteile Pauschal", new XFont("Century Gothic", 10, XFontStyle.Bold), XBrushes.White, new XPoint(xPos + 15, yPos + 13));
			if (einsatz.KleinteilePauschal == "true")
			{
				gfx.DrawString("X", new XFont("Century Gothic", 10, XFontStyle.Bold), XBrushes.Black, new XPoint(xPos + 2, yPos + 13));
			}

			gfx.DrawRectangle(XBrushes.White, new XRect(xPos + 118, yPos + 4, 10, 10));
			gfx.DrawString("Probelauf i.O.", new XFont("Century Gothic", 10, XFontStyle.Bold), XBrushes.White, new XPoint(xPos + 133, yPos + 13));
			if (einsatz.ProbelaufInOrdnung == "true")
			{
				gfx.DrawString("X", new XFont("Century Gothic", 10, XFontStyle.Bold), XBrushes.Black, new XPoint(xPos + 119, yPos + 13));
			}
		}

		private static void writeTableHeader(XGraphics gfx, double yPos)
		{
			const int xPos = 50;

			var font = new XFont("Century Gothic", 8, XFontStyle.Bold);
			var borderPen = new XPen(XColors.Black, 0.1);
			gfx.DrawRectangle(borderPen, new XRect(xPos, yPos, 60, 10));
			gfx.DrawRectangle(borderPen, new XRect(xPos + 60, yPos, 90, 10));
			gfx.DrawRectangle(borderPen, new XRect(xPos + 150, yPos, 100, 10));
			gfx.DrawRectangle(borderPen, new XRect(xPos + 250, yPos, 75, 10));
			gfx.DrawRectangle(borderPen, new XRect(xPos + 325, yPos, 75, 10));
			gfx.DrawRectangle(borderPen, new XRect(xPos + 400, yPos, 100, 10));

			gfx.DrawString("Datum", font, XBrushes.Black, new XPoint(xPos + 3, yPos + 8));
			gfx.DrawString("Monteur", font, XBrushes.Black, new XPoint(xPos + 63, yPos + 8));
			gfx.DrawString("An- und Abreise km", font, XBrushes.Black, new XPoint(xPos + 153, yPos + 8));
			gfx.DrawString("Anreisezeit", font, XBrushes.Black, new XPoint(xPos + 253, yPos + 8));
			gfx.DrawString("Rückreisezeit", font, XBrushes.Black, new XPoint(xPos + 328, yPos + 8));
			gfx.DrawString("Arbeitszeit", font, XBrushes.Black, new XPoint(xPos + 403, yPos + 8));
		}

		private static void writeTableRow(XGraphics gfx, double yPos, GruppEinsatz einsatz)
		{
			const int xPos = 50;

			var font = new XFont("Century Gothic", 9);
			var borderPen = new XPen(XColors.Black, 0.1);
			gfx.DrawRectangle(borderPen, new XRect(xPos, yPos, 60, 16));
			gfx.DrawRectangle(borderPen, new XRect(xPos + 60, yPos, 90, 16));
			gfx.DrawRectangle(borderPen, new XRect(xPos + 150, yPos, 100, 16));
			gfx.DrawRectangle(borderPen, new XRect(xPos + 250, yPos, 75, 16));
			gfx.DrawRectangle(borderPen, new XRect(xPos + 325, yPos, 75, 16));
			gfx.DrawRectangle(borderPen, new XRect(xPos + 400, yPos, 100, 16));

			gfx.DrawString(einsatz.Einsatzdatum, font, XBrushes.Black, new XPoint(xPos + 3, yPos + 12));
			gfx.DrawString(einsatz.Mitarbeiter, font, XBrushes.Black, new XPoint(xPos + 63, yPos + 12));
			gfx.DrawString(einsatz.EntfernungHinfahrt + " + " + einsatz.EntfernungRueckfahrt, font, XBrushes.Black, new XPoint(xPos + 153, yPos + 12));
			gfx.DrawString(einsatz.Hinfahrzeit, font, XBrushes.Black, new XPoint(xPos + 253, yPos + 12));
			gfx.DrawString(einsatz.Rueckfahrzeit, font, XBrushes.Black, new XPoint(xPos + 328, yPos + 12));
			gfx.DrawString(einsatz.Arbeitszeit, font, XBrushes.Black, new XPoint(xPos + 403, yPos + 12));
		}

		private static void writeTableAddInfo(XGraphics gfx, string title, string text, int yPos)
		{
			const int xPos = 50;

			var fontHeader = new XFont("Century Gothic", 10, XFontStyle.Bold);
			var fontText = new XFont("Century Gothic", 10);

			var lines = 0;
			var paragraphs = text.Split('\n');
			foreach (var para in paragraphs)
			{
				var linePara = GetSplittedLineCount(gfx, text, fontText, 450);
				lines += linePara;
			}

			gfx.DrawString(title, fontHeader, XBrushes.Black, new XPoint(xPos, yPos));

			XTextFormatter myTf = new XTextFormatter(gfx);
			myTf.Alignment = XParagraphAlignment.Justify;
			XRect rectangle = new XRect(xPos, yPos + 2, 500, (lines * 13));
			myTf.DrawString(text, fontText, XBrushes.Black, rectangle);
		}

		private static void writeInfo(XGraphics gfx, string text, int yPos)
		{
			const int xPos = 50;
			var fontText = new XFont("Century Gothic", 10);
			gfx.DrawString(text, fontText, XBrushes.Black, new XPoint(xPos, yPos));
		}

		private static int measureTableAddInfo(XGraphics gfx, string text)
		{
			var fontText = new XFont("Century Gothic", 10);
			var lines = 0;
			var paragraphs = text.Split('\n');
			foreach (var para in paragraphs)
			{
				var linePara = GetSplittedLineCount(gfx, text, fontText, 450);
				lines += linePara;
			}
			return 18 + lines * 13;
		}

		private static void writeSignature(XGraphics gfx, int yPos, GruppKunde kunde, GruppMonteur monteur)
		{
			// Höhe ca. 190 Pixel
			const int xPos = 50;

			writeHeaderRed(gfx, "Unterschriften", yPos);
			XFont font1 = new XFont("Century Gothic", 9);
			XFont font2 = new XFont("Century Gothic", 8);
			XFont font3 = new XFont("Century Gothic", 6, XFontStyle.Bold);
			string text1 = "Die Rückreisezeit und -km kann erst nach Beendigung der Montage ermittelt werden.";
			string text2 = "Hiermit bestätigen wir, dass die Arbeiten sachgemäß erledigt sind und die Maschine/Anlage " +
						   "in einem einwandfreien Zustand vorgeführt und abgenommen wurde. Der oben stehende Bericht trifft zu. " +
						   "Sicherheits- und Schutzeinrichtungen wurden erklärt. Grundlage sind unsere Allgemeinen Geschäftsbedingungen." +
						   "Diese können Sie bei uns anfordern.";

			XTextFormatter tf = new XTextFormatter(gfx);
			tf.Alignment = XParagraphAlignment.Justify;
			tf.DrawString(text1, font1, XBrushes.Black, new XRect(xPos, yPos + 30, 500, 10));
			tf.DrawString(text2, font2, XBrushes.Black, new XRect(xPos, yPos + 50, 500, 30));

			gfx.DrawLine(new XPen(XColors.Black, 0.1), new XPoint(xPos, yPos + 135), new XPoint(xPos + 165, yPos + 135));
			gfx.DrawLine(new XPen(XColors.Black, 0.1), new XPoint(xPos + 260, yPos + 135), new XPoint(550, yPos + 135));

			try { gfx.DrawImage(XImage.FromStream(new MemoryStream(monteur.Unterschrift)), new XRect(xPos, yPos + 88, 80, 40)); } catch  { }
			gfx.DrawString(formatDate(monteur.Datum), font1, XBrushes.Black, new XPoint(xPos + 90, yPos + 132));
			gfx.DrawString("Unterschrift Fa. GRUPP                 Datum", font3, XBrushes.Black, new XPoint(xPos, yPos + 142));

			try { gfx.DrawImage(XImage.FromStream(new MemoryStream(kunde.Unterschrift)), new XRect(xPos + 260, yPos + 88, 80, 40)); } catch { }
			gfx.DrawString(formatDate(kunde.Datum) + "   " + kunde.Name, font1, XBrushes.Black, new XPoint(xPos + 350, yPos + 132));
			gfx.DrawString("Unterschrift Auftraggeber           Datum                    Druckbuchstaben", font3, XBrushes.Black, new XPoint(xPos + 260, yPos + 142));
		}

		private static int GetSplittedLineCount(XGraphics gfx, string content, XFont font, double maxWidth)
		{
			//handy function for creating list of string
			Func<string, IList<string>> listFor = val => new List<string> { val };
			// string.IsNullOrEmpty is too long :p
			Func<string, bool> nOe = str => string.IsNullOrEmpty(str);
			// return a space for an empty string (sIe = Space if Empty)
			Func<string, string> sIe = str => nOe(str) ? " " : str;
			// check if we can fit a text in the maxWidth
			Func<string, string, bool> canFitText = (t1, t2) => gfx.MeasureString($"{(nOe(t1) ? "" : $"{t1} ")}{sIe(t2)}", font).Width <= maxWidth;

			Func<IList<string>, string, IList<string>> appendtoLast =
					(list, val) => list.Take(list.Count - 1)
									   .Concat(listFor($"{(nOe(list.Last()) ? "" : $"{list.Last()} ")}{sIe(val)}"))
									   .ToList();

			var splitted = content.Split(' ');

			var lines = splitted.Aggregate(listFor(""),
					(lfeed, next) => canFitText(lfeed.Last(), next) ? appendtoLast(lfeed, next) : lfeed.Concat(listFor(next)).ToList(),
					list => list.Count());

			return lines;
		}

		private static void DrawHelpLines(XGraphics gfx, int steps)
		{
			for (int i = 0; i <= 600; i = i + steps)
			{
				gfx.DrawString(i.ToString(), new XFont("Arial", 8), XBrushes.Black, new XPoint(i + 3, steps));
				XColor color = (i % 100 == 0) ? XColors.Black : (i % 50 == 0) ? XColors.Gray : XColors.LightGray;
				gfx.DrawLine(new XPen(color), new XPoint(i, 0), new XPoint(i, 850));
			}

			for (int j = 0; j <= 850; j = j + steps)
			{
				gfx.DrawString(j.ToString(), new XFont("Arial", 8), XBrushes.Black, new XPoint(10, j + steps));
				XColor color = (j % 100 == 0) ? XColors.Black : (j % 50 == 0) ? XColors.Gray : XColors.LightGray;
				gfx.DrawLine(new XPen(color), new XPoint(0, j), new XPoint(600, j));
			}
		}

		private static string formatTime(string time)
		{
			if (time == String.Empty) return String.Empty;
			string result = "";
			result = time.Replace(".", "")
				.Replace("Stunden", "Std")
				.Replace("Minuten", "Min")
				.Replace("std", "Std")
				.Replace("min", "Min")
				.Replace("Std", "Std.")
				.Replace("Min", "Min.");

			if (result.Trim().Split(' ').Length == 2)
			{
				if (result.IndexOf("Std.") > -1)
				{
					result = result + " 0 Min.";
				}
				else
				{
					result = "0 Std. " + result;
				}
			}

			return result;
		}

		private static string formatDate(string dateStr)
		{
			if (dateStr == String.Empty) return String.Empty;
			DateTime date;
			if (DateTime.TryParse(dateStr, out date))
			{
				return date.ToString("dd.MM.yyyy");
			}
			else
			{
				return dateStr;
			}

		}

		private class GruppEinsatz
		{
			public GruppEinsatz(List<OrderProductOptionInfo> options)
			{
				this.Taetigkeit = options.FirstOrDefault(o => o.OptionName == "Tätigkeit")?.OptionValue;
				this.Maschinenbezeichnung = options.FirstOrDefault(o => o.OptionName == "Maschinenbezeichnung")?.OptionValue;
				this.Fabrikat = options.FirstOrDefault(o => o.OptionName == "Fabrikat")?.OptionValue;
				this.Typ = options.FirstOrDefault(o => o.OptionName == "Typ")?.OptionValue;
				this.Baujahr = options.FirstOrDefault(o => o.OptionName == "Baujahr")?.OptionValue;
				this.Maschinennummer = options.FirstOrDefault(o => o.OptionName == "Maschinennummer")?.OptionValue;
				this.Mitarbeiter = options.FirstOrDefault(o => o.OptionName == "Mitarbeiter")?.OptionValue;
				this.Einsatzdatum = formatDate(options.FirstOrDefault(o => o.OptionName == "Einsatzdatum")?.OptionValue);
				this.Arbeitszeit = formatTime(options.FirstOrDefault(o => o.OptionName == "Arbeitszeit")?.OptionValue);
				this.Hinfahrzeit = formatTime(options.FirstOrDefault(o => o.OptionName == "Hinfahrzeit")?.OptionValue);
				this.Rueckfahrzeit = formatTime(options.FirstOrDefault(o => o.OptionName == "Rückfahrzeit")?.OptionValue);
				this.EntfernungHinfahrt = options.FirstOrDefault(o => o.OptionName == "Entfernung Hinfahrt")?.OptionValue;
				this.EntfernungRueckfahrt = options.FirstOrDefault(o => o.OptionName == "Entfernung Rückfahrt")?.OptionValue;
				this.NotizenMonteur = options.FirstOrDefault(o => o.OptionName == "Notizen Monteur")?.OptionValue;
				this.KleinteilePauschal = options.FirstOrDefault(o => o.OptionName == "Kleinteile pauschal")?.OptionValue;
				this.ProbelaufInOrdnung = options.FirstOrDefault(o => o.OptionName == "Probelauf in Ordnung")?.OptionValue;
				this.EingebauteErsatzteile = options.FirstOrDefault(o => o.OptionName == "Eingebaute Ersatzteile")?.OptionValue;
				this.BenoetigteErsatzteile = options.FirstOrDefault(o => o.OptionName == "Benötigte Ersatzteile")?.OptionValue;
				this.AngebotBestellung = options.FirstOrDefault(o => o.OptionName == "Angebot oder Bestellung")?.OptionValue;
			}

			public string Taetigkeit { get; set; }
			public string Maschinenbezeichnung { get; set; }
			public string Fabrikat { get; set; }
			public string Typ { get; set; }
			public string Baujahr { get; set; }
			public string Maschinennummer { get; set; }
			public string Mitarbeiter { get; set; }
			public string Einsatzdatum { get; set; }
			public string Arbeitszeit { get; set; }
			public string Hinfahrzeit { get; set; }
			public string Rueckfahrzeit { get; set; }
			public string EntfernungHinfahrt { get; set; }
			public string EntfernungRueckfahrt { get; set; }
			public string NotizenMonteur { get; set; }
			public string KleinteilePauschal { get; set; }
			public string ProbelaufInOrdnung { get; set; }
			public string EingebauteErsatzteile { get; set; }
			public string BenoetigteErsatzteile { get; set; }
			public string AngebotBestellung { get; set; }
		}

		private class GruppMonteur
		{
			public GruppMonteur(List<OrderProductOptionInfo> options)
			{
				this.ZeitRueckfahrt = formatTime(options.FirstOrDefault(o => o.OptionName == "letzte Rückfahrzeit").OptionValue);
				this.EntfernungRueckfahrt = options.FirstOrDefault(o => o.OptionName == "Entfernung letzte Rückfahrt").OptionValue;
				this.Datum = options.FirstOrDefault(o => o.OptionName == "Datum").OptionValue;
				this.Name = options.FirstOrDefault(o => o.OptionName == "Monteur").OptionValue;
				this.Unterschrift = options.FirstOrDefault(o => o.OptionName == "Unterschrift Monteur").OptionImage;
				this.Bemerkung = options.FirstOrDefault(o => o.OptionName == "Notizen intern").OptionValue;
			}

			public string ZeitRueckfahrt { get; set; }
			public string EntfernungRueckfahrt { get; set; }
			public string Datum { get; set; }
			public string Name { get; set; }
			public byte[] Unterschrift { get; set; }
			public string Bemerkung { get; set; }
		}

		private class GruppKunde
		{
			public GruppKunde(List<OrderProductOptionInfo> options)
			{
				this.Datum = options.FirstOrDefault(o => o.OptionName == "Datum").OptionValue;
				this.Name = options.FirstOrDefault(o => o.OptionName == "Name").OptionValue;
				this.Unterschrift = options.FirstOrDefault(o => o.OptionName == "Unterschrift").OptionImage;
				this.Bemerkung = options.FirstOrDefault(o => o.OptionName == "Bemerkung Kunde").OptionValue;
				this.Email = options.FirstOrDefault(o => o.OptionName == "E-Mail").OptionValue;
			}

			public string Datum { get; set; }
			public string Name { get; set; }
			public byte[] Unterschrift { get; set; }
			public string Bemerkung { get; set; }
			public string Email { get; set; }
		}
	}
}
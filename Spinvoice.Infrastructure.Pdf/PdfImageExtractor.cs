using System;
using System.Globalization;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Image = System.Drawing.Image;

namespace Spinvoice.Infrastructure.Pdf
{
    public class PdfImageExtractor
    {
        public Image ExtractImagesFromPdf(PdfReader reader, int pageNumber)
        {
            var pg = reader.GetPageN(pageNumber);

            var imageObject = FindImageInPdfDictionary(pg);
            if (imageObject == null)
            {
                return null;
            }

            var xrefIndex = Convert.ToInt32(((PRIndirectReference)imageObject).Number.ToString(CultureInfo.InvariantCulture));
            var pdfObj = reader.GetPdfObject(xrefIndex);
            var pdfStream = (PRStream)pdfObj;

            var pdfImage = new PdfImageObject(pdfStream);
            var img = pdfImage.GetDrawingImage();

            return img;
        }

        private static PdfObject FindImageInPdfDictionary(PdfDictionary pg)
        {
            var res = (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
            var xobj = (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
            if (xobj == null) return null;

            foreach (var name in xobj.Keys)
            {
                var obj = xobj.Get(name);
                if (!obj.IsIndirect()) continue;

                var tg = (PdfDictionary)PdfReader.GetPdfObject(obj);

                var type = (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));

                if (PdfName.IMAGE.Equals(type))
                {
                    return obj;
                }
                if (PdfName.FORM.Equals(type))
                {
                    return FindImageInPdfDictionary(tg);
                }
                if (PdfName.GROUP.Equals(type))
                {
                    return FindImageInPdfDictionary(tg);
                }
            }

            return null;
        }
    }
}
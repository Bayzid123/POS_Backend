using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeLib;

public class BarCodeImage
{
    private Barcode b = new Barcode();

    public byte[] GetImage(string number, bool isShowNumber)
    {
        int width = 300;
        int height = 150;
        b.Alignment = AlignmentPositions.CENTER;
        TYPE tYPE = TYPE.CODE128;
        try
        {
            if (tYPE != 0)
            {
                b.IncludeLabel = isShowNumber;
                b.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), RotateFlipType.RotateNoneFlipNone.ToString(), ignoreCase: true);
                b.LabelPosition = LabelPositions.BOTTOMCENTER;
                Image image = b.Encode(tYPE, number, Color.Black, Color.White, width, height);
                MemoryStream memoryStream = new MemoryStream();
                image.Save(memoryStream, ImageFormat.Png);
                return memoryStream.ToArray();
            }
        }
        catch (Exception)
        {
            return null;
        }

        return null;
    }
}

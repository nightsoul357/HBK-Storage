using HBK.Storage.Core.FileSystem;
using HBK.Storage.Core.FileSystem.Memory;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HBK.Storage.Api.FileProcessHandlers
{
    /// <summary>
    /// 預設浮水印的檔案處理器
    /// </summary>
    public class WatermarkDefaultProcessHandler : FileProcessHandlerBase
    {
        /// <inheritdoc/>
        public async override Task<FileProcessTaskModel> ProcessAsync(FileProcessTaskModel taskModel)
        {
            if (!taskModel.FileEntity.MimeType.StartsWith("image/"))
            {
                return taskModel;
            }

            using Stream stream = await taskModel.FileInfo.CreateReadStreamAsync();
            using Bitmap sourceBmp = new Bitmap(stream);
            using Image logo = Image.FromFile("hbk_logo.png");
            using Bitmap logoBmp = this.ChangeOpacity(logo, 0.5f);
            using Graphics g = Graphics.FromImage(sourceBmp);

            float rectWidth = logo.Width + 10;
            float rectHeight = logo.Height + 10;
            RectangleF textArea = new RectangleF(0, 0, Math.Min(rectWidth, sourceBmp.Width), Math.Min(rectHeight, sourceBmp.Height));
            g.DrawImage(logoBmp, textArea);

            MemoryStream memoryStream = new MemoryStream();
            sourceBmp.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Seek(0, SeekOrigin.Begin);
            taskModel.FileInfo = new MemoryFileInfo(memoryStream);

            return taskModel;
        }
        /// <summary>
        /// 變更圖片透明度
        /// </summary>
        /// <param name="image">圖片</param>
        /// <param name="opacity">透明度(0~1)</param>
        /// <returns></returns>
        private Bitmap ChangeOpacity(Image image, float opacity)
        {

            Bitmap bmp = new Bitmap(image.Width, image.Height);

            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                //create a color matrix object  
                ColorMatrix matrix = new ColorMatrix();

                //set the opacity  
                matrix.Matrix33 = opacity;

                //create image attributes  
                ImageAttributes attributes = new ImageAttributes();

                //set the color(opacity) of the image  
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //now draw the image  
                gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                return bmp;
            }
        }
        /// <inheritdoc/>
        public override string Name => "watermark-default";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing.Imaging;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace OAuth2_Dotnet_UsingSDK
{
    public partial class ProfitReality : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int perc = 25; // TODO:  Get from User
            int userpercent_12 = Convert.ToInt32(Request.QueryString["up12"]);
            int userpercent_3 = Convert.ToInt32(Request.QueryString["up3"]); 

            int size = 360;
            int trianglesize = 5;
            Font drawFont = new Font("Arial", 8);

            

            Bitmap oCanvas = new Bitmap(size+(2*trianglesize), size+ (2 * trianglesize));
            Graphics g = Graphics.FromImage(oCanvas);
            g.Clear(Color.White);

            var linesize = (size - trianglesize);
            var midpoint = (size - trianglesize) / 2;

            int percincrement = midpoint / perc;

            using (Pen p = new Pen(Color.Black, 2))
            using (GraphicsPath capPath = new GraphicsPath())
            {
                p.StartCap = LineCap.ArrowAnchor;
                p.EndCap = LineCap.ArrowAnchor;
                g.DrawLine(p, 0, midpoint, linesize, midpoint);
                g.DrawLine(p, midpoint, 0, midpoint, linesize);

                var markerx = 0;
                var markery = 0;
                if (userpercent_12 >= 0)
                    markerx = midpoint + (percincrement * userpercent_12);
                else
                    markerx = midpoint - (percincrement * userpercent_12);

                if (userpercent_3 >= 0)
                    markery = midpoint - (percincrement * userpercent_3);
                else
                    markery = midpoint + (percincrement * userpercent_3);

                g.DrawEllipse(new Pen(Color.Blue), markerx, markery, 8, 8);

            }

            g.DrawString("12-months", drawFont, new SolidBrush(Color.Black), linesize - 60, midpoint - 20, null);
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
            g.DrawString("3-months", drawFont, new SolidBrush(Color.Black), midpoint - 20, 20, drawFormat);


            


            // Now, we only need to send it // to the client
            Response.ContentType = "image/jpeg";
            oCanvas.Save(Response.OutputStream, ImageFormat.Jpeg);
            Response.End();

            // Cleanup
            g.Dispose();
            oCanvas.Dispose();
        }
    }
}
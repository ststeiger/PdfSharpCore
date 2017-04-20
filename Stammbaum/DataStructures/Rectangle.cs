
namespace Stammbaum.DataStructures
{


    public class Rectangle
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;


        public Rectangle(double x, double y, double width, double height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }


        public static Rectangle FromXRect(PdfSharpCore.Drawing.XRect rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }



        public PdfSharpCore.Drawing.XRect ToXRect()
        {
            return new PdfSharpCore.Drawing.XRect(this.X, this.Y, this.Width, this.Height);
        }


        public double Left
        {
            get
            {
                return this.X;
            }
        }

        public double Right
        {
            get
            {
                return this.X + this.Width;
            }
        }

        public double Top
        {
            get
            {
                return this.Y;
            }
        }

        public double Bottom
        {
            get
            {
                return this.Y + this.Height;
            }
        }


        public Point TopLeft
        {
            get
            {
                return new Point(this.X, this.Y);
            }
        }

        public Point TopRight
        {
            get
            {
                return new Point(this.X + this.Width, this.Y);
            }
        }

        public Point BottomLeft
        {
            get
            {
                return new Point(this.X, this.Y + this.Height);
            }
        }

        public Point BottomRight
        {
            get
            {
                return new Point(this.X + this.Width, this.Y + this.Height);
            }
        }


    }


}

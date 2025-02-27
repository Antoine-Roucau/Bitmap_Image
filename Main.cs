namespace BitmapProject 
{
    
    class Program
    {

        static void Main(string[] args)
        {
            
            MyImage image = new MyImage("bin/Debug/net7.0/Image/coco");
            
            image.From_Image_To_File("cocoRevive");

        }

    }
}



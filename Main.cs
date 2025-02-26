namespace BitmapProject 
{
    
    class Program
    {

        static void Main(string[] args)
        {
            
            MyImage image = new MyImage("bin/Debug/net7.0/Image/coco.bmp");

            Huffman huffman = new Huffman();
            
            huffman.CompressImage(image,"test.txt");

            image = huffman.DecompressImage("test.txt");
            
            image.From_Image_To_File("cocoRevive");

        }

    }
}



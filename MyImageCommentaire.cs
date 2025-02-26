using System.Numerics;
using System.Runtime.Serialization;

namespace BitmapProject
{
    public class MyImage
    {
        /// <summary>
        /// Variable de la classe MyImage
        /// </summary>
        private string type;
        private string filname;
        private int taille_fill;
        private int taille_offset;
        private int largeur;
        private int hauteur;
        private int nb_byte_color;
        private (int, int, int)[,] image;

        /// <summary>
        /// Constructeur depuis un fichier
        /// </summary>
        public MyImage(string filname)
        {
            if (filname != null)
            {
                this.filname = filname;
                byte[] info = File.ReadAllBytes(filname + ".bmp");
                char c1 = Convert.ToChar(info[0]);
                char c2 = Convert.ToChar(info[1]);
                this.type = c1 + "" + c2;
                this.taille_fill = Little_EndianToDecimal(2, 6, info);
                this.taille_offset = Little_EndianToDecimal(10, 14, info);
                this.largeur = Little_EndianToDecimal(18, 22, info);
                this.hauteur = Little_EndianToDecimal(22, 26, info);
                this.nb_byte_color = Little_EndianToDecimal(28, 30, info);
                image = new (int, int, int)[this.hauteur, this.largeur];
                int g = 54;
                for (int i = 0; i < this.hauteur; i++)
                {
                    for (int j = 0; j < this.largeur; j++)
                    {
                        image[i, j] = (info[g], info[g + 1], info[g + 2]);
                        g = g + 3;
                    }
                }
                info = null;
            }
            else
            {
                /// <summary>
                /// Constructeur image null
                /// </summary>
                this.filname = "Null";
                this.type = "BM";
                this.taille_offset = 54;
                this.largeur = 4;
                this.hauteur = 4;
                this.taille_fill = 102;
                this.nb_byte_color = 24;
                image = new (int, int, int)[this.hauteur, this.largeur];
                for (int i = 0; i < this.hauteur; i++)
                {
                    for (int j = 0; j < this.largeur; j++)
                    {
                        image[i, j] = (0, 0, 0);
                    }
                }
            }
        }
        /// <summary>
        /// //Constructeur avec dimensions
        /// </summary>        
        public MyImage(int width, int height)
        {
            this.filname = "Null";
            this.type = "B M"; 
            this.taille_fill = width * height * 3 + 54; 
            this.taille_offset = 0; 
            this.largeur = width;
            this.hauteur = height;
            this.nb_byte_color = 24; 
            this.image = new(int, int, int)[height,width] ;
        }
        /// <summary>
        /// Acces Lecture Variables
        /// </summary>
        public string Filename
        { get { return filname; } }
        public string Type
        { get { return type; } }
        public int Taille_fill
        { get { return taille_fill; } }
        public int Taille_Offset
        { get { return taille_offset; } }
        public int Hauteur
        { get { return hauteur; } }
        public int Largeur
        { get { return largeur; } }
        public int Nb_byte_color
        { get { return nb_byte_color; } }
        public (int, int, int)[,] Image
        { get { return image; } }

        /// <summary>
        /// Conversion little_endian a int 
        /// </summary>
        public int Little_EndianToDecimal(int index1, int index2, byte[] info)
        {
            string taille_binary = "";
            //fabrication de la chaine bin de notre nombre
            for (int i = index2 - 1; i > index1 - 1; i--)
            {
                int number = info[i];
                int[] numberArray = new int[8];
                for (int g = 0; g < 8; g++)
                {
                    numberArray[g] = 0;
                }
                for (int j = 0; number > 0; j++)
                {
                    numberArray[j] = number % 2;
                    number = number / 2;
                }
                for (int k = 7; k > -1; k--)
                {
                    taille_binary = taille_binary + numberArray[k];
                }
            }
            
            int decimale = 0;
            int n = 2;
            //passage de notre string bin vers notre decimal
            if (taille_binary[taille_binary.Length - 1] == 1)
            {
                decimale++;
            }
            for (int i = 1; i < taille_binary.Length; i++)
            {
                if (taille_binary[taille_binary.Length - i - 1] == '1')
                {
                    decimale += n;
                }
                n = n * 2;
            }
            return decimale;
        }
        /// <summary>
        /// Conversion int a little_endian
        /// </summary>
        public byte[] DecimalToLittle_Endian(int number)
        {
            //byte du int32
            byte[] result = new byte[4];
            int count = 0;
            //liste de notre bin
            int[] numberArray = new int[32];
            //constitution de notre liste
            for (int j = 0; number > 0; j++)
            {
                numberArray[31 - j] = number % 2;
                number = number / 2;
                count++;
            }
            //passage to byte
            for (int i = 3; i > -1; i--)
            {
                int[] nb = new int[8];
                for (int h = i * 8; h < (i + 1) * 8; h++)
                {
                    nb[h - i * 8] = numberArray[h];
                }
                byte decimale = 0;
                int n = 2;
                if (nb[7] == 1)
                {
                    decimale++;
                }
                for (int k = 6; k > -1; k--)
                {
                    if (nb[k] == 1)
                    {
                        decimale += (byte)n;
                    }
                    n = n * 2;
                }
                result[3 - i] = decimale;
            }
            return result;
        }
        /// <summary>
        /// Fonction qui transforme l'image en fichier bmp
        /// </summary>
        public void From_Image_To_File(string file)
        {
            //creation de notre byte[] qui va contenir toute les infos de notre image
            byte[] newImage = new byte[this.taille_fill];
            //B
            newImage[0] = 66;
            //M
            newImage[1] = 77;
            //taille
            byte[] taille = DecimalToLittle_Endian(this.taille_fill);
            
            for (int t = 2; t < 6; t++)
            {
                newImage[t] = taille[t - 2];
            }
            //offset
            byte[] unchange = new byte[] { 0, 0, 0, 0, (byte)this.taille_offset, 0, 0, 0, 40, 0, 0, 0 };
            for (int t = 6; t < 18; t++)
            {
                newImage[t] = unchange[t - 6];
            }
            //largeur
            byte[] largeur = DecimalToLittle_Endian(this.largeur);
            for (int t = 18; t < 22; t++)
            {
                newImage[t] = largeur[t - 18];
            }
            //hauteur
            byte[] hauteur = DecimalToLittle_Endian(this.hauteur);
            for (int t = 22; t < 26; t++)
            {
                newImage[t] = hauteur[t - 22];
            }
            //nombre de bits pour la couleur
            unchange = new byte[] { 1, 0, (byte)this.nb_byte_color, 0, 0, 0, 0, 0 };
            for (int t = 26; t < 34; t++)
            {
                newImage[t] = unchange[t - 26];
            }
            //taille totale
            byte[] tailleImage = DecimalToLittle_Endian(this.taille_fill - this.taille_offset);
            for (int t = 34; t < 38; t++)
            {
                newImage[t] = tailleImage[t - 34];
            }
            for (int t = 38; t < 54; t++)
            {
                newImage[t] = 0;
            }
            int count = 54;
            //image
            for (int i = 0; i < this.hauteur; i++)
            {
                for (int j = 0; j < this.largeur; j++)
                {
                    newImage[count] = (byte)this.image[i, j].Item1;
                    newImage[count + 1] = (byte)this.image[i, j].Item2;
                    newImage[count + 2] = (byte)this.image[i, j].Item3;
                    count = count + 3;
                }
            }
            File.WriteAllBytes(file + ".bmp", newImage);
        }
        /// <summary>
        /// Filtre de passage en noir et blanc suivant l'equation R*0.3 + G*0.59 + B*0.11 = Couleur du pixel
        /// </summary>
        public void Grisage()
        {
            for (int i = 0; i < this.hauteur; i++)
            {
                for (int j = 0; j < this.largeur; j++)
                {
                    int grissonnant = Convert.ToInt32(this.image[i, j].Item1 * 0.3) + Convert.ToInt32(this.image[i, j].Item2 * 0.59) + Convert.ToInt32(this.image[i, j].Item3 * 0.11);
                    this.image[i, j].Item1 = grissonnant;
                    this.image[i, j].Item2 = grissonnant;
                    this.image[i, j].Item3 = grissonnant;
                }
            }
        }
        /// <summary>
        /// Filtre de passage en noir et blanc suivant l'equation (R+G+B)/3 = Couleur du pixel
        /// </summary>
        public void NoirEtBlanc()
        {
            for (int i = 0; i < this.hauteur; i++)
            {
                for (int j = 0; j < this.largeur; j++)
                {
                    int Mixe = (this.image[i, j].Item1 + this.image[i, j].Item2 + this.image[i, j].Item3) / 3;
                    this.image[i, j].Item1 = Mixe;
                    this.image[i, j].Item2 = Mixe;
                    this.image[i, j].Item3 = Mixe;
                }
            }
        }
        /// <summary>
        ///                                                 {-1,0,1}   {-1,-2,-1}
        /// Filtre detect bord 3x3 Operateur de Sobel       {-2,0,2} + { 0, 0, 0}
        /// Commentaire s'appliquant pour tout les filtres  {-1,0,1}   { 1, 2, 1}
        /// </summary>
        public void Detect_Bord_Type1()
        {
            //Matrice convolution Verticale
            (int, int, int)[,] image_bisVerticale = ((int, int, int)[,])this.image.Clone();
            //Matrice convolution Horizontale
            (int, int, int)[,] image_bisHorizontale = ((int, int, int)[,])this.image.Clone();
            //Verticale
            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    int count = 0;
                    int R = 0;
                    int G = 0;
                    int B = 0;

                    if (i > 0)
                    {
                        R -= image[i - 1, j].Item1 * 2; //haut
                        G -= image[i - 1, j].Item2 * 2;
                        B -= image[i - 1, j].Item3 * 2;
                        count += 1;
                        if (j > 0)
                        {
                            R -= image[i - 1, j - 1].Item1; //haut gauche
                            G -= image[i - 1, j - 1].Item2;
                            B -= image[i - 1, j - 1].Item3;
                            count += 1;
                        }
                        if (j < this.image.GetLength(1) - 1)
                        {
                            R -= image[i - 1, j + 1].Item1; //haut droite
                            G -= image[i - 1, j + 1].Item2;
                            B -= image[i - 1, j + 1].Item3;
                            count += 1;
                        }
                    }
                    if (i < this.image.GetLength(0) - 1)
                    {
                        R += image[i + 1, j].Item1 * 2; //bas
                        G += image[i + 1, j].Item2 * 2;
                        B += image[i + 1, j].Item3 * 2;
                        count += 1;
                        if (j > 0)
                        {
                            R += image[i + 1, j - 1].Item1; //bas gauche
                            G += image[i + 1, j - 1].Item2;
                            B += image[i + 1, j - 1].Item3;
                            count += 1;
                        }
                        if (j < this.image.GetLength(1) - 1)
                        {
                            R += image[i + 1, j + 1].Item1; //bas droite
                            G += image[i + 1, j + 1].Item2;
                            B += image[i + 1, j + 1].Item3;
                            count += 1;
                        }
                    }
                    if (count > 0)
                    {
                        image_bisVerticale[i, j].Item1 = Math.Min(Math.Max((R / count), 0), 255);
                        image_bisVerticale[i, j].Item2 = Math.Min(Math.Max((G / count), 0), 255);
                        image_bisVerticale[i, j].Item3 = Math.Min(Math.Max((B / count), 0), 255);
                    }
                }
            }

            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    int count = 0;
                    int R = 0;
                    int G = 0;
                    int B = 0;


                    if (i > 0)
                    {
                        if (j > 0)
                        {
                            R -= image[i - 1, j - 1].Item1; //haut gauche
                            G -= image[i - 1, j - 1].Item2;
                            B -= image[i - 1, j - 1].Item3;
                            count += 1;
                        }
                        if (j < this.image.GetLength(1) - 1)
                        {
                            R += image[i - 1, j + 1].Item1; //haut droite
                            G += image[i - 1, j + 1].Item2;
                            B += image[i - 1, j + 1].Item3;
                            count += 1;
                        }
                    }
                    if (j > 0)
                    {
                        R -= image[i, j - 1].Item1 * 2; //gauche
                        G -= image[i, j - 1].Item2 * 2;
                        B -= image[i, j - 1].Item3 * 2;
                        count += 1;
                    }
                    if (j < this.image.GetLength(1) - 1)
                    {
                        R += image[i, j + 1].Item1 * 2; //droite
                        G += image[i, j + 1].Item2 * 2;
                        B += image[i, j + 1].Item3 * 2;
                        count += 1;
                    }
                    if (i < this.image.GetLength(0) - 1)
                    {
                        if (j > 0)
                        {
                            R -= image[i + 1, j - 1].Item1; //bas gauche
                            G -= image[i + 1, j - 1].Item2;
                            B -= image[i + 1, j - 1].Item3;
                            count += 1;
                        }
                        if (j < this.image.GetLength(1) - 1)
                        {
                            R += image[i + 1, j + 1].Item1; //bas droite
                            G += image[i + 1, j + 1].Item2;
                            B += image[i + 1, j + 1].Item3;
                            count += 1;
                        }
                    }
                    if (count > 0)
                    {
                        image_bisHorizontale[i, j].Item1 = Math.Min(Math.Max((R / count), 0), 255);
                        image_bisHorizontale[i, j].Item2 = Math.Min(Math.Max((G / count), 0), 255);
                        image_bisHorizontale[i, j].Item3 = Math.Min(Math.Max((B / count), 0), 255);
                    }
                }
            }


            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    //On applique pas le min max ici pour avoir des jolies couleurs
                    this.image[i, j].Item1 = image_bisVerticale[i, j].Item1 + image_bisHorizontale[i, j].Item1;
                    this.image[i, j].Item2 = image_bisVerticale[i, j].Item2 + image_bisHorizontale[i, j].Item2;
                    this.image[i, j].Item3 = image_bisVerticale[i, j].Item3 + image_bisHorizontale[i, j].Item3;
                }
            }

        }
        /// <summary>
        ///                                             {-1,-1,-1}
        /// Filtre detect bord 3x3                      {-1, 8,-1}
        /// Meme commentaire que le filtre detectbord_1 {-1,-1,-1}
        /// </summary>
        public void Detect_Bord_Type2()
        {
            (int, int, int)[,] image_bis = ((int, int, int)[,])this.image.Clone();
            //Verticale
            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    int count = 0;
                    int R = image[i, j].Item1 * 8;
                    int G = image[i, j].Item2 * 8;
                    int B = image[i, j].Item3 * 8;

                    if (i > 0)
                    {
                        R -= image[i - 1, j].Item1; //haut
                        G -= image[i - 1, j].Item2;
                        B -= image[i - 1, j].Item3;
                        count += 1;
                        if (j > 0)
                        {
                            R -= image[i - 1, j - 1].Item1; //haut gauche
                            G -= image[i - 1, j - 1].Item2;
                            B -= image[i - 1, j - 1].Item3;
                            count += 1;
                        }
                        if (j < this.image.GetLength(1) - 1)
                        {
                            R -= image[i - 1, j + 1].Item1; //haut droite
                            G -= image[i - 1, j + 1].Item2;
                            B -= image[i - 1, j + 1].Item3;
                            count += 1;
                        }
                    }
                    if (j > 0)
                    {
                        R -= image[i, j - 1].Item1; //gauche
                        G -= image[i, j - 1].Item2;
                        B -= image[i, j - 1].Item3;
                        count += 1;
                    }
                    if (j < this.image.GetLength(1) - 1)
                    {
                        R -= image[i, j + 1].Item1; //droite
                        G -= image[i, j + 1].Item2;
                        B -= image[i, j + 1].Item3;
                        count += 1;
                    }
                    if (i < this.image.GetLength(0) - 1)
                    {
                        R -= image[i + 1, j].Item1; //bas
                        G -= image[i + 1, j].Item2;
                        B -= image[i + 1, j].Item3;
                        count += 1;
                        if (j > 0)
                        {
                            R -= image[i + 1, j - 1].Item1; //bas gauche
                            G -= image[i + 1, j - 1].Item2;
                            B -= image[i + 1, j - 1].Item3;
                            count += 1;
                        }
                        if (j < this.image.GetLength(1) - 1)
                        {
                            R -= image[i + 1, j + 1].Item1; //bas droite
                            G -= image[i + 1, j + 1].Item2;
                            B -= image[i + 1, j + 1].Item3;
                            count += 1;
                        }
                    }
                    if (count > 0)
                    {
                        image_bis[i, j].Item1 = Math.Min(Math.Max((R / count), 0), 255);
                        image_bis[i, j].Item2 = Math.Min(Math.Max((G / count), 0), 255);
                        image_bis[i, j].Item3 = Math.Min(Math.Max((B / count), 0), 255);
                    }
                }
            }

            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    this.image[i, j].Item1 = image_bis[i, j].Item1;
                    this.image[i, j].Item2 = image_bis[i, j].Item2;
                    this.image[i, j].Item3 = image_bis[i, j].Item3;
                }
            }

        }
        /// <summary>
        ///                                             { 0, 0,-1, 0, 0}   { 0, 0, 0, 0, 0}     
        ///                                             { 0, 0,-1, 0, 0}   { 0, 0, 0, 0, 0}                 
        /// Filtre detect bord 5x5                      { 0, 0, 4, 0, 0} + {-1,-1, 4,-1,-1} 
        ///                                             { 0, 0,-1, 0, 0}   { 0, 0, 0, 0, 0}                       
        /// Meme commentaire que le filtre detectbord_1 { 0, 0,-1, 0, 0}   { 0, 0, 0, 0, 0}                      
        /// </summary>
        public void Detect_Bord_Type3()
        {
            (int, int, int)[,] image_bisVerticale = ((int, int, int)[,])this.image.Clone();
            (int, int, int)[,] image_bisHorizontale = ((int, int, int)[,])this.image.Clone();
            //Verticale
            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    int count = 0;
                    int R = image[i, j].Item1 * 4;
                    int G = image[i, j].Item2 * 4;
                    int B = image[i, j].Item3 * 4;

                    if (i > 0)
                    {
                        R -= image[i - 1, j].Item1; //haut n + 1
                        G -= image[i - 1, j].Item2;
                        B -= image[i - 1, j].Item3;
                        count += 1;
                    }
                    if (i > 1)
                    {
                        R -= image[i - 2, j].Item1; //haut n + 2
                        G -= image[i - 2, j].Item2;
                        B -= image[i - 2, j].Item3;
                        count += 1;
                    }
                    if (i < this.image.GetLength(0) - 1)
                    {
                        R -= image[i + 1, j].Item1; //bas + 1
                        G -= image[i + 1, j].Item2;
                        B -= image[i + 1, j].Item3;
                        count += 1;
                    }
                    if (i < this.image.GetLength(0) - 2)
                    {
                        R -= image[i + 2, j].Item1; //bas + 2
                        G -= image[i + 2, j].Item2;
                        B -= image[i + 2, j].Item3;
                        count += 1;
                    }
                    if (count > 0)
                    {
                        image_bisVerticale[i, j].Item1 = Math.Min(Math.Max((R / count), 0), 255);
                        image_bisVerticale[i, j].Item2 = Math.Min(Math.Max((G / count), 0), 255);
                        image_bisVerticale[i, j].Item3 = Math.Min(Math.Max((B / count), 0), 255);
                    }
                }
            }

            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    int count = 0;
                    int R = image[i, j].Item1 * 4;
                    int G = image[i, j].Item2 * 4;
                    int B = image[i, j].Item3 * 4;



                    if (j > 0)
                    {
                        R -= image[i, j - 1].Item1; //gauche n + 1
                        G -= image[i, j - 1].Item2;
                        B -= image[i, j - 1].Item3;
                        count += 1;
                    }
                    if (j > 1)
                    {
                        R -= image[i, j - 2].Item1; //gauche n + 2
                        G -= image[i, j - 2].Item2;
                        B -= image[i, j - 2].Item3;
                        count += 1;
                    }
                    if (j < this.image.GetLength(1) - 1)
                    {
                        R -= image[i, j + 1].Item1; //droite n + 1
                        G -= image[i, j + 1].Item2;
                        B -= image[i, j + 1].Item3;
                        count += 1;
                    }
                    if (j < this.image.GetLength(1) - 2)
                    {
                        R -= image[i, j + 2].Item1; //droite n + 2
                        G -= image[i, j + 2].Item2;
                        B -= image[i, j + 2].Item3;
                        count += 1;
                    }

                    if (count > 0)
                    {
                        image_bisHorizontale[i, j].Item1 = Math.Min(Math.Max((R / count), 0), 255);
                        image_bisHorizontale[i, j].Item2 = Math.Min(Math.Max((G / count), 0), 255);
                        image_bisHorizontale[i, j].Item3 = Math.Min(Math.Max((B / count), 0), 255);
                    }
                }
            }


            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    this.image[i, j].Item1 = image_bisVerticale[i, j].Item1 + image_bisHorizontale[i, j].Item1;
                    this.image[i, j].Item2 = image_bisVerticale[i, j].Item2 + image_bisHorizontale[i, j].Item2;
                    this.image[i, j].Item3 = image_bisVerticale[i, j].Item3 + image_bisHorizontale[i, j].Item3;
                }
            }

        }
        /// <summary>
        ///                                             { 0, 0, 0}
        /// Filtre renforcement bord 3x3                {-1, 1, 0}
        /// Meme commentaire que le filtre detectbord_1 { 0, 0, 0}
        /// </summary>
        public void RenforcementBord()
        {
            (int, int, int)[,] image_bis = ((int, int, int)[,])this.image.Clone();

            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    int count = 0;
                    int R = image[i, j].Item1;
                    int G = image[i, j].Item2;
                    int B = image[i, j].Item3;


                    if (j > 0)
                    {
                        R -= image[i, j - 1].Item1; //gauche
                        G -= image[i, j - 1].Item2;
                        B -= image[i, j - 1].Item3;
                        count += 1;
                    }
                    if (count > 0)
                    {

                        image_bis[i, j].Item1 = Math.Min(Math.Max((R / count), 0), 255);
                        image_bis[i, j].Item2 = Math.Min(Math.Max((G / count), 0), 255);
                        image_bis[i, j].Item3 = Math.Min(Math.Max((B / count), 0), 255);
                    }
                }
            }

            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    this.image[i, j].Item1 = image_bis[i, j].Item1;
                    this.image[i, j].Item2 = image_bis[i, j].Item2;
                    this.image[i, j].Item3 = image_bis[i, j].Item3;
                }
            }

        }
        /// <summary>
        ///                                             {-2,-1, 0}
        /// Filtre renforcement bord 3x3                {-1, 1, 1}
        /// Meme commentaire que le filtre detectbord_1 { 0, 1, 2}
        /// </summary>
        public void Repoussage()
        {
            (int, int, int)[,] image_bis = ((int, int, int)[,])this.image.Clone();
            //Verticale
            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    int count = 0;
                    int R = image[i, j].Item1;
                    int G = image[i, j].Item2;
                    int B = image[i, j].Item3;

                    if (i > 0)
                    {
                        R -= image[i - 1, j].Item1; //haut
                        G -= image[i - 1, j].Item2;
                        B -= image[i - 1, j].Item3;
                        count += 1;
                        if (j > 0)
                        {
                            R -= image[i - 1, j - 1].Item1 * 2; //haut gauche
                            G -= image[i - 1, j - 1].Item2 * 2;
                            B -= image[i - 1, j - 1].Item3 * 2;
                            count += 1;
                        }
                    }
                    if (j > 0)
                    {
                        R -= image[i, j - 1].Item1; //gauche
                        G -= image[i, j - 1].Item2;
                        B -= image[i, j - 1].Item3;
                        count += 1;
                    }
                    if (j < this.image.GetLength(1) - 1)
                    {
                        R += image[i, j + 1].Item1; //droite
                        G += image[i, j + 1].Item2;
                        B += image[i, j + 1].Item3;
                        count += 1;
                    }
                    if (i < this.image.GetLength(0) - 1)
                    {
                        R += image[i + 1, j].Item1; //bas
                        G += image[i + 1, j].Item2;
                        B += image[i + 1, j].Item3;
                        count += 1;
                        if (j < this.image.GetLength(1) - 1)
                        {
                            R += image[i + 1, j + 1].Item1 * 2; //bas droite
                            G += image[i + 1, j + 1].Item2 * 2;
                            B += image[i + 1, j + 1].Item3 * 2;
                            count += 1;
                        }
                    }
                    if (count > 0)
                    {

                        image_bis[i, j].Item1 = Math.Min(Math.Max((R / count), 0), 255);
                        image_bis[i, j].Item2 = Math.Min(Math.Max((G / count), 0), 255);
                        image_bis[i, j].Item3 = Math.Min(Math.Max((B / count), 0), 255);
                    }
                }
            }

            for (int i = 0; i < this.image.GetLength(0); i++)
            {
                for (int j = 0; j < this.image.GetLength(1); j++)
                {
                    this.image[i, j].Item1 = image_bis[i, j].Item1;
                    this.image[i, j].Item2 = image_bis[i, j].Item2;
                    this.image[i, j].Item3 = image_bis[i, j].Item3;
                }
            }

        }
        /// <summary>
        /// Fonction servant a cacher une image dans une autre
        /// Les coordonnées serve à spécifier où cacher l'image sachant que sa correspond a l'angle en bas à gauche de la seconde image
        /// </summary>
        public void Steganographie(string pathFirstImage, string pathSecondImage, string file, int coordooneeX = 0, int coordooneeY = 0)
        {
            MyImage firstImage = new MyImage(pathFirstImage);
            MyImage secondImage = new MyImage(pathSecondImage);
            //On accorde les dimensions de l'image 1 pour accomder l'image 2
            if (firstImage.hauteur < secondImage.hauteur)
            {
                double coeff = secondImage.hauteur / firstImage.hauteur;
                firstImage.Agrandissement(coeff);
            }
            if (firstImage.largeur < secondImage.largeur)
            {
                double coeff = secondImage.largeur / firstImage.largeur;
                firstImage.Agrandissement(coeff);
            }

            //si coordonne nulle
            if (coordooneeX == 0 && coordooneeY == 0)
            {
                //matrice de l'image double
                (int, int, int)[,] imageDouble = new (int, int, int)[firstImage.hauteur, firstImage.largeur];

                for (int i = 0; i < firstImage.hauteur; i++)
                {
                    for (int j = 0; j < firstImage.largeur; j++)
                    {
                        //si dans les dimesensions de l'image 2 ajouter a l'image double le combiner des deux images
                        if (i < secondImage.hauteur && j < secondImage.largeur)
                        {
                            string redImage1 = Convert.ToString(firstImage.image[i, j].Item1, 2).PadLeft(8, '0').Substring(0, 4);
                            string redImage2 = Convert.ToString(secondImage.image[i, j].Item1, 2).PadLeft(8, '0').Substring(0, 4);
                            string greenImage1 = Convert.ToString(firstImage.image[i, j].Item2, 2).PadLeft(8, '0').Substring(0, 4);
                            string greenImage2 = Convert.ToString(secondImage.image[i, j].Item2, 2).PadLeft(8, '0').Substring(0, 4);
                            string blueImage1 = Convert.ToString(firstImage.image[i, j].Item3, 2).PadLeft(8, '0').Substring(0, 4);
                            string blueImage2 = Convert.ToString(secondImage.image[i, j].Item3, 2).PadLeft(8, '0').Substring(0, 4);

                            string red = redImage1 + redImage2;
                            string green = greenImage1 + greenImage2;
                            string blue = blueImage1 + blueImage2;

                            imageDouble[i, j] = (Convert.ToInt32(red, 2), Convert.ToInt32(green, 2), Convert.ToInt32(blue, 2));
                        }
                        //sinon prendre la donne de l'image original
                        else
                        {
                            string redImage1 = Convert.ToString(firstImage.image[i, j].Item1, 2).PadLeft(8, '0').Substring(0, 4);
                            string greenImage1 = Convert.ToString(firstImage.image[i, j].Item2, 2).PadLeft(8, '0').Substring(0, 4);
                            string blueImage1 = Convert.ToString(firstImage.image[i, j].Item3, 2).PadLeft(8, '0').Substring(0, 4);

                            string red = redImage1.PadRight(8, '0');
                            string green = greenImage1.PadRight(8, '0');
                            string blue = blueImage1.PadRight(8, '0');

                            imageDouble[i, j] = (Convert.ToInt32(red, 2), Convert.ToInt32(green, 2), Convert.ToInt32(blue, 2));
                        }

                    }
                }
                //on change l'image de la classe avec imageDouble
                this.hauteur = firstImage.hauteur;
                this.largeur = firstImage.largeur;
                this.taille_fill = Convert.ToInt32(this.taille_offset + this.hauteur * 3 * this.largeur);
                this.image = new (int, int, int)[firstImage.hauteur, firstImage.largeur];

                for (int i = 0; i < firstImage.hauteur; i++)
                {
                    for (int j = 0; j < firstImage.largeur; j++)
                    {
                        this.image[i, j] = imageDouble[i, j];
                    }
                }

                From_Image_To_File(file);

            }
            //Meme commentaire que la partie 1 sauf que les coordonées restraine l'affectation de l'image 2
            else
            {
                (int, int, int)[,] imageDouble = new (int, int, int)[firstImage.hauteur, firstImage.largeur];

                for (int i = 0; i < firstImage.hauteur; i++)
                {
                    for (int j = 0; j < firstImage.largeur; j++)
                    {
                        if (i > coordooneeY && j > coordooneeX && i < secondImage.hauteur + coordooneeY && j < secondImage.largeur + coordooneeX)
                        {
                            string redImage1 = Convert.ToString(firstImage.image[i, j].Item1, 2).PadLeft(8, '0').Substring(0, 4);
                            string redImage2 = Convert.ToString(secondImage.image[i - coordooneeY, j - coordooneeX].Item1, 2).PadLeft(8, '0').Substring(0, 4);
                            string greenImage1 = Convert.ToString(firstImage.image[i, j].Item2, 2).PadLeft(8, '0').Substring(0, 4);
                            string greenImage2 = Convert.ToString(secondImage.image[i - coordooneeY, j - coordooneeX].Item2, 2).PadLeft(8, '0').Substring(0, 4);
                            string blueImage1 = Convert.ToString(firstImage.image[i, j].Item3, 2).PadLeft(8, '0').Substring(0, 4);
                            string blueImage2 = Convert.ToString(secondImage.image[i - coordooneeY, j - coordooneeX].Item3, 2).PadLeft(8, '0').Substring(0, 4);

                            string red = redImage1 + redImage2;
                            string green = greenImage1 + greenImage2;
                            string blue = blueImage1 + blueImage2;

                            imageDouble[i, j] = (Convert.ToInt32(red, 2), Convert.ToInt32(green, 2), Convert.ToInt32(blue, 2));
                        }
                        else
                        {
                            string redImage1 = Convert.ToString(firstImage.image[i, j].Item1, 2).PadLeft(8, '0').Substring(0, 4);
                            string greenImage1 = Convert.ToString(firstImage.image[i, j].Item2, 2).PadLeft(8, '0').Substring(0, 4);
                            string blueImage1 = Convert.ToString(firstImage.image[i, j].Item3, 2).PadLeft(8, '0').Substring(0, 4);

                            string red = redImage1.PadRight(8, '0');
                            string green = greenImage1.PadRight(8, '0');
                            string blue = blueImage1.PadRight(8, '0');

                            imageDouble[i, j] = (Convert.ToInt32(red, 2), Convert.ToInt32(green, 2), Convert.ToInt32(blue, 2));
                        }
                    }
                }

                this.hauteur = firstImage.hauteur;
                this.largeur = firstImage.largeur;
                this.taille_fill = Convert.ToInt32(this.taille_offset + this.hauteur * 3 * this.largeur);
                this.image = new (int, int, int)[firstImage.hauteur, firstImage.largeur];

                for (int i = 0; i < firstImage.hauteur; i++)
                {
                    for (int j = 0; j < firstImage.largeur; j++)
                    {
                        this.image[i, j] = imageDouble[i, j];
                    }
                }

                From_Image_To_File(file);

            }
        } 
        /// <summary>
        /// Fonction servant a split l'image contenue dans MyImage en deux separant ainsi l'image cache de l'image double
        /// </summary>
        public void DecryptageSteganographie()
        {
            (int, int, int)[,] image1 = new (int, int, int)[hauteur, largeur];
            (int, int, int)[,] image2 = new (int, int, int)[hauteur, largeur];

            //Division de l'image dans MyImage en deux partie suivant le principe de la steganographie
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    string red = Convert.ToString(image[i, j].Item1, 2).PadLeft(8, '0');
                    string green = Convert.ToString(image[i, j].Item2, 2).PadLeft(8, '0');
                    string blue = Convert.ToString(image[i, j].Item3, 2).PadLeft(8, '0');

                    string redImage1 = red.Substring(0, 4).PadRight(8, '0');
                    string redImage2 = red.Substring(4, 4).PadRight(8, '0');
                    string greenImage1 = green.Substring(0, 4).PadRight(8, '0');
                    string greenImage2 = green.Substring(4, 4).PadRight(8, '0');
                    string blueImage1 = blue.Substring(0, 4).PadRight(8, '0');
                    string blueImage2 = blue.Substring(4, 4).PadRight(8, '0');

                    image1[i, j] = (Convert.ToInt32(redImage1, 2), Convert.ToInt32(greenImage1, 2), Convert.ToInt32(blueImage1, 2));
                    image2[i, j] = (Convert.ToInt32(redImage2, 2), Convert.ToInt32(greenImage2, 2), Convert.ToInt32(blueImage2, 2));
                }
            }
            //on change l'image de la classe avec image1
            (int, int, int)[,] imageOriginal = new (int, int, int)[hauteur, largeur];
            Array.Copy(image, imageOriginal, image.Length);

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    image[i, j] = image1[i, j];
                }
            }
            From_Image_To_File(this.filname + "_Split_1");
            //on change l'image de la classe avec image2
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    image[i, j] = image2[i, j];
                }
            }
            From_Image_To_File(this.filname + "_Split_2");
            //on retourne à l'original
            Array.Copy(imageOriginal, image, imageOriginal.Length);
        }
        /// <summary>
        /// Fonction de flou uniforme
        /// Meme commentaire que le filtre detectbord_1
        /// </summary>
        public void FloutageV1(int coeff)
        {
            if (this.image != null && this.image.Length >= 4)
            {
                for (int k = 0; k < coeff; k++)
                {
                    (int, int, int)[,] image_bis = ((int, int, int)[,])this.image.Clone();
                    for (int i = 0; i < this.image.GetLength(0); i++)
                    {
                        for (int j = 0; j < this.image.GetLength(1); j++)
                        {
                            int count = 0;
                            int Rflou = 0;
                            int Gflou = 0;
                            int Bflou = 0;


                            if (i > 0)
                            {
                                Rflou += image_bis[i - 1, j].Item1; //haut
                                Gflou += image_bis[i - 1, j].Item2;
                                Bflou += image_bis[i - 1, j].Item3;
                                count += 1;
                                if (j > 0)
                                {
                                    Rflou += image_bis[i - 1, j - 1].Item1; //haut gauche
                                    Gflou += image_bis[i - 1, j - 1].Item2;
                                    Bflou += image_bis[i - 1, j - 1].Item3;
                                    count += 1;
                                }
                                if (j < this.image.GetLength(1) - 1)
                                {
                                    Rflou += image_bis[i - 1, j + 1].Item1; //haut droite
                                    Gflou += image_bis[i - 1, j + 1].Item2;
                                    Bflou += image_bis[i - 1, j + 1].Item3;
                                    count += 1;
                                }
                            }
                            if (j > 0)
                            {
                                Rflou += image_bis[i, j - 1].Item1; //gauche
                                Gflou += image_bis[i, j - 1].Item2;
                                Bflou += image_bis[i, j - 1].Item3;
                                count += 1;
                            }
                            if (j < this.image.GetLength(1) - 1)
                            {
                                Rflou += image_bis[i, j + 1].Item1; //droite
                                Gflou += image_bis[i, j + 1].Item2;
                                Bflou += image_bis[i, j + 1].Item3;
                                count += 1;
                            }
                            if (i < this.image.GetLength(0) - 1)
                            {
                                Rflou += image_bis[i + 1, j].Item1; //bas
                                Gflou += image_bis[i + 1, j].Item2;
                                Bflou += image_bis[i + 1, j].Item3;
                                count += 1;
                                if (j > 0)
                                {
                                    Rflou += image_bis[i + 1, j - 1].Item1; //bas gauche
                                    Gflou += image_bis[i + 1, j - 1].Item2;
                                    Bflou += image_bis[i + 1, j - 1].Item3;
                                    count += 1;
                                }
                                if (j < this.image.GetLength(1) - 1)
                                {
                                    Rflou += image_bis[i + 1, j + 1].Item1; //bas droite
                                    Gflou += image_bis[i + 1, j + 1].Item2;
                                    Bflou += image_bis[i + 1, j + 1].Item3;
                                    count += 1;
                                }
                            }
                            if (count > 0)
                            {
                                if (k % 2 == 0)
                                {
                                    Rflou += 5;
                                    Gflou += 5;
                                    Bflou += 5;
                                }

                                this.image[i, j].Item1 = (Rflou / count);
                                this.image[i, j].Item2 = (Gflou / count);
                                this.image[i, j].Item3 = (Bflou / count);
                            }
                        }
                    }
                }
            }
        } // Floutage Uniforme
        /// <summary>
        /// Fonction de flou gaussien avec n rayon, à faire à vos risque et péril, complexité O(n^5) et pas du tout optimiser, 
        /// fait une moyenne de la matrice de taille coeff * 2 + 1 pour chaque pixel, marche bien et est plus précis que de faire 
        /// Floutage Uniforme plusieur fois mais beaucoup trop long.
        /// </summary>
        public void FloutageV2(int coeff)
        {
            if (this.image != null && this.image.Length >= 4)
            {
                (int, int, int)[,] image_bis = ((int, int, int)[,])this.image.Clone();
                for (int i = 0; i < this.image.GetLength(0); i++)
                {
                    for (int j = 0; j < this.image.GetLength(1); j++)
                    {
                        int count = 0;
                        int Rflou = 0;
                        int Gflou = 0;
                        int Bflou = 0;

                        for (int n = 0; n <= coeff; n++)
                        {
                            for (int k = 0; k < n * 2 + 1; k++)
                            {
                                for (int l = 0; l < n * 2 + 1; l++)
                                {
                                    try
                                    {
                                        if (k == 0 && l < n * 2)
                                        {
                                            Rflou += image_bis[i - n, j - n + l].Item1;
                                            Gflou += image_bis[i - n, j - n + l].Item2;
                                            Bflou += image_bis[i - n, j - n + l].Item3;
                                            count += 1;
                                        }
                                        if (k == n * 2 && l > 0)
                                        {
                                            Rflou += image_bis[i + n, j - n + l].Item1;
                                            Gflou += image_bis[i + n, j - n + l].Item2;
                                            Bflou += image_bis[i + n, j - n + l].Item3;
                                            count += 1;
                                        }
                                        if (l == n * 2 && k < n * 2)
                                        {
                                            Rflou += image_bis[i - n + k, j + n].Item1;
                                            Gflou += image_bis[i - n + k, j + n].Item2;
                                            Bflou += image_bis[i - n + k, j + n].Item3;
                                            count += 1;
                                        }
                                        if (l == 0 && k > 0)
                                        {
                                            Rflou += image_bis[i - n + k, j - n].Item1;
                                            Gflou += image_bis[i - n + k, j - n].Item2;
                                            Bflou += image_bis[i - n + k, j - n].Item3;
                                            count += 1;
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                        }

                        if (count > 0)
                        {
                            this.image[i, j].Item1 = (Rflou / count);
                            this.image[i, j].Item2 = (Gflou / count);
                            this.image[i, j].Item3 = (Bflou / count);
                        }
                    }
                    if (i % 20 == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("\nGénération en cours : " + i + "/" + this.hauteur);
                    }
                }
            }
        }
        /// <summary>
        /// Filtre divers fesant penser a une hallucination collective
        /// Meme commentaire que le filtre detectbord_1
        /// </summary>
        public void Hallucination(int coeff)
        {
            if (this.image != null && this.image.Length >= 4)
            {
                for (int k = 0; k < coeff; k++)
                {
                    (int, int, int)[,] image_bis = ((int, int, int)[,])this.image.Clone();
                    for (int i = 0; i < this.image.GetLength(0); i++)
                    {
                        for (int j = 0; j < this.image.GetLength(1); j++)
                        {
                            int count = 0;
                            int Rflou = 0;
                            int Gflou = 0;
                            int Bflou = 0;


                            if (i > 0)
                            {
                                Rflou += image_bis[i - 1, j].Item1; //haut
                                Gflou += image_bis[i - 1, j].Item2;
                                Bflou += image_bis[i - 1, j].Item3;
                                count += 1;
                                if (j > 0)
                                {
                                    Rflou += image_bis[i - 1, j - 1].Item1; //haut gauche
                                    Gflou += image_bis[i - 1, j - 1].Item2;
                                    Bflou += image_bis[i - 1, j - 1].Item3;
                                    count += 1;
                                }
                                if (j < this.image.GetLength(1) - 1)
                                {
                                    Rflou += image_bis[i - 1, j + 1].Item1; //haut droite
                                    Gflou += image_bis[i - 1, j + 1].Item2;
                                    Bflou += image_bis[i - 1, j + 1].Item3;
                                    count += 1;
                                }
                            }
                            if (j > 0)
                            {
                                Rflou += image_bis[i, j - 1].Item1; //gauche
                                Gflou += image_bis[i, j - 1].Item2;
                                Bflou += image_bis[i, j - 1].Item3;
                                count += 1;
                            }
                            if (j < this.image.GetLength(1) - 1)
                            {
                                Rflou += image_bis[i, j + 1].Item1; //droite
                                Gflou += image_bis[i, j + 1].Item2;
                                Bflou += image_bis[i, j + 1].Item3;
                                count += 1;
                            }
                            if (i < this.image.GetLength(0) - 1)
                            {
                                Rflou += image_bis[i + 1, j].Item1; //bas
                                Gflou += image_bis[i + 1, j].Item2;
                                Bflou += image_bis[i + 1, j].Item3;
                                count += 1;
                                if (j > 0)
                                {
                                    Rflou += image_bis[i + 1, j - 1].Item1; //bas gauche
                                    Gflou += image_bis[i + 1, j - 1].Item2;
                                    Bflou += image_bis[i + 1, j - 1].Item3;
                                    count += 1;
                                }
                                if (j < this.image.GetLength(1) - 1)
                                {
                                    Rflou += image_bis[i + 1, j + 1].Item1; //bas droite
                                    Gflou += image_bis[i + 1, j + 1].Item2;
                                    Bflou += image_bis[i + 1, j + 1].Item3;
                                    count += 1;
                                }
                            }
                            if (count > 0)
                            {
                                if (k % 2 == 0)
                                {
                                    Rflou = Convert.ToInt16(Rflou * 1.05);
                                    Gflou = Convert.ToInt16(Gflou * 1.05);
                                    Bflou = Convert.ToInt16(Bflou * 1.05);
                                }

                                this.image[i, j].Item1 = (Rflou / count);
                                this.image[i, j].Item2 = (Gflou / count);
                                this.image[i, j].Item3 = (Bflou / count);
                            }
                        }
                    }
                }
            }
        } // Floutage Uniforme
        /// <summary>
        /// Fonction d'agrandissement marchant pour n'importe quel coef
        /// </summary>
        public void Agrandissement(double coeff)
        {
            if (this.image != null && this.image.Length >= 4)
            {
                //Matrice clone de MyImage.Image
                (int, int, int)[,] image_bis = ((int, int, int)[,])this.image.Clone();
                
                //on change les dimensions tout en assurant que c'est toujours du bmp
                this.hauteur = Convert.ToInt32(this.hauteur * coeff - (this.hauteur * coeff % 4));
                this.largeur = Convert.ToInt32(this.largeur * coeff - (this.largeur * coeff % 4));
                this.taille_fill = Convert.ToInt32(this.taille_offset + this.hauteur * 3 * this.largeur);
                this.image = new (int, int, int)[this.hauteur, this.largeur];


                //affectation des pixels dans la nouvelle image redimensionne grace au clone d'image
                for (int i = 0; i < this.hauteur; i++)
                {
                    for (int j = 0; j < this.largeur; j++)
                    {
                        int lig = Convert.ToInt32(i / coeff);
                        int col = Convert.ToInt32(j / coeff);

                        if (lig > image_bis.GetLength(0) - 1 && col > image_bis.GetLength(1) - 1)
                        {
                            image[i, j] = image_bis[image_bis.GetLength(0) - 1, image_bis.GetLength(1) - 1];
                        }
                        else if (lig > image_bis.GetLength(0) - 1)
                        {
                            image[i, j] = image_bis[image_bis.GetLength(0) - 1, col];
                        }
                        else if (col > image_bis.GetLength(1) - 1)
                        {
                            image[i, j] = image_bis[lig, image_bis.GetLength(1) - 1];
                        }
                        else
                        {
                            image[i, j] = image_bis[lig, col];
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Fonction de rotation de l'image marchant pour n'importe quel coef
        /// </summary>
        public void Rotation(double angle)
        {
            // Roation de l'image dans une plus grande

            if (angle > 360)
            {
                angle = angle % 360; // si l'angle dépasse 360, il fait un tour complet donc on prend le reste de angle / 360
            }
            if (angle != 0)
            {
                (int, int, int)[,] image_bis = ((int, int, int)[,])this.image.Clone();

                int CenterX1 = this.image.GetLength(0) / 2; // on calcul le milieu de l'image de base
                int CenterY1 = this.image.GetLength(1) / 2;

                angle = -angle * Math.PI / 180.0; // on met l'angle en radian

                this.largeur = Convert.ToInt32(this.image.GetLength(1) * Math.Abs(Math.Cos(angle)) + this.image.GetLength(0) * Math.Abs(Math.Sin(angle))); // on calcul la largeur de l'image de sortie
                this.hauteur = Convert.ToInt32(this.image.GetLength(1) * Math.Abs(Math.Sin(angle)) + this.image.GetLength(0) * Math.Abs(Math.Cos(angle))); // on calcul la hauteur de l'image de sortie

                int CenterX2 = this.hauteur / 2; // on calcul le milieu de l'image de sortie
                int CenterY2 = this.largeur / 2;

                this.image = new (int, int, int)[this.hauteur, this.largeur]; // on refait l'image de la classe pour qu'elle ai la taille de l'image de sortie

                for (int i = 0; i < this.hauteur; i++) // parcours l'image de sortie et y assigne les pixel de l'image de base avec l'angle en plus selon la formule mathématique en partant du centre des images
                {
                    for (int j = 0; j < this.largeur; j++)
                    {
                        double rayon = Math.Sqrt(Math.Pow(i - CenterX2, 2) + Math.Pow(j - CenterY2, 2));
                        double teta = Math.Atan2((j - CenterY2), (i - CenterX2)) - angle;

                        int posX = (int)(CenterX1 + rayon * Math.Cos(teta));
                        int posY = (int)(CenterY1 + rayon * Math.Sin(teta));

                        if (posX < image_bis.GetLength(0) && posX >= 0 && posY < image_bis.GetLength(1) && posY >= 0) // vérifie que les position soit bien dans l'image de base
                        {
                            this.image[i, j] = image_bis[posX, posY];
                        }
                        else
                        {
                            this.image[i, j] = (-1, -1, -1);
                        }

                    }
                }
                // redimension de l'image
                int largeurBase = image_bis.GetLength(1);
                double coeffBase = Convert.ToDouble(image_bis.GetLength(1)) / Convert.ToDouble(image_bis.GetLength(0));
                image_bis = ((int, int, int)[,])this.image.Clone();
                bool test = true;
                int debX = CenterX2;
                int debY = CenterY2;
                for (int i = 0; i < this.hauteur && test; i++)
                {
                    if (Convert.ToInt16(CenterY2 + i * coeffBase) < this.largeur && (image_bis[CenterX2 - i, CenterY2 + Convert.ToInt16(i * coeffBase)] == (-1, -1, -1) || image_bis[CenterX2 - i, CenterY2 - Convert.ToInt16(i * coeffBase)] == (-1, -1, -1)))
                    {
                        test = false;
                        debX = CenterX2 - i + 1;
                        debY = CenterY2 - Convert.ToInt16(i * coeffBase) + 1;
                    }
                }
                if (debX == CenterX2 && debY == CenterY2)
                {
                    debY = 1;
                    debX = CenterX2 - Convert.ToInt16(CenterY2 / coeffBase) + 1;
                }
                this.hauteur = (CenterX2 - debX) * 2;
                this.largeur = (CenterY2 - debY) * 2;
                this.hauteur -= this.hauteur % 4;
                this.largeur -= this.largeur % 4;
                this.image = new (int, int, int)[this.hauteur, this.largeur];

                for (int i = 0; i < this.hauteur; i++)
                {
                    for (int j = 0; j < this.largeur; j++)
                    {
                        this.image[i, j] = image_bis[debX + i, debY + j];
                    }
                }

                double coeffRedimension = Convert.ToDouble(largeurBase) / this.largeur;
                Agrandissement(coeffRedimension);
            }
        }
        public void Fractal_Julia(int resolution, int iterationMax, double RealAdd, double ImaginaryAdd, double couleur)
        {
            this.largeur = resolution - resolution % 4;
            this.hauteur = this.largeur * 9 / 16; // fois 9/16 pour être en format 16/9
            this.hauteur -= this.hauteur % 4;
            this.taille_fill = this.taille_offset + this.hauteur * 3 * this.largeur;
            this.image = new (int, int, int)[this.hauteur, this.largeur];

            int CentreX = this.largeur / 2;
            int CentreY = this.hauteur / 2;

            for (int i = 0; i < this.hauteur; i++)
            {
                for (int j = 0; j < this.largeur; j++)
                {
                    double real = Convert.ToDouble(j - CentreX) * 4 / this.largeur;
                    double ima = -Convert.ToDouble(i - CentreY) * 4 / this.hauteur / 16 * 9; // fois 9/16 pour le format des écrans actuel
                    int inte = 0;

                    while (real * real + ima * ima < 4 && inte < iterationMax)
                    {
                        double real2 = real;
                        double ima2 = ima;

                        real = real2 * real2 - ima2 * ima2 + RealAdd;
                        ima = 2.0 * real2 * ima2 + ImaginaryAdd;
                        inte++;
                    }
                    if (iterationMax == inte)
                    {
                        this.image[i, j] = (0, 0, 0);
                    }
                    else
                    {
                        int rgb = Convert.ToInt16(255 * inte * couleur / iterationMax + 5);
                        this.image[i, j] = (rgb, Convert.ToInt16(rgb * 0.5), Convert.ToInt16(rgb * 0.25));
                    }
                }
                if (i % 100 == 0)
                {
                    Console.Clear();
                    Console.WriteLine("\nGénération en cours : " + i + "/" + this.hauteur);
                }
            }
        }
        public void Fractal_Mandelbrot(int resolution, int iterationMax, double couleur)
        {
            this.largeur = resolution - resolution % 4;
            this.hauteur = this.largeur * 9 / 16;
            this.hauteur -= this.hauteur % 4;
            this.taille_fill = this.taille_offset + this.hauteur * 3 * this.largeur;
            this.image = new (int, int, int)[this.hauteur, this.largeur];

            int CentreX = Convert.ToInt32(this.largeur / 1.5);
            int CentreY = this.hauteur / 2;

            for (int i = 0; i < this.hauteur; i++)
            {
                for (int j = 0; j < this.largeur; j++)
                {
                    double real_c = Convert.ToDouble(j - CentreX) * 4 / this.largeur;
                    double ima_c = -Convert.ToDouble(i - CentreY) * 4 / this.hauteur / 16 * 9; // *9/16 pour le coeff des écran actuel
                    double real = 0;
                    double ima = 0;
                    int inte = 0;

                    while (real * real + ima * ima < 4 && inte < iterationMax)
                    {
                        double real2 = real;
                        double ima2 = ima;

                        real = (real2 * real2 - ima2 * ima2) + real_c;
                        ima = 2.0 * real2 * ima2 + ima_c;
                        inte++;
                    }
                    if (iterationMax == inte)
                    {
                        this.image[i, j] = (0, 0, 0);
                    }
                    else
                    {
                        int rgb = 5 + Convert.ToInt16(250 * inte * couleur / iterationMax + 5);
                        this.image[i, j] = (rgb, Convert.ToInt16(rgb * 0.5), Convert.ToInt16(rgb * 0.25));
                    }
                }
                if (i % 100 == 0)
                {
                    Console.Clear();
                    Console.WriteLine("\nGénération en cours : " + i + "/" + this.hauteur);
                }
            }
        }
        public void Fractal_Newton(int resolution, int iterationMax, int color, int Fbase, int nb = 1, int dnb = 1, string F = "z ^ 3 - 1", string D = "3 * z ^ 2")
        {
            this.largeur = resolution - resolution % 4;
            this.hauteur = this.largeur * 9 / 16; // fois 9/16 pour être en format 16/9
            this.hauteur -= this.hauteur % 4;
            this.taille_fill = this.taille_offset + this.hauteur * 3 * this.largeur;
            this.image = new (int, int, int)[this.hauteur, this.largeur];

            int CentreX = this.largeur / 2;
            int CentreY = this.hauteur / 2;
            int coeffColor = color;

            int zoom = 4;
            if (Fbase == 2)
            {
                zoom = 8;
            }
            // mettre un try pour les Df ou le degre du polinome est supérieur à F
            for (int i = 0; i < this.hauteur; i++)
            {
                for (int j = 0; j < this.largeur; j++)
                {
                    Complex z = new Complex(Convert.ToDouble(j - CentreX) * zoom / this.largeur, Convert.ToDouble(i - CentreY) * zoom / this.hauteur / 16 * 9);
                    Complex z1 = new Complex(0, 0);
                    int inte = 0;

                    for (int k = 0; k < iterationMax; k++)
                    {
                        inte++;
                        if (ABS(z - z1) <= 0.0001)
                        {
                            if (z.Imaginary + 0.001 >= -z.Real * 1.732 && z.Imaginary + 0.001 >= 0)
                            {
                                this.image[i, j] = (Convert.ToInt32(z.Imaginary * 255), 255 - inte * coeffColor, Convert.ToInt32(z.Real * 255));
                            }
                            else if (z.Imaginary - 0.001 <= z.Real * 1.732 && z.Imaginary - 0.001 <= 0)
                            {
                                this.image[i, j] = (Convert.ToInt32(z.Real * 255), Convert.ToInt32(z.Imaginary * 255), 255 - inte * coeffColor);
                            }
                            else
                            {
                                this.image[i, j] = ((255 - inte * coeffColor), Convert.ToInt32(z.Real * 255), Convert.ToInt32(z.Imaginary * 255));
                            }
                            z1 = new Complex(0, 0);
                            break;
                        }
                        z1 = z;
                        if (Fbase == 0)
                        {
                            z = z - Fct(z, F) / DFct(z, D);
                        }
                        else if (Fbase == 1)
                        {
                            z = z - Fct1(z, nb) / DFct1(z, nb, dnb);
                        }
                        else if (Fbase == 2)
                        {
                            z = z - Fct2(z) / DFct2(z);
                        }
                        if (Fbase == 3)
                        {
                            z = z - Fct3(z) / DFct3(z);
                        }
                    }
                }
                if (i % 25 == 0)
                {
                    Console.Clear();
                    Console.WriteLine("\nGénération en cours : " + i + "/" + this.hauteur);
                }
            }
        }
        /// <summary>
        /// Fonction Parse de la Fonction F
        /// </summary>
        private Complex Fct(Complex z, string F)
        {
            return Parser(F, z);
        }
        /// <summary>
        /// Fonction Parse de la fonction DF (derive de F)
        /// </summary>
        private Complex DFct(Complex z, string Df)
        {
            return Parser(Df, z);
        }
        /// <summary>
        /// Renvoie l'absolue du complex
        /// </summary>
        private double ABS(Complex z)
        {
            return Math.Sqrt(z.Real * z.Real) + Math.Sqrt(z.Imaginary * z.Imaginary);
        }
        /// <summary>
        /// Adapter une fonction string avec des nombres complexs vers le résultat de cette fonction avec z comme parametre complexe
        /// </summary>
        private static Complex Parser(string f, Complex z)
        {
            //Creation de notre complex
            Complex res = new Complex(0, 0);
            //Separe tout les composants de l'equation
            string[] tab = f.Split(' ');
            bool fin = false;
            int nb = 0;
            //on effectue les puissances en priorite
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i] == "^")
                {
                    Complex pow = Complex.Pow(z, Convert.ToDouble(tab[i + 1]));
                    tab[i - 1] = "";
                    tab[i] = "";
                    tab[i + 1] = Convert.ToString(pow.Real + " " + pow.Imaginary + "i");
                    nb += 2;
                }
            }
            string[] tab1 = new string[tab.Length - nb];
            nb = 0;
            //on enleve les espaces cree par la boucle precedente
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i] != "")
                {
                    tab1[i - nb] = tab[i];
                }
                else
                {
                    nb++;
                }
            }
            //on effectue dans l'ordre les * puis / 
            nb = 0;
            for (int i = 0; i < tab1.Length; i++)
            {
                if (tab1[i] == "*")
                {
                    string x = tab1[i - 1];
                    string y = tab1[i + 1];
                    Complex xc = new Complex(1, 0);
                    Complex yc = new Complex(1, 0);
                    if (x[x.Length - 1] == 'i')
                    {
                        string[] xt = x.Split(' ');
                        xt[1] = xt[1].Substring(0, xt[1].Length - 1);
                        xc = new Complex(Convert.ToDouble(xt[0]), Convert.ToDouble(xt[1]));
                    }
                    else if (x == "z")
                    {
                        xc = z;
                    }
                    else
                    {
                        xc = new Complex(Convert.ToDouble(x), 0);
                    }
                    if (y[y.Length - 1] == 'i')
                    {
                        string[] yt = y.Split(' ');
                        yt[1] = yt[1].Substring(0, yt[1].Length - 1);
                        yc = new Complex(Convert.ToDouble(yt[0]), Convert.ToDouble(yt[1]));
                    }
                    else if (y == "z")
                    {
                        yc = z;
                    }
                    else
                    {
                        yc = new Complex(Convert.ToDouble(y), 0);
                    }
                    Complex fois = xc * yc;
                    nb += 2;
                    tab1[i - 1] = "";
                    tab1[i] = "";
                    tab1[i + 1] = Convert.ToString(fois.Real + " " + fois.Imaginary + "i");
                }
                else if (tab1[i] == "/")
                {
                    string x = tab1[i - 1];
                    string y = tab1[i + 1];
                    Complex xc = new Complex(1, 0);
                    Complex yc = new Complex(1, 0);
                    if (x[x.Length - 1] == 'i')
                    {
                        string[] xt = x.Split(' ');
                        xt[1] = xt[1].Substring(0, xt[1].Length - 1);
                        xc = new Complex(Convert.ToDouble(xt[0]), Convert.ToDouble(xt[1]));
                    }
                    else if (x == "z")
                    {
                        xc = z;
                    }
                    else
                    {
                        xc = new Complex(Convert.ToDouble(x), 0);
                    }
                    if (y[y.Length - 1] == 'i')
                    {
                        string[] yt = y.Split(' ');
                        yt[1] = yt[1].Substring(0, yt[1].Length - 1);
                        yc = new Complex(Convert.ToDouble(yt[0]), Convert.ToDouble(yt[1]));
                    }
                    else if (y == "z")
                    {
                        yc = z;
                    }
                    else
                    {
                        yc = new Complex(Convert.ToDouble(y), 0);
                    }
                    Complex div = xc / yc;
                    nb += 2;
                    tab1[i - 1] = "";
                    tab1[i] = "";
                    tab1[i + 1] = Convert.ToString(div.Real + " " + div.Imaginary + "i");
                }
            }
            string[] tab2 = new string[tab1.Length - nb];
            nb = 0;
            //on enleve les espaces cree par la boucle precedente
            for (int i = 0; i < tab1.Length; i++)
            {
                if (tab1[i] != "")
                {
                    tab2[i - nb] = tab1[i];
                }
                else
                {
                    nb++;
                }
            }
            nb = 0;
            //on effectue dans l'ordre le + puis -
            for (int i = 0; i < tab2.Length; i++)
            {
                if (tab2[i] == "+")
                {
                    string x = tab2[i - 1];
                    string y = tab2[i + 1];
                    Complex xc = new Complex(1, 0);
                    Complex yc = new Complex(1, 0);
                    if (x[x.Length - 1] == 'i')
                    {
                        string[] xt = x.Split(' ');
                        xt[1] = xt[1].Substring(0, xt[1].Length - 1);
                        xc = new Complex(Convert.ToDouble(xt[0]), Convert.ToDouble(xt[1]));
                    }
                    else if (x == "z")
                    {
                        xc = z;
                    }
                    else
                    {
                        xc = new Complex(Convert.ToDouble(x), 0);
                    }
                    if (y[y.Length - 1] == 'i')
                    {
                        string[] yt = y.Split(' ');
                        yt[1] = yt[1].Substring(0, yt[1].Length - 1);
                        yc = new Complex(Convert.ToDouble(yt[0]), Convert.ToDouble(yt[1]));
                    }
                    else if (y == "z")
                    {
                        yc = z;
                    }
                    else
                    {
                        yc = new Complex(Convert.ToDouble(y), 0);
                    }
                    Complex plus = xc + yc;
                    nb += 2;
                    tab2[i - 1] = "";
                    tab2[i] = "";
                    tab2[i + 1] = Convert.ToString(plus.Real + " " + plus.Imaginary + "i");
                }
                else if (tab2[i] == "-")
                {
                    string x = tab2[i - 1];
                    string y = tab2[i + 1];
                    Complex xc = new Complex(1, 0);
                    Complex yc = new Complex(1, 0);
                    if (x[x.Length - 1] == 'i')
                    {
                        string[] xt = x.Split(' ');
                        xt[1] = xt[1].Substring(0, xt[1].Length - 1);
                        xc = new Complex(Convert.ToDouble(xt[0]), Convert.ToDouble(xt[1]));
                    }
                    else if (x == "z")
                    {
                        xc = z;
                    }
                    else
                    {
                        xc = new Complex(Convert.ToDouble(x), 0);
                    }
                    if (y[y.Length - 1] == 'i')
                    {
                        string[] yt = y.Split(' ');
                        yt[1] = yt[1].Substring(0, yt[1].Length - 1);
                        yc = new Complex(Convert.ToDouble(yt[0]), Convert.ToDouble(yt[1]));
                    }
                    else if (y == "z")
                    {
                        yc = z;
                    }
                    else
                    {
                        yc = new Complex(Convert.ToDouble(y), 0);
                    }
                    Complex moin = xc - yc;
                    nb += 2;
                    tab2[i - 1] = "";
                    tab2[i] = "";
                    tab2[i + 1] = Convert.ToString(moin.Real + " " + moin.Imaginary + "i");
                }
            }
            string[] tab3 = new string[tab2.Length - nb];
            nb = 0;
            //on enleve les espaces cree par la boucle precedente
            for (int i = 0; i < tab2.Length; i++)
            {
                if (tab2[i] != "")
                {
                    tab3[i - nb] = tab2[i];
                }
                else
                {
                    nb++;
                }
            }
            //On affecte les valeurs calcule a notre complex
            if (tab3[0][tab3[0].Length - 1] == 'i')
            {
                string[] rest = tab3[0].Split(' ');
                rest[1] = rest[1].Substring(0, rest[1].Length - 1);
                res = new Complex(Convert.ToDouble(rest[0]), Convert.ToDouble(rest[1]));
            }
            else
            {
                res = new Complex(Convert.ToDouble(tab3[0]), 0);
            }
            //on renvoie le complex
            return res;
        }
        /// <summary>
        /// Fonction predefini Newton : Pole
        /// </summary>
        private Complex Fct1(Complex z, int nb)
        {
            return Complex.Pow(z, nb) - 1;
        }
        /// <summary>
        /// derive de la Fct1
        /// </summary>
        private Complex DFct1(Complex z, int nb, int dnb)
        {
            return dnb * Complex.Pow(z, nb - 1);
        }
        /// <summary>
        /// Fonction predifini Newton : cellule
        /// </summary>
        private Complex Fct2(Complex z)
        {
            return (Complex.Pow(z, 3) - 1) / (Complex.Pow(z, 3) + 1);
        }
        /// <summary>
        /// derive de la Fct2
        /// </summary>
        private Complex DFct2(Complex z)
        {
            return (6 * Complex.Pow(z, 2)) / (Complex.Pow(z, 6) + Complex.Pow(z, 3) + 1);
        }
        /// <summary>
        /// Fonction predefini Newton : hasard
        /// </summary>
        private Complex Fct3(Complex z)
        {
            return Complex.Pow(z, 6) + Complex.Pow(z, 5) + Complex.Pow(z, 4) + Complex.Pow(z, 3) + Complex.Pow(z, 2) + z - 1;
        }
        /// <summary>
        /// derive de la Fct3
        /// </summary>
        private Complex DFct3(Complex z)
        {
            return 6 * Complex.Pow(z, 5) + 5 * Complex.Pow(z, 4) + 4 * Complex.Pow(z, 3) + 3 * Complex.Pow(z, 2) + 2 * z + 1;
        }
    }
}
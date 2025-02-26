namespace BitmapProject
{
    class MyImage
    {
        private string type;

        private int taille;

        private int taille_offset;

        private int largeur;

        private int hauteur;

        private int nb_byte_color;

        private (int, int, int)[,] image;

        public MyImage(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            char type1 = Convert.ToChar(data[0]);
            char type2 = Convert.ToChar(data[1]);
            this.type = type1 + " " + type2;
            this.taille = Little_EndianToDecimal(2, 6, data);
            this.taille_offset = Little_EndianToDecimal(10, 14, data);
            this.largeur = Little_EndianToDecimal(18, 22, data);
            this.hauteur = Little_EndianToDecimal(22, 26, data);
            this.nb_byte_color = Little_EndianToDecimal(28, 30, data);
            image = new (int, int, int)[this.hauteur, this.largeur];
            int g = 54;
            for (int i = 0; i < this.hauteur; i++)
            {
                for (int j = 0; j < this.largeur; j++)
                {
                    image[i, j] = (data[g], data[g + 1], data[g + 2]);
                    g = g + 3;
                }
            }
        }
        public MyImage(int width, int height)
        {
            this.type = "B M"; 
            this.taille = width * height * 3 + 54; 
            this.taille_offset = 0; 
            this.largeur = width;
            this.hauteur = height;
            this.nb_byte_color = 24; 
            this.image = new(int, int, int)[height,width] ;
        }
        public string Type
        {
            get { return type; }
        }
        public int Taille
        {
            get { return taille; }
        }
        public int Taille_Offset
        {
            get { return taille_offset; }
        }
        public int Hauteur
        {
            get { return hauteur; }
        }
        public int Largeur
        {
            get { return largeur; }
        }
        public int Nb_byte_color
        {
            get { return nb_byte_color; }
        }
        public (int, int, int)[,] Image
        {
            get { return image; }
            set { image = value; }
        }
        public int Little_EndianToDecimal(int index1, int index2, byte[] info)
        {
            string taille_binary = "";
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
        public byte[] DecimalToLittle_Endian(int number)
        {
            byte[] result = new byte[4];
            int count = 0;
            int[] numberArray = new int[32];
            for (int g = 0; g < 32; g++)
            {
                numberArray[g] = 0;
            }
            for (int j = 0; number > 0; j++)
            {
                numberArray[31 - j] = number % 2;
                number = number / 2;
                count++;
            }
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
        public void From_Image_To_File(string file)
        {
            byte[] saveImage = new byte[taille];
            saveImage[0] = 66;
            saveImage[1] = 77;
            byte[] saveTaille = DecimalToLittle_Endian(this.taille);
            for (int t = 2; t < 6; t++)
            {
                saveImage[t] = saveTaille[t - 2];
            }
            byte[] unchange = new byte[] { 0, 0, 0, 0, (byte)this.taille_offset, 0, 0, 0, 40, 0, 0, 0 };
            for (int t = 6; t < 18; t++)
            {
                saveImage[t] = unchange[t - 6];
            }
            byte[] largeur = DecimalToLittle_Endian(this.largeur);
            for (int t = 18; t < 22; t++)
            {
                saveImage[t] = largeur[t - 18];
            }
            byte[] hauteur = DecimalToLittle_Endian(this.hauteur);
            for (int t = 22; t < 26; t++)
            {
                saveImage[t] = hauteur[t - 22];
            }
            unchange = new byte[] { 1, 0, (byte)this.nb_byte_color, 0, 0, 0, 0, 0 };
            for (int t = 26; t < 34; t++)
            {
                saveImage[t] = unchange[t - 26];
            }
            byte[] tailleImage = DecimalToLittle_Endian(this.taille - this.taille_offset);
            for (int t = 34; t < 38; t++)
            {
                saveImage[t] = tailleImage[t - 34];
            }
            for (int t = 38; t < 54; t++)
            {
                saveImage[t] = 0;
            }
            int count = 54;
            for (int i = 0; i < this.hauteur; i++)
            {
                for (int j = 0; j < this.largeur; j++)
                {
                    saveImage[count] = (byte)this.image[i, j].Item1;
                    saveImage[count + 1] = (byte)this.image[i, j].Item2;
                    saveImage[count + 2] = (byte)this.image[i, j].Item3;
                    count = count + 3;
                }
            }

            File.WriteAllBytes("bin/Debug/net7.0/Image/" + file + ".bmp", saveImage);

        }
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
        public void Floutage(int coeff)
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
                }
            }
        } // A faire à vos risque et péril, complexité O(n^5) pas du tout optimiser, fait une moyenne de la matrice de taille coeff * 2 + 1 pour chaque pixel, marche bien et est plus précis que de faire Floutage Uniforme plusieur fois mais beaucoup trop long
        public void Agrandissement(double coeff)
        {
            if (this.image != null && this.image.Length >= 4)
            {
                (int, int, int)[,] image_bis = ((int, int, int)[,])this.image.Clone();

                this.hauteur = Convert.ToInt32(this.hauteur * coeff - (this.hauteur * coeff % 4));
                this.largeur = Convert.ToInt32(this.largeur * coeff - (this.largeur * coeff % 4));
                Console.WriteLine(this.largeur + " " + this.hauteur);
                this.taille = Convert.ToInt32(this.taille_offset + this.hauteur * 3 * this.largeur);
                this.image = new (int, int, int)[this.hauteur, this.largeur];

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

        //Operateur de sobel 
        public void Detect_Bord_Type1()
        {
            //{-1,0,1}  {-1,-2,-1}
            //{-2,0,2} +{ 0, 0, 0}
            //{-1,0,1}  { 1, 2, 1}
            (int, int, int)[,] image_bisVerticale = ((int, int, int)[,])this.image.Clone();
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
                    this.image[i, j].Item1 = image_bisVerticale[i, j].Item1 + image_bisHorizontale[i, j].Item1;
                    this.image[i, j].Item2 = image_bisVerticale[i, j].Item2 + image_bisHorizontale[i, j].Item2;
                    this.image[i, j].Item3 = image_bisVerticale[i, j].Item3 + image_bisHorizontale[i, j].Item3;
                }
            }

        }
        //Detect bord 3x3
        public void Detect_Bord_Type2()
        {
            //{-1,-1,-1}
            //{-1, 8,-1}
            //{-1,-1,-1}
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
        //Detect Bord 5x5
        public void Detect_Bord_Type3()
        {
            //{ 0, 0,-1, 0, 0}   { 0, 0, 0, 0, 0}
            //{ 0, 0,-1, 0, 0}   { 0, 0, 0, 0, 0}
            //{ 0, 0, 4, 0, 0} + {-1,-1, 4,-1,-1}
            //{ 0, 0,-1, 0, 0}   { 0, 0, 0, 0, 0}
            //{ 0, 0,-1, 0, 0}   { 0, 0, 0, 0, 0}
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

        public void RenforcementBord()
        {
            //{ 0, 0, 0}
            //{-1, 1, 0}
            //{ 0, 0, 0}
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

        public void Repoussage()
        {
            //{-2,-1, 0}
            //{-1, 1, 1}
            //{ 0, 1, 2}
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

        ///Les coordonnées serve à spécifier où cacher l'image sachant que sa correspond a l'angle en bas à gauche de la seconde image
        public void ImageDouble(string pathFirstImage, string pathSecondImage, int coordooneeX = 0, int coordooneeY = 0)
        {
            MyImage firstImage = new MyImage(pathFirstImage);
            MyImage secondImage = new MyImage(pathSecondImage);


            if (coordooneeX == 0 && coordooneeY == 0)
            {
                (int, int, int)[,] imageDouble = new (int, int, int)[firstImage.hauteur, firstImage.largeur];

                for (int i = 0; i < firstImage.hauteur; i++)
                {
                    for (int j = 0; j < firstImage.largeur; j++)
                    {
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

                image = new (int, int, int)[firstImage.hauteur, firstImage.largeur];

                for (int i = 0; i < firstImage.hauteur; i++)
                {
                    for (int j = 0; j < firstImage.largeur; j++)
                    {
                        image[i, j] = imageDouble[i, j];
                    }
                }

                From_Image_To_File("ImageDouble");

            }
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

                image = new (int, int, int)[firstImage.hauteur, firstImage.largeur];

                for (int i = 0; i < firstImage.hauteur; i++)
                {
                    for (int j = 0; j < firstImage.largeur; j++)
                    {
                        image[i, j] = imageDouble[i, j];
                    }
                }

                From_Image_To_File("ImageDouble");

            }



        }

        public void SplitImage()
        {
            (int, int, int)[,] image1 = new (int, int, int)[hauteur, largeur];
            (int, int, int)[,] image2 = new (int, int, int)[hauteur, largeur];

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

            (int, int, int)[,] imageOriginal = new (int, int, int)[hauteur, largeur];
            Array.Copy(image, imageOriginal, image.Length);

            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    image[i, j] = image1[i, j];
                }
            }
            From_Image_To_File("Split_1");
            for (int i = 0; i < image.GetLength(0); i++)
            {
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    image[i, j] = image2[i, j];
                }
            }
            From_Image_To_File("Split_2");

            Array.Copy(imageOriginal, image, imageOriginal.Length);
        }


    }
}
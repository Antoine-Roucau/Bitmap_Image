namespace BitmapProject
{
    /// <summary>
    /// Class noeud Huffman 
    /// </summary>
    class HuffmanNode
    {
        /// <summary>
        /// Declaration des Variables
        /// </summary>
        public int Value { get; set; }
        public float Frequency { get; set; }
        public HuffmanNode Left { get; set; }
        public HuffmanNode Right { get; set; }
        /// <summary>
        /// Constructeur
        /// </summary>
        public HuffmanNode(int value, float frequency)
        {
            Value = value;
            Frequency = frequency;
        }
        /// <summary>
        /// Renvoie si c'est a la fin de mon arbre
        /// </summary>
        public bool isLeaf()
        {
            return Left == null & Right == null;
        }

        /// <summary>
        /// Pour afficher notre class
        /// </summary>
        public override string ToString()
        {
            return Value + " " + Frequency;
        }

    }

    /// <summary>
    /// Class Huffman comprennant toute les logics
    /// </summary>
    class Huffman
    {
        /// <summary>
        /// Fonction qui calcule tout les frequences de qui renvoie float[] pour chaque couleur 
        /// </summary>
        private List<float[]> Frequence(MyImage image)
        {
            List<float[]> counts = new List<float[]>();
            float[] countR = new float[256];
            float[] countV = new float[256];
            float[] countB = new float[256];

            //Count pour chaque couleur
            for (int i = 0; i < image.Hauteur; i++)
            {
                for (int j = 0; j < image.Largeur; j++)
                {

                    int R = (int)image.Image[i, j].Item1;
                    int V = (int)image.Image[i, j].Item2;
                    int B = (int)image.Image[i, j].Item3;

                    countR[R]++;
                    countV[V]++;
                    countB[B]++;
                }
            }

            //Frequence pour chaque valeur dans chaque float[]
            for (int i = 0; i < countR.Length; i++)
            {
                countR[i] = countR[i] / (image.Hauteur * image.Largeur);
            }
            for (int i = 0; i < countV.Length; i++)
            {
                countV[i] = countV[i] / (image.Hauteur * image.Largeur);
            }
            for (int i = 0; i < countB.Length; i++)
            {
                countB[i] = countB[i] / (image.Hauteur * image.Largeur);
            }

            counts.Add(countR);
            counts.Add(countV);
            counts.Add(countB);

            return counts;
        }
        /// <summary>
        /// Fonction qui construit un arbre par couleur
        /// </summary>
        private List<HuffmanNode> CreateHuffmanTree(List<float[]> frequence)
        {
            //arbre general
            List<HuffmanNode> generalTree = new List<HuffmanNode>();
            for (int i = 0; i < frequence.Count; i++)
            {
                //arbre d'une couleur specifique
                List<HuffmanNode> tree = new List<HuffmanNode>();
                //ajoute le nombre de cellule avec leur valeur et frequence
                for (int j = 0; j < frequence[i].Length; j++)
                {
                    tree.Add(new HuffmanNode(j, frequence[i][j]));
                }
                //on trie notre list de la plus petite frequence a la plus grande
                tree.Sort((n1, n2) => n1.Frequency.CompareTo(n2.Frequency));
                //boucle pour cree mon arbre
                while (tree.Count > 1)
                {
                    //prend le noeud avec la plus basse frequence
                    HuffmanNode leftNode = tree[0];
                    //on l'enleve de notre arbre
                    tree.RemoveAt(0);
                    //prend le noeud avec la plus basse frequence
                    HuffmanNode rightNode = tree[0];
                    //on l'enleve de notre arbre
                    tree.RemoveAt(0);
                    //on cree notre noeud parent qui va contenir a gauche leftNode et a droite rightNode et on ajoute les deux frequence la valeur est sans importance
                    HuffmanNode parentNode = new HuffmanNode(-1, leftNode.Frequency + rightNode.Frequency);
                    parentNode.Left = leftNode;
                    parentNode.Right = rightNode;
                    //on le rajoute a la fin de notre list
                    tree.Add(parentNode);
                }
                //renvoie la racine
                generalTree.Add(tree[0]);
            }

            return generalTree;
        }
        /// <summary>
        /// Fonction qui construit mon dictionnaire qui donne le chemin dans mon arbre pour chaque valeur possible
        /// </summary>
        private List<Dictionary<int, string>> BuildCodeTable(List<HuffmanNode> roots)
        {
            //List de mes dictionnaires de couleurs
            List<Dictionary<int, string>> codetables = new List<Dictionary<int, string>>();
            for (int i = 0; i < roots.Count; i++)
            {
                //Creation de dictionnaire pour ma couleur
                Dictionary<int, string> codeTable = new Dictionary<int, string>();
                string currentCode = "";
                //fonction recursive pour fabriquer mon string bin depuis mon arbre
                void BuildCodeTableHelper(HuffmanNode node, string code)
                {
                    if (node == null)
                    {
                        return;
                    }

                    if (node.Left == null && node.Right == null)
                    {
                        codeTable.Add(node.Value, code);
                    }
                    else
                    {
                        BuildCodeTableHelper(node.Left, code + "0");
                        BuildCodeTableHelper(node.Right, code + "1");
                    }
                }
                //on lance la fonction depuis la racine de mon arbre
                BuildCodeTableHelper(roots[i], currentCode);
                codetables.Add(codeTable);
            }
            return codetables;
        }
        /// <summary>
        /// Fonction Compresser une image qui va encode dans l'ordre height, width, chaque codeTable de couleur puis l'image
        /// </summary>
        public void CompressImage(MyImage myImage, string output)
        {
            //Frequences couleurs de l'image
            List<float[]> frequence = Frequence(myImage);
            //Arbres couleurs de l'image
            List<HuffmanNode> huffmanNodes = CreateHuffmanTree(frequence);
            //Dicos couleurs int to bin string 
            List<Dictionary<int, string>> codeTables = BuildCodeTable(huffmanNodes);

            //On verifie que l'image et bien au format bmp
            int height = myImage.Hauteur;
            int width = myImage.Largeur;
            width -= width % 4;
            height -= height % 4;

            //passage de int vers string bin
            (string, string, string)[,] imageByteCode = new (string, string, string)[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    string red = codeTables[0][myImage.Image[i, j].Item1];
                    string green = codeTables[1][myImage.Image[i, j].Item2];
                    string blue = codeTables[2][myImage.Image[i, j].Item3];
                    imageByteCode[i, j] = (red, green, blue);
                }
            }

            //calcul du nombre byte necessaire
            int totalBytes = 0;
            totalBytes += 4;
            totalBytes += 4;
            foreach (var codeTable in codeTables)
            {
                totalBytes += codeTable.Count;
            }
            totalBytes += height * width * 3;

            //byte[] comprenenant tout nos donnes
            byte[] compressDataBytes = new byte[totalBytes];

            int offset = 0;
            //hauteur en little endian
            compressDataBytes[offset++] = (byte)(height & 0xFF);
            compressDataBytes[offset++] = (byte)((height >> 8) & 0xFF);
            compressDataBytes[offset++] = (byte)((height >> 16) & 0xFF);
            compressDataBytes[offset++] = (byte)(height >> 24);
            //largeur en little endian
            compressDataBytes[offset++] = (byte)(width & 0xFF);
            compressDataBytes[offset++] = (byte)((width >> 8) & 0xFF);
            compressDataBytes[offset++] = (byte)((width >> 16) & 0xFF);
            compressDataBytes[offset++] = (byte)(width >> 24);

            //Compression des tableaux
            for (int i = 0; i<codeTables.Count;i++)
            {
                for (int j = 0; j<codeTables[i].Count;j++)//codeTables[i].Count == 256
                {
                    compressDataBytes[offset++] = Convert.ToByte(codeTables[i][j].Substring(0, 8), 2);
                }
            }
            //Compression de l'image
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    //on determine le nombre de byte necessaire (on sait que c'est 1 mais sans ça ne marche pas)
                    int numOfBytesRed = imageByteCode[i, j].Item1.Length / 8;
                    byte[] bytesRed = new byte[numOfBytesRed];
                    //Convertie mon bin string en byte
                    for (int l = 0; l < numOfBytesRed; ++l)
                    {
                        bytesRed[l] = Convert.ToByte(imageByteCode[i, j].Item1.Substring(8 * l, 8), 2);
                    }
                    //on rajoute a la donne compresse
                    compressDataBytes[offset++] = bytesRed[0];

                    int numOfBytesGreen = imageByteCode[i, j].Item2.Length / 8;
                    byte[] bytesGreen = new byte[numOfBytesGreen];
                    for (int l = 0; l < numOfBytesGreen; ++l)
                    {
                        bytesGreen[l] = Convert.ToByte(imageByteCode[i, j].Item2.Substring(8 * l, 8), 2);
                    }
                    compressDataBytes[offset++] = bytesGreen[0];

                    int numOfBytesBlue = imageByteCode[i, j].Item2.Length / 8;
                    byte[] bytesBlue = new byte[numOfBytesBlue];
                    for (int l = 0; l < numOfBytesBlue; ++l)
                    {
                        bytesBlue[l] = Convert.ToByte(imageByteCode[i, j].Item2.Substring(8 * l, 8), 2);
                    }
                    compressDataBytes[offset++] = bytesBlue[0];
                }
            }
            //ecriture du fichier avec les donnes compressée
            File.WriteAllBytes(output, compressDataBytes);

        }
        /// <summary>
        /// Fonction Decompresser une image qui va decode dans l'ordre height, width, chaque codeTable de couleur puis l'image et renvoie une MyImage
        /// </summary>
        public MyImage DecompressImage(string input)
        {
            //bytes[] extrait de notre fichier
            byte[] compressdata = File.ReadAllBytes(input);
            
            int offset = 0;
            //byte[] height et width
            byte[] heightByte = new byte[4];
            byte[] widthByte = new byte[4];
            //lecture pour height et width
            for (int i = 0;i<4;i++)
            {
                heightByte[i] = compressdata[i];
                widthByte[i] = compressdata[i+4];
            }
            //passage en int
            int height = BitConverter.ToInt32(heightByte, 0);
            int width = BitConverter.ToInt32(widthByte, 0);
            offset +=8;

            //dico par couleur
            Dictionary<int,string> codeTableRed = new Dictionary<int, string>();
            Dictionary<int,string> codeTableGreen = new Dictionary<int, string>();
            Dictionary<int,string> codeTableBlue = new Dictionary<int, string>();
            //extraction dico rouge
            for (int i = 0; i<256;i++)
            {
                codeTableRed.Add(i,compressdata[offset+i].ToString());
            }
            offset+=256;
            //extraction dico vert
            for (int i = 0; i<256;i++)
            {
                codeTableGreen.Add(i,compressdata[offset+i].ToString());
            }
            offset+=256;
            //extraction dico bleu
            for (int i = 0; i<256;i++)
            {
                codeTableBlue.Add(i,compressdata[offset+i].ToString());
            }
            offset+=256;
            //retourne les dictionnaires
            var reversedCodeTableRed = codeTableRed.ToDictionary(x => x.Value, x => x.Key);
            var reversedCodeTableGreen = codeTableGreen.ToDictionary(x => x.Value, x => x.Key);
            var reversedCodeTableBlue = codeTableBlue.ToDictionary(x => x.Value, x => x.Key);

            //creation de l'image de sortie
            MyImage decompressedImage = new MyImage(width,height);

            //affectation des couleurs
            for (int i = 0; i<height; i++)
            {
                for (int j = 0; j<width; j++)
                {

                    decompressedImage.Image[i,j].Item1 = reversedCodeTableRed[compressdata[offset++].ToString()];
                    decompressedImage.Image[i,j].Item2 = reversedCodeTableGreen[compressdata[offset++].ToString()];
                    decompressedImage.Image[i,j].Item3 = reversedCodeTableBlue[compressdata[offset++].ToString()];
                }
            }

            return decompressedImage;
        }
    }

}
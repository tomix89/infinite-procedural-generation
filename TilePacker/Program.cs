using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePacker {
    class Program {

        // this was only ever run from the editor
        static readonly string TILE_FOLDER_PATH = @"..\..\..\TileTest\tiles";
        static List<ushort> palette = new List<ushort>();
        static Dictionary<ushort, byte> usedColors = new Dictionary<ushort, byte>();
        static List<StringBuilder> imageCodedList = new List<StringBuilder>();
        static List<string> imageEnumsList = new List<string>();

        static void Main(string[] args) {

            // Process the png files
            string[] fileEntries = Directory.GetFiles(TILE_FOLDER_PATH);
            foreach (string fileName in fileEntries)
                if (fileName.EndsWith(".png")) {
                    ProcessFile(fileName);
                }

            writeFinalFile();

        }

        // we need a RGB 565 format
        // and yes i know there are faster ways to get the byte data
        // but we do not need to process neither UHD images nor a lot of them
        // so i can live with this optimization not done
        static ushort ColorTo565(Color color) {
            ushort retVal = 0;
            retVal |= (ushort)((color.R & 0xF8) << 8);
            retVal |= (ushort)((color.G & 0xFC) << 3);
            retVal |= (ushort)(color.B >> 3);
            return retVal;
        }


        static void ProcessFile(string filename) {

            Console.WriteLine("Procesing: " + filename);
            Bitmap bitmap = new Bitmap(filename);
            int w = bitmap.Width;
            int h = bitmap.Height;
            int cntr = 0;
            StringBuilder sb = new StringBuilder();

            string imgName = Path.GetFileNameWithoutExtension(filename);
            imageEnumsList.Add(imgName.ToUpper());

            sb.Append("const uint8 " + imgName + "[] = {\n");

            // since we will draw lines on the ESP32 in vertical manner, we will code them like this
            for (int x = 0; x < w; ++x) {
                for (int y = 0; y < h; ++y) {
                    ushort color565 = ColorTo565(bitmap.GetPixel(x, y));
                    byte indexed;

                    // check if we have this already in the palette
                    if (usedColors.ContainsKey(color565)) {
                        indexed = usedColors[color565];
                    } else {
                        if (palette.Count > 0xFF) {
                            Console.WriteLine("Too many colors, need to fit into 256");
                            return;
                        }

                        byte idx = (byte)palette.Count;
                        usedColors.Add(color565, idx);
                        indexed = idx;
                        palette.Add(color565);
                    }


                    if (cntr == 0) {
                        sb.Append("0x" + indexed.ToString("X2"));
                    } else {
                        sb.Append(", 0x" + indexed.ToString("X2"));
                    }


                    cntr++;
                    // newline each 16 byte(text) 
                    if (cntr >= 16) {

                        if (x * y < (w - 1) * (h - 1)) {
                            sb.Append(",\n");
                        } else {
                            sb.Append("\n}"); // last one closes
                        }

                        cntr = 0;
                    }

                }
            }

            imageCodedList.Add(sb);
        }


        static void writeFinalFile() {
            // first all the images

           
            var sw = new StreamWriter("tiles.h", false);

            for (int i = 0; i < imageCodedList.Count; ++i) {
                sw.Write(imageCodedList[i].ToString());
                sw.Write("\n\n");
            }

            // write out array


            Console.WriteLine("Total palette colors: " + palette.Count);
            // write out palette 

            // write out enums

            //

            sw.Close();


        }


    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TilePacker {
    class Program {

        enum BuildType {
            C_PP,
            C_SHARP,
        }

        const BuildType buildType = BuildType.C_SHARP;

        // this was only ever run from the editor
        static readonly string TILE_FOLDER_PATH = @"tiles";
        static List<ushort> palette = new List<ushort>();
        static Dictionary<ushort, byte> usedColors = new Dictionary<ushort, byte>();
        static List<StringBuilder> imageCodedList = new List<StringBuilder>();
        static List<string> imageNamesList = new List<string>();
        static readonly string INDENT = "    ";

        static void Main(string[] args) {

            // Process the png files
            string[] fileEntries = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + TILE_FOLDER_PATH);
            foreach (string fileName in fileEntries)
                if (fileName.EndsWith(".png")) {
                    ProcessFile(fileName);
                }

            writeFinalFile();
            Console.WriteLine("\nDONE, press any key to exit");
            Console.ReadKey();
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

            Console.WriteLine("Processing: " + filename);
            Bitmap bitmap = new Bitmap(filename);
            int w = bitmap.Width;
            int h = bitmap.Height;
            int cntr = 0;
            StringBuilder sb = new StringBuilder();

            string imgName = Path.GetFileNameWithoutExtension(filename);
            imageNamesList.Add(imgName);

            if (buildType == BuildType.C_PP) {
                sb.Append("const uint8 " + imgName + "[] = {\n");
            } else {
                sb.Append("static readonly byte[] " + imgName + " = {\n");
            }

            // since we will draw lines on the ESP32 in vertical mode, we will code them like this
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
                        sb.Append(INDENT + "0x" + indexed.ToString("X2"));
                    } else {
                        sb.Append(", 0x" + indexed.ToString("X2"));
                    }


                    cntr++;
                    // newline each 16 byte(text) 
                    if (cntr >= 16) {

                        if (x * y < (w - 1) * (h - 1)) {
                            sb.Append(",\n");
                        } else {
                            sb.Append("\n};"); // last one closes
                        }

                        cntr = 0;
                    }

                }
            }

            imageCodedList.Add(sb);
        }

        static void writeFinalFile() {
            // first all the images
            StreamWriter sw = null;

            if (buildType == BuildType.C_PP) {
                sw = new StreamWriter("tiles.h", false);
            } else {
                sw = new StreamWriter("Tiles.cs", false);

                sw.WriteLine("using System.Collections.Generic;\n");
                sw.WriteLine("public class Tiles {");
            }

            for (int i = 0; i < imageCodedList.Count; ++i) {
                sw.Write(imageCodedList[i].ToString());
                sw.Write("\n\n");
            }



            Console.WriteLine("Total palette colors: " + palette.Count);
            // write out palette 
            int cntr = 0;
            sw.WriteLine("// in reality only " + palette.Count + " is used");
            sw.WriteLine("public static readonly ushort[] palette = {");


            for (int i = 0; i <= 0xFF; ++i) {
                UInt16 num = 0;
                if (i < palette.Count) {
                    num = palette[i];
                }

                if (cntr == 0) {
                    sw.Write(INDENT + "0x" + num.ToString("X4"));
                } else {
                    sw.Write(", 0x" + num.ToString("X4"));
                }

                cntr++;
                // newline each 16 byte(text) 
                if (cntr >= 8) {

                    if (i < 255) {
                        sw.Write(",\n");
                    } else {
                        sw.Write("\n};"); // last one closes
                    }

                    cntr = 0;
                }
            }


            // helper for counting
            sw.Write("\n\n\n");

            if (buildType == BuildType.C_PP) {
                sw.WriteLine("const uint16 TILE_COUNT = " + imageNamesList.Count + ";");
            } else {
                sw.WriteLine("public const int TILES_COUNT = " + imageNamesList.Count + ";");
            }

            

            // write out array of images (for easy enum indexing)
            sw.Write("\n");
            if (buildType == BuildType.C_PP) {
                sw.WriteLine("const uint8* allTiles[] = {");
            } else {
                sw.WriteLine("public static readonly List<byte[]> allTiles = new List<byte[]> {");
            }

            for (int i = 0; i < imageNamesList.Count; ++i) {
                if (i == 0) {
                    sw.Write(INDENT + imageNamesList[i]);
                } else {
                    sw.Write(",\n" + INDENT + imageNamesList[i]);
                }
            }
            sw.Write("\n};");



            sw.Write("\n\n");
            // write out enums
            sw.WriteLine("public enum ImageName {");
            for (int i = 0; i < imageNamesList.Count; ++i) {
                if (i == 0) {
                    sw.Write(INDENT + imageNamesList[i].ToUpper() + "=0");
                } else {
                    sw.Write(",\n" + INDENT + imageNamesList[i].ToUpper());
                }
            }

            sw.Write(",\n" + INDENT + "INVALID=0xFF");
            sw.WriteLine("\n}");

            if (buildType == BuildType.C_SHARP) {
                sw.WriteLine("\n}"); // closes class
            }

            sw.Flush();
            sw.Close();


            if (buildType == BuildType.C_SHARP) {
                // copy the new file to the main project
                File.Copy("Tiles.cs", @"..\..\..\TileTest\Tiles.cs", true);
            }

        }
    }
}

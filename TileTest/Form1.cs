﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TileTest {
    public partial class Form1 : Form {

        public Form1() {
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        const int TILE_TOUCH_POINTS = 2;

        Random rnd;    

        private enum TileType {
            GROUND,
            CLOUD,
            SKY,
            BIGHOUSE,
            FOREST,
        }

        private enum TileTag {
            NONE,
            FLYING_GROUND,
            CLOUD,
            BIGHOUSE_NOT_BASE,
        }

        const int CANVAS_SIZE_X = 10 + 1; // canvas needs to be one tile more to X as we will scroll in X direction
        const int CANVAS_SIZE_Y = 8;

        // canvas 0,0 is top left
        Tiles.ImageName[,] canvas = new Tiles.ImageName[CANVAS_SIZE_X, CANVAS_SIZE_Y];

        int xShift = 0; // 0 to tile size

        int[,] adjOffsets = new int[2, 2] {
                //{0, -1}, // top
                {0, 1}, // bottom
              //  {1, 0}, // right
                {-1, 0} // left
            };

        char[] adjDirections = new char[2] { 'B', 'L' };

        // so in ESSP 32 we don't have RAM to contain a full 16b image, so we will write it col by col
        // emulating this here will be via one large bitmap (to overcome writing to the form directly)
        static ushort[,] display = new ushort[320, 240];
        byte[] linearDisplayBuff = new byte[display.Length * 2];
        Bitmap screen = new Bitmap(320, 240, PixelFormat.Format16bppRgb565);
   

        void writeTileCol(Tiles.ImageName name, int tileCol, int dispCol, int yIdx) {
            int offset = 0;
            // the topmost tile is drawn only from half
            if (yIdx == 0) {
                offset = 16;
            }
            for (int y = offset; y < 32; ++y) {
                byte clrIdx = Tiles.allTiles[(int)name][tileCol * 32 + y];
                ushort clr565 = Tiles.palette[clrIdx];

                display[dispCol, y - 16 + yIdx * 32] = clr565;
            }
        }

        private struct TileProp {

            public TileProp(Tiles.ImageName name, TileType[,] map, TileTag tag = TileTag.NONE) {
                this.name = name;
                this.tag = tag;
                this.map = map;
            }

            public Tiles.ImageName name;
            public TileType[,] map;
            public TileTag tag;

            public bool isTileMatching(char side, TileType[,] toMatch) {

                bool isMatch = true;
                switch (side) {
                    // we are probing the bottom of an already placed tile 
                    // to the top side of this 
                    case 'T':
                        isMatch &= toMatch[1, 0] == map[0, 0];
                        isMatch &= toMatch[1, 1] == map[0, 1];
                        break;

                    // we are probing the top of an already placed tile 
                    // to the bottom side of this 
                    case 'B':
                        isMatch &= toMatch[0, 0] == map[1, 0];
                        isMatch &= toMatch[0, 1] == map[1, 1];
                        break;

                    // we are probing the right of an already placed tile 
                    // to the left side of this 
                    case 'L':
                        isMatch &= toMatch[0, 1] == map[0, 0];
                        isMatch &= toMatch[1, 1] == map[1, 0];
                        break;

                    // we are probing the left of an already placed tile 
                    // to the right side of this 
                    case 'R':
                        isMatch &= toMatch[0, 0] == map[0, 1];
                        isMatch &= toMatch[1, 0] == map[1, 1];
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return isMatch;
            }
        }

        private List<TileProp> tiles = new List<TileProp>();

        private void Form1_Load(object sender, EventArgs e) {
            rnd = new Random();

            tiles.Add(new TileProp(Tiles.ImageName.BIGHOUSE_MB,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.BIGHOUSE, TileType.BIGHOUSE },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.BIGHOUSE_MT,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.BIGHOUSE, TileType.BIGHOUSE  }
            }, TileTag.BIGHOUSE_NOT_BASE));

            tiles.Add(new TileProp(Tiles.ImageName.BIGHOUSE_LB,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.BIGHOUSE, TileType.SKY },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.BIGHOUSE_LT,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.BIGHOUSE, TileType.SKY }
            }, TileTag.BIGHOUSE_NOT_BASE));

            tiles.Add(new TileProp(Tiles.ImageName.BIGHOUSE_RB,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.BIGHOUSE },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.BIGHOUSE_RT,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.SKY, TileType.BIGHOUSE }
            }, TileTag.BIGHOUSE_NOT_BASE));

            tiles.Add(new TileProp(Tiles.ImageName.BIGHOUSE_MM,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.BIGHOUSE, TileType.BIGHOUSE },
                { TileType.BIGHOUSE, TileType.BIGHOUSE }
            }, TileTag.BIGHOUSE_NOT_BASE));

            tiles.Add(new TileProp(Tiles.ImageName.BIGHOUSE_ML,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.BIGHOUSE, TileType.SKY },
                { TileType.BIGHOUSE, TileType.SKY }
            }, TileTag.BIGHOUSE_NOT_BASE));

            tiles.Add(new TileProp(Tiles.ImageName.BIGHOUSE_MR,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.BIGHOUSE },
                { TileType.SKY, TileType.BIGHOUSE }
            }, TileTag.BIGHOUSE_NOT_BASE));

            tiles.Add(new TileProp(Tiles.ImageName.CLOUD_B,
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.CLOUD, TileType.CLOUD },
                { TileType.SKY, TileType.SKY }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.CLOUD_BL,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.CLOUD, TileType.SKY },
                { TileType.SKY, TileType.SKY }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.CLOUD_BR,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.CLOUD },
                { TileType.SKY, TileType.SKY }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.CLOUD_T,
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.CLOUD, TileType.CLOUD }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.CLOUD_TL,
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.CLOUD, TileType.SKY }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.CLOUD_TR,
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.SKY, TileType.CLOUD }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.FOREST_BL,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.FOREST, TileType.SKY },
                { TileType.GROUND, TileType.GROUND }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.FOREST_BM,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.FOREST, TileType.FOREST },
                { TileType.GROUND, TileType.GROUND }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.FOREST_BR,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.FOREST },
                { TileType.GROUND, TileType.GROUND }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.FOREST_TL,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.FOREST, TileType.SKY }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.FOREST_TM,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.FOREST, TileType.FOREST }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.FOREST_TR,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.SKY, TileType.FOREST }
            }, TileTag.CLOUD));

            tiles.Add(new TileProp(Tiles.ImageName.GROUND,
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.GROUND, TileType.GROUND },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.GROUND_L,
             new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.GROUND, TileType.SKY },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.GROUND_R,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.GROUND },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.GROUND_T,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.GROUND_T_TREE1,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.GROUND_T_TREE2,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.GROUND_T_HOUSE1,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.GROUND_T_HOUSE2,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.WALL_BR,
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.GROUND },
                { TileType.SKY, TileType.SKY }
            }, TileTag.FLYING_GROUND));

            tiles.Add(new TileProp(Tiles.ImageName.WALL_BL,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.GROUND, TileType.SKY },
                { TileType.SKY, TileType.SKY }
            }, TileTag.FLYING_GROUND));

            tiles.Add(new TileProp(Tiles.ImageName.WALL_B,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.GROUND, TileType.GROUND },
                { TileType.SKY, TileType.SKY }
            }, TileTag.FLYING_GROUND));

            tiles.Add(new TileProp(Tiles.ImageName.SKY,
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.SKY, TileType.SKY }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.WALL_L,
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.GROUND, TileType.SKY },
                { TileType.GROUND, TileType.SKY }
            }));

            tiles.Add(new TileProp(Tiles.ImageName.WALL_TL,
               new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.GROUND, TileType.SKY }
           }));

            tiles.Add(new TileProp(Tiles.ImageName.WALL_R,
               new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.GROUND },
                { TileType.SKY, TileType.GROUND }
           }));

            tiles.Add(new TileProp(Tiles.ImageName.WALL_TR,
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.SKY, TileType.GROUND }
            }));

            // need to sort to match the enum indexes
            tiles.Sort((s1, s2) => s1.name.CompareTo(s2.name));

            for (int x = 0; x < CANVAS_SIZE_X; x++) {
                for (int y = 0; y < CANVAS_SIZE_Y; y++) {
                    canvas[x, y] = Tiles.ImageName.INVALID;
                }
            }

            generate(0);

            cBScrollSpeed.DataSource = Enum.GetNames(typeof(ScrollSpeed));
        }

        // https://stackoverflow.com/questions/11456440/how-to-resize-a-bitmap-image-in-c-sharp-without-blending-or-filtering
        private Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height) {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result)) {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.DrawImage(sourceBMP, 0, 0, width, height);
            }
            return result;
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            // we need to translate canvas to display columns
            // with the xShift. So we will iterate the col count (not canvas Idx)
            for (int dispCol = 0; dispCol < 320; ++dispCol) {
                // this is directly canvas Idx
                for (int yIdx = CANVAS_SIZE_Y - 1; yIdx >= 0; --yIdx) {
                    // and now calculate the xIdx 
                    int xIdx = (dispCol + xShift) / 32;
                    // and the col on the given tile
                    int tileCol = (dispCol + xShift) % 32;
                    if (canvas[xIdx, yIdx] < Tiles.ImageName.INVALID) {
                        writeTileCol(canvas[xIdx, yIdx], tileCol, dispCol, yIdx);
                    }
                }
            }

            // copy the 16b colors to a linear buffer for bitmap conversion
            int cntr = 0;
            for (int y = 0; y < 240; ++y) {
                for (int x = 0; x < 320; ++x) {
                    linearDisplayBuff[cntr++] = (byte)(display[x, y] & 0x00FF);
                    linearDisplayBuff[cntr++] = (byte)((display[x, y] >> 8) & 0x00FF); 
                }
            }

            ImageFromRaw565Array(linearDisplayBuff, screen, 320, 240);
            Bitmap resized = ResizeBitmap(screen, 320 * 2, 240 * 2);
            e.Graphics.DrawImage(resized, 10, 10);
            resized.Dispose();
        }


        private void generate(int x_start) {
            Console.WriteLine("---------------------");

            cHashSet bannedTiles = new cHashSet(Tiles.TILES_COUNT);

            // we have to fill in from bottom to top
            // to overcome invalid placing (sky below ground)
            for (int x = x_start; x < CANVAS_SIZE_X; x++) {
                for (int y = CANVAS_SIZE_Y - 1; y >= 0; y--) {

                    // and therefore we have to test only the 
                    // below and the left tile
                    cHashSet tilesToChose = new cHashSet(Tiles.TILES_COUNT);
                    for (int tileId = 0; tileId < Tiles.TILES_COUNT; tileId++) {
                        bool isFitting = true;

                        if (bannedTiles.contains(tileId)) {
                            continue;
                        }

                        // do not put bighouse tiles to the bottom most row
                        if (y >= CANVAS_SIZE_Y - 1) {
                            if (tiles[tileId].tag == TileTag.BIGHOUSE_NOT_BASE) {
                                continue;
                            }
                        }

                        // put cloud tiles only to the top rows
                        if (y > 2) {
                            if (tiles[tileId].tag == TileTag.CLOUD) {
                                continue;
                            }
                        }

                        // put flying ground tiles only on the mid part
                        if (tiles[tileId].tag == TileTag.FLYING_GROUND) {
                            if ((y > CANVAS_SIZE_Y - 3) || (y < 3)) {
                                continue;
                            }
                        }

                        for (int adj = 0; adj < 2; adj++) {
                            // check which can go in this position
                            int xn = x + adjOffsets[adj, 0];
                            int yn = y + adjOffsets[adj, 1];

                            // check if we are out of bounds
                            if (xn < 0 || xn >= CANVAS_SIZE_X) {
                                continue;
                            }
                            // check if we are out of bounds
                            if (yn < 0 || yn >= CANVAS_SIZE_Y) {
                                continue;
                            }

                            // if in bounds, check the adjecent tile's constatints
                            isFitting &= tiles[tileId].isTileMatching(adjDirections[adj], tiles[(int)canvas[xn, yn]].map);
                        }

                        if (isFitting) {
                            tilesToChose.add(tileId);
                        }

                    }

                    // at this point the array shall not be empty
                    // if it is empty we need to return one step
                    if (tilesToChose.count() == 0) {
                        Console.WriteLine("Banning: " + tiles[(int)canvas[x, y + 1]].name + " @ " + x + " " + y + " when size is: " + bannedTiles.count());
                        bannedTiles.add((int)canvas[x, y + 1]);
                        y += 2;
                        continue;
                    }

                    bannedTiles.clear();

                    // roll the dice and decide what to paint
                    int randIdx = rnd.Next(0, tilesToChose.count());
                    canvas[x, y] = (Tiles.ImageName)tilesToChose.getValueOfIndex((UInt32)randIdx);
                }
            }

        }

        private void button_new_Click(object sender, EventArgs e) {
            xShift = 0;
            cBScrollSpeed.SelectedIndex = 0; // will stop auto scroll if running
            for (int x = 0; x < CANVAS_SIZE_X; x++) {
                for (int y = 0; y < CANVAS_SIZE_Y; y++) {
                    canvas[x, y] = Tiles.ImageName.INVALID;
                }
            }

            generate(0);

            // the draw will be called
            this.Invalidate();
        }

        private void button_scroll_tile_Click(object sender, EventArgs e) {

            for (int x = 0; x < CANVAS_SIZE_X; x++) {
                for (int y = 0; y < CANVAS_SIZE_Y; y++) {

                    if (x == CANVAS_SIZE_X - 1) {
                        canvas[x, y] = Tiles.ImageName.INVALID;
                    } else {
                        canvas[x, y] = canvas[x + 1, y];
                    }
                }
            }

            generate(CANVAS_SIZE_X - 1);

            // the draw will be called
            this.Invalidate();
        }

        private void button_scroll_col_Click(object sender, EventArgs e) {
            xShift++;

            if (xShift >= 32) {

                xShift = 0;
                button_scroll_tile_Click(button_scroll_tile, null);
                return;
            }
            this.Invalidate();
        }



        private enum ScrollSpeed {
            NO_SCROLL = 0,
            FPS_1,
            FPS_2,
            FPS_5,
            FPS_10,
            FPS_20,
            FPS_50,
            FPS_100,
        }

        private void cBScrollSpeed_SelectedIndexChanged(object sender, EventArgs e) {

            ScrollSpeed ss = (ScrollSpeed)cBScrollSpeed.SelectedIndex;


            if (cBScrollSpeed.SelectedIndex == 0) {
                timer1.Stop();
            } else {
                timer1.Interval = (int)Math.Round(2148.7190921026 * Math.Exp(-0.76487189348237 * cBScrollSpeed.SelectedIndex));
                timer1.Start();
                Console.WriteLine(timer1.Interval);
            }

        }

        private void timer1_Tick(object sender, EventArgs e) {
            button_scroll_col_Click(button_scroll_col, null);
        }


        // https://stackoverflow.com/a/24315437
        private static Bitmap ImageFromRaw565Array(byte[] arr, Bitmap output, int width, int height) {
            var rect = new Rectangle(0, 0, width, height);
            var bmpData = output.LockBits(rect, ImageLockMode.ReadWrite, output.PixelFormat);
            var ptr = bmpData.Scan0;
            Marshal.Copy(arr, 0, ptr, arr.Length);
            output.UnlockBits(bmpData);
            return output;
        }

    }
}
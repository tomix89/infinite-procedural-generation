using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TileTest {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
        }

        const int TILE_TOUCH_POINTS = 2;

        Random rnd;

        private enum TileType {
            GROUND,
            CLOUD,
            SKY,
        }

        const int CANVAS_SIZE_X = 10;
        const int CANVAS_SIZE_Y = 8;


        int[,] adjOffsets = new int[2, 2] {
                //{0, -1}, // top
                {0, 1}, // bottom
              //  {1, 0}, // right
                {-1, 0} // left
            };

        char[] adjDirections = new char[2] { 'B', 'L' };


        // canvas 0,0 is top left
        int[,] canvas = new int[CANVAS_SIZE_X, CANVAS_SIZE_Y];

        private struct TileProp {

            public TileProp(string name, TileType[,] map) {
                this.name = name;
                this.img = new Bitmap("Tiles\\" + name + ".png");
                // resize
                this.img = new Bitmap(this.img, new Size(64, 64));
                
                this.treats = new HashSet<TileType>();
                for (int x = 0; x < TILE_TOUCH_POINTS; x++) {
                    for (int y = 0; y < TILE_TOUCH_POINTS; y++) {
                        this.treats.Add(map[x, y]); // this auto filters out duplicates
                    }
                }

                this.map = map;
            }

            public string name; // also the path
            public TileType[,] map;
            public Bitmap img;
            public HashSet<TileType> treats;


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

            tiles.Add(new TileProp("cloud_B",
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.CLOUD, TileType.CLOUD },
                { TileType.SKY, TileType.SKY }
            }));

            tiles.Add(new TileProp("cloud_BL",
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.CLOUD, TileType.SKY },
                { TileType.SKY, TileType.SKY }
            }));

            tiles.Add(new TileProp("cloud_BR",
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.CLOUD },
                { TileType.SKY, TileType.SKY }
            }));


            tiles.Add(new TileProp("cloud_T",
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.CLOUD, TileType.CLOUD }
            }));

            tiles.Add(new TileProp("cloud_TL",
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.CLOUD, TileType.SKY }
            }));

            tiles.Add(new TileProp("cloud_TR",
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.SKY, TileType.CLOUD }
            }));

            tiles.Add(new TileProp("ground",
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.GROUND, TileType.GROUND },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp("ground_L",
             new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.GROUND, TileType.SKY },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp("ground_R",
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.GROUND },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp("ground_T",
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp("ground_T2",
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.GROUND, TileType.GROUND }
            }));

            tiles.Add(new TileProp("sky",
            new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.SKY, TileType.SKY }
            }));

            tiles.Add(new TileProp("wall_L",
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.GROUND, TileType.SKY },
                { TileType.GROUND, TileType.SKY }
            }));

            tiles.Add(new TileProp("wall_LT",
               new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.GROUND, TileType.SKY }
           }));

            tiles.Add(new TileProp("wall_R",
               new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.GROUND },
                { TileType.SKY, TileType.GROUND }
           }));

            tiles.Add(new TileProp("wall_RT",
                new TileType[TILE_TOUCH_POINTS, TILE_TOUCH_POINTS] {
                { TileType.SKY, TileType.SKY },
                { TileType.SKY, TileType.GROUND }
            }));

            for (int x = 0; x < CANVAS_SIZE_X; x++) {
                for (int y = 0; y < CANVAS_SIZE_Y; y++) {
                    canvas[x, y] = 0xFF;
                }
            }

            generate(0);
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            for (int x = 0; x < CANVAS_SIZE_X; x++) {
                for (int y = CANVAS_SIZE_Y - 1; y >= 0; y--) {

                    if (canvas[x, y] < 0xFF) {
                        e.Graphics.DrawImage(tiles[canvas[x, y]].img,
                            10 + x * 32 * 2,
                            10 + y * 32 * 2);
                    }
                }
            }
        }


        private void generate(int x_start) {
            HashSet<int> bannedTiles = new HashSet<int>();

            // we have to fill in from bottom to top
            // to overcome invalid placing (sky below ground)
            for (int x = x_start; x < CANVAS_SIZE_X; x++) {
                for (int y = CANVAS_SIZE_Y - 1; y >= 0; y--) {

                    // and therefore we have to test only the 
                    // below and the left tile
                    List<int> tilesToChose = new List<int>();
                    for (int i = 0; i < tiles.Count; i++) {
                        bool isFitting = true;

                        if (bannedTiles.Contains(i)) {
                            continue;
                        }

                        if (y > CANVAS_SIZE_Y - 6) {
                            if (tiles[i].name.StartsWith("cloud")) {
                                continue;
                            }
                        }

                        // do not put sky or cloud tiles to the bottom most row
                        if (y > CANVAS_SIZE_Y - 2) {
                            if (tiles[i].name == "sky") {
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
                            isFitting &= tiles[i].isTileMatching(adjDirections[adj], tiles[canvas[xn, yn]].map);
                        }

                        if (isFitting) {
                            tilesToChose.Add(i);
                        }
                    }

                    // at this point the array shall not be empty
                    // if it is empty we need to return one step
                    if (tilesToChose.Count == 0) {
                        bannedTiles.Add(canvas[x, y + 1]);
                        y += 2;
                        continue;
                    }

                    bannedTiles.Clear();

                    // roll the dice and decide what to paint
                    int randIdx = rnd.Next(0, tilesToChose.Count);
                    canvas[x, y] = tilesToChose[randIdx];
                }
            }

        }

        private void button_new_Click(object sender, EventArgs e) {

            for (int x = 0; x < CANVAS_SIZE_X; x++) {
                for (int y = 0; y < CANVAS_SIZE_Y; y++) {
                    canvas[x, y] = 0xFF;
                }
            }

            generate(0);

            // the draw will be called
            this.Invalidate();
        }

        private void button_scroll_Click(object sender, EventArgs e) {

            for (int x = 0; x < CANVAS_SIZE_X; x++) {
                for (int y = 0; y < CANVAS_SIZE_Y; y++) {

                    if (x == CANVAS_SIZE_X - 1) {
                        canvas[x, y] = 0xFF;
                    } else {
                        canvas[x, y] = canvas[x + 1, y];
                    }
                }
            }

            generate(CANVAS_SIZE_X - 1);

            // the draw will be called
            this.Invalidate();
        }



    }
}

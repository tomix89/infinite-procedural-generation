using System;

namespace TileTest {

    // since in esp32 we wont have fancy hashset, we implement ours
    // this is really trivial and optimized only for continuous enum numbers (e.g. no gaps and jumps)
    class cHashSet {
        // as of now we have 31 tiles, so they will fit into 1 32b value

        private const int STORAGE_SIZE = 2;
        private UInt32[] storage = new UInt32[STORAGE_SIZE];


        public cHashSet(int maxNumOfItems) {
            if (maxNumOfItems > STORAGE_SIZE * 32) {
                throw new ArgumentOutOfRangeException();        
            }
        }

        public void clear() {
            for (int id = 0; id < STORAGE_SIZE; ++id) {
                storage[id] = 0;
            }
        }

        public void add(int value) {
            int id = value / 32;
            storage[id] |= (1U << (value % 32));
        }

        public bool contains(int value) {
            int id = value / 32;
            return (storage[id] & (1U << (value % 32))) > 0;
        }

        public UInt32 count() {
            UInt32 count = 0;
            for (int id = 0; id < STORAGE_SIZE; ++id) {
                count += populationCount(storage[id]);
            }
            return count;
        }

        // https://github.com/hcs0/Hackers-Delight/blob/master/popArrayHS.c.txt
        private UInt32 populationCount(UInt32 x) {
            x = x - ((x >> 1) & 0x55555555);
            x = (x & 0x33333333) + ((x >> 2) & 0x33333333);
            x = (x + (x >> 4)) & 0x0F0F0F0F;
            x = x + (x >> 8);
            x = x + (x >> 16);
            return x & 0x0000003F;
        }
        /* Note: an alternative to the last three executable lines above is:
           return x*0x01010101 >> 24;
        if your machine has a fast multiplier (suggested by Jari Kirma). */

    }

}

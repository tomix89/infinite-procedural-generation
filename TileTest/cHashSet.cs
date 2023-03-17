using System;

namespace TileTest {

    // since in esp32 we wont have fancy hashset, we implement ours
    // this is really trivial and optimized only for continuous enum numbers (e.g. no gaps and jumps)
    class cHashSet {

        // as of now we have 37 tiles, so they will fit into 2 32b value
        private const int STORAGE_SIZE = 2;
        private int itemCount = 0;
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
            itemCount = 0;
        }

        public void add(int value) {
            int id = value / 32;
            UInt32 bit = 1U << (value % 32);

            // increase count only when we haven't had this bit
            if ((storage[id] & bit) != 0) {
                return;
            }

            itemCount++;
            storage[id] |= bit;
        }

        public bool contains(int value) {
            int id = value / 32;
            return (storage[id] & (1U << (value % 32))) > 0;
        }

        public int count() {
            return itemCount;
        }
    }
}

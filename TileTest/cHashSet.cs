using System;

namespace TileTest {

    // since in esp32 we wont have fancy hashset, we implement ours
    // this is really trivial and optimized only for continuous enum numbers (e.g. no gaps and jumps)
    class cHashSet {

        // as of now we have 37 tiles, so they will fit into 2 32b value
        private const int STORAGE_SIZE = 2;
        private int itemCount = 0;
        private UInt32[] storage = new UInt32[STORAGE_SIZE];

        public const UInt32 INVALID_VALUE = UInt32.MaxValue;

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

        // this replaces the hash to list conversion
        // and then indexing the list
        // n is zero based index
        //
        // example we have internally set bits 1,5,12 = 0001 0000 0010 0010
        // therefore if we would convert hashset to list we would get an list with 3 items
        // list[0] -> 1
        // list[1] -> 5
        // list[3] -> 12
        // now if user asks for 1, it shall return 5


        // NOTE i'll bench this on the ESP32 and will se how it behaves.
        // if it too slow, we would just take the array approach
        // that will consume as many Bytes as long the set needs to be 
        // but it will be ligtning fast to return these indexes
        public UInt32 getValueOfIndex(UInt32 n) {

            UInt32 id = 0;
            UInt32 popCount;
            UInt32 retVal = nth_bit_set(storage[id], n, out popCount);

            while (retVal == INVALID_VALUE && id < STORAGE_SIZE - 1) {
                n = n - popCount; // need to lower by the previus pop count
                id++;
                retVal = nth_bit_set(storage[id], n, out popCount);
            }

            if (retVal == INVALID_VALUE) {
                return INVALID_VALUE;
            } else {
                return retVal + 32 * id; // also has to compensate the output
            }
        }


        // https://stackoverflow.com/a/45487375
        // https://stackoverflow.com/questions/7669057/find-nth-set-bit-in-an-int/45487375#45487375

        // n is zero based index
        private UInt32 nth_bit_set(UInt32 value, UInt32 n, out UInt32 popCount) {
            UInt32 pop2 = (value & 0x55555555u) + ((value >> 1) & 0x55555555u);
            UInt32 pop4 = (pop2 & 0x33333333u) + ((pop2 >> 2) & 0x33333333u);
            UInt32 pop8 = (pop4 & 0x0f0f0f0fu) + ((pop4 >> 4) & 0x0f0f0f0fu);
            UInt32 pop16 = (pop8 & 0x00ff00ffu) + ((pop8 >> 8) & 0x00ff00ffu);
            UInt32 pop32 = (pop16 & 0x000000ffu) + ((pop16 >> 16) & 0x000000ffu);
            UInt32 rank = 0;
            UInt32 temp;
            popCount = pop32;

            if (n++ >= pop32) {
                return INVALID_VALUE;
            }

            temp = pop16 & 0xffu;
            /* if (n > temp) { n -= temp; rank += 16; } */
            rank += ((temp - n) & 256) >> 4;
            n -= temp & ((temp - n) >> 8);

            temp = (pop8 >> (int)rank) & 0xffu;
            /* if (n > temp) { n -= temp; rank += 8; } */
            rank += ((temp - n) & 256) >> 5;
            n -= temp & ((temp - n) >> 8);

            temp = (pop4 >> (int)rank) & 0x0fu;
            /* if (n > temp) { n -= temp; rank += 4; } */
            rank += ((temp - n) & 256) >> 6;
            n -= temp & ((temp - n) >> 8);

            temp = (pop2 >> (int)rank) & 0x03u;
            /* if (n > temp) { n -= temp; rank += 2; } */
            rank += ((temp - n) & 256) >> 7;
            n -= temp & ((temp - n) >> 8);

            temp = (value >> (int)rank) & 0x01u;
            /* if (n > temp) rank += 1; */
            rank += ((temp - n) & 256) >> 8;

            return rank;
        }

    }
}

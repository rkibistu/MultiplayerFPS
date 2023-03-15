using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NumberExtensions
{
    public static bool IsBitSet(this byte flags, int bit) {
        return (flags & (1 << bit)) == (1 << bit);
    }

    public static byte SetBit(ref this byte flags, int bit, bool value) {
        if (value == true) {
            return flags |= (byte)(1 << bit);
        }
        else {
            return flags &= unchecked((byte)~(1 << bit));
        }
    }

    public static bool IsBitSet(this int flags, int bit) {
        return (flags & (1 << bit)) == (1 << bit);
    }

}

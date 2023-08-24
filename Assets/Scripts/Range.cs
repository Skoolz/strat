using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range<T>
{
    public T min {get; private set;}
    public T max {get; private set;}

    public Range(T min, T max) {
        this.min = min;
        this.max = max;
    }
}

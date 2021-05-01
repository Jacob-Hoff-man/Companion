#include "RollAvg.h"


RollAvg::RollAvg(int rollingAvgLength) {
    avgLen = rollingAvgLength;
    avgs = new float[rollingAvgLength];
}


void RollAvg::updateAvg(float val) {
    value = val;
    avgs[index] = val;// - base;
    int nextIndex = (index + 1) % avgLen;
    if (!rolledOver && !nextIndex) {
        rolledOver = 1;
    }
    index = nextIndex;
}


float RollAvg::rollingAvg() {
    float avg = 0;
    int len = avgLen;

    if (!rolledOver) { // check to see if we have populated the rolling avg array
        if (!index) {
            return 0; //no val's exist.
        }
        len = index; //if not we will only calculate the avg for what exists.
    }

    for (int i = 0; i < len; i++) {
        avg += avgs[i] / (float)len;
    }
    return avg;
}


#ifndef INCLUDE_ROLLAVG
#define INCLUDE_ROLLAVG


class RollAvg {
private:

    unsigned int avgLen;
    float* avgs;
    unsigned int index = 0;
    char rolledOver = 0;


public:
    float rollingAvg();
    float value = 0;

    RollAvg(int rollingAvgLength = 1);

    void updateAvg(float val);

};

#endif
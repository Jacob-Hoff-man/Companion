#ifndef INCLUDE_STAT
#define INCLUDE_STAT

#include <ArduinoBLE.h>
#include <CSTDefinitions.h>

class Stat {
private:
    //BLEBroadcast, BLERead, BLEWriteWithoutResponse, BLEWrite, BLENotify, BLEIndicate
    static const unsigned char mask = BLERead | BLEWrite | BLENotify;
    static const int statCount = 24;

    unsigned int avgLen;
    float* avgs;
    unsigned int index = 0;
    float base = 0;
    char rolledOver = 0;


public:
    float rollingAvg();
    float value = 0;
    char state = 0; // this may come in handy if multiple events control a stat
    BLEFloatCharacteristic* _characteristic;

    static Stat* statArray[24];// = { 0 };

    Stat* getStat(char type);

    //should make char* array and make it pull uiids from that once you give it type.
    //could also make an array to point to the service.
    //then you just need rollingAvgLength and type.
    Stat(char* uuid, BLEService service, char type, int rollingAvgLength = 1);

    void updateStat(float value);
    void updateAvg(float value);
    void updateBLE(float value);
    void tareAvg();
    void tare();

};

#endif
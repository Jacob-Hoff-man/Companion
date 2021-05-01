#include "Stat.h"

//
Stat* Stat::statArray[24] = { 0 };

Stat::Stat(char* uuid, BLEService service, char type, int rollingAvgLength) {
#ifdef STAT_DEBUG
    delay(1);
    Serial.println("\tCreatedStat()");
    delay(1);
#endif // STAT_DEBUG
    base = 0;
    avgLen = rollingAvgLength;
    avgs = new float[rollingAvgLength];

    for (int i = 0; i < rollingAvgLength; i++) {
        avgs[i] = 0;
    }
    _characteristic = new BLEFloatCharacteristic(uuid, mask);
    service.addCharacteristic(*_characteristic);
    statArray[type] = this;
}

Stat* Stat::getStat(char type) {
    return statArray[type];
}

void Stat::updateStat(float val) {
#ifdef STAT_DEBUG
    delay(1);
    Serial.println("\t\tStat.updateStat()");
    delay(1);
#endif // STAT_DEBUG
    updateBLE(val);
    updateAvg(val);
    
}

void Stat::updateAvg(float val) {
#ifdef STAT_DEBUG
    delay(1);
    Serial.println("\t\tStat.updateAvg()");
    delay(1);
#endif
    value = val;
    avgs[index] = val;// - base;
    int nextIndex = (index + 1) % avgLen;
    if (!rolledOver && !nextIndex) {
        rolledOver = 1;
    }
    index = nextIndex;
}

void Stat::updateBLE(float val) {
#ifdef STAT_DEBUG
    delay(1);
    Serial.println("\t\tStat.updateBLE()");
    delay(1);
#endif
    if (_characteristic) {
        _characteristic->writeValue(val);
    }
}

float Stat::rollingAvg() {
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

void Stat::tareAvg() {
    base = rollingAvg();
}

void Stat::tare() {
    base = value;
}
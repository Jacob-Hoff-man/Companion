#pragma once
#ifndef CST_EXTERNS
#define CST_EXTERNS

#include <Adafruit_GPS.h>
#include <Stat.h>

extern float tireDiameter;
extern Adafruit_GPS* GPSptr;
extern Stat** statArray;
extern unsigned long tripStartTime;
extern BLEService CSTService;
extern BLEService GPSService;
extern BLEService PedalService;



#endif

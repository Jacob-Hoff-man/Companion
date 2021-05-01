#include <Arduino.h>
#include <stdlib.h>
#include <avr/dtostrf.h>

#include <Stat.h>
#include <CSTDefinitions.h>
#include "CSTExterns.h"

#include <Adafruit_GFX.h>    // Core graphics library
#include "Adafruit_EPD.h"
#include <Adafruit_GPS.h>



extern Stat** statArray;
extern unsigned long tripStartTime;
extern Adafruit_GPS* GPSptr;


Adafruit_IL0373 display(296, 128, EPD_DC, EPD_RESET, EPD_CS, SRAM_CS, EPD_BUSY);

//TODO: clean function call for adding elements to the table.


void displaySetup() {
#ifdef EPD_ENABLE
    pinMode(EPD_ENABLE, OUTPUT);
    digitalWrite(EPD_ENABLE, LOW); //disable breakout.
#endif
    //display is actually enabled at each update call.
}


const int charWidth = 6;
const int charHeight = 9;

const int _leftPadding = 0; //gap that appears on the left side of the screen
const int _padding = 1;    // used to pad on the left side of a few col's,
const int _NameWidth = 10; //max width of name
const int _UnitWidth = 7;  //max width of units (also pads the left side of right col)
const int _digits = 5;   //amount of digits that appears on the screen.

int _NameColumn;  //location's of columns
int _NumColumn;
int _UnitColumn;


int updateColumnLocations(int offset = 0) { //returns the end of the last column.
    _NameColumn = offset + (_padding * charWidth);
    _NumColumn = _NameColumn + (_NameWidth * charWidth);
    _UnitColumn = _NumColumn + (((_digits + 1) + _padding) * charWidth);
    int _rightPadding = _UnitColumn + (_UnitWidth * charWidth);
    return _rightPadding;
}

void printCol(const char* name, float value, const char* units, int height, int offset) {
    
    int bufferWidth = 8;
    char stringBuffer[bufferWidth + 1] = { 0 };

    height *= charHeight;

    display.setCursor(_NameColumn, height);
    display.print(name);

    int digits = _digits;
    for (int i = 1; i <= 1000; i *= 10) {
        if (i <= abs(value))
            digits--;
    }

    if (value < 0) //done because negative sign takes up one char.
        digits--;
    if (value < 1 && value > -1)// this is done because the period in decimals takes up one char
        digits--;

    dtostrf(value, -bufferWidth, digits, stringBuffer);

    display.setCursor(_NumColumn, height);
    display.print(stringBuffer);

    display.setCursor(_UnitColumn, height);
    display.print(units);


}

void printRightCol(const char* name, float value, const char* units, int height) { //prints a col on the right side
    int offset = updateColumnLocations(updateColumnLocations(_leftPadding * charWidth));
    printCol(name, value, units, height, offset);
}

void printLeftCol(const char* name, float value, const char* units, int height) {
    int offset = updateColumnLocations(_leftPadding * charWidth);
    printCol(name, value, units, height, offset);
}

float getAvg(int x) {
    return statArray[x]->rollingAvg();
}

float getVal(int x) {
    return statArray[x]->value;
}



void updateDisplay() {
#ifdef CST_DEBUG
    Serial.println("Updating display..");
#endif

#ifdef EPD_ENABLE
    digitalWrite(EPD_ENABLE, HIGH);
#endif
    delay(5); //need to give the buffer time to power on.
    display.begin();

    display.setBlackBuffer(1, false);
    display.setColorBuffer(1, false);
    display.setRotation(2);
    display.setTextWrap(false);
    display.clearBuffer();
    display.setTextSize(1);
    display.setTextColor(EPD_BLACK);

    //constants used for testing.. delete later
    const float zero = 0.0000000;

    int bufferWidth = 9;
    char stringBuffer[bufferWidth + 1] = { 0 };

    
    unsigned long tripTime = millis() - tripStartTime;
    unsigned long tripTimeSecs = tripTime / 1000;

    int hour = (tripTimeSecs / 60) / 60;
    int minute = (tripTimeSecs / 60) - (hour * 60);
    int second = tripTimeSecs - (hour * 60 * 60) - (minute * 60);

    //duration:  H:MM:SS
    display.setCursor((_leftPadding + _padding + 1) * charWidth, 1 * charHeight);
    display.print("DURATION: ");
    display.print(hour);
    display.print(":");
    if (minute < 10)
        display.print(0);
    display.print(minute);
    display.print(":");
    if (second < 10)
        display.print(0);
    display.print(second);


    //Time   12:34 AM

    bool AM = true;
    int timeMinute = GPSptr->minute;
    int timeHour = GPSptr->hour - 4;

    if (timeHour <= 0)
        timeHour += 24;
    if (timeHour >= 12 && timeHour != 24)
        AM = false;
    if (timeHour > 12)
        timeHour -= 12;

    display.setCursor(40 * charWidth, 1 * charHeight);
    display.print(timeHour);
    display.print(":");
    if (timeMinute < 10)
        display.print(0);
    display.print(timeMinute);

    if (AM)
        display.print(" AM");
    else
        display.print(" PM");


    //GPS COORDINATES
    display.setCursor(10 * charWidth, 3 * charHeight);

    display.print("GPS: ");

    dtostrf(getAvg(_latitude), -bufferWidth, 6, stringBuffer);
    display.print(stringBuffer);

    display.print(" , ");

    dtostrf(getAvg(_longitude), -bufferWidth, 6, stringBuffer);
    display.print(stringBuffer);


    int initialColHeight = 5;

    //Left Col

    printLeftCol("SPEED:", getAvg(_speed), "m/s", initialColHeight);
    printLeftCol("DISTANCE:", getVal(_distance), "km", initialColHeight + 1);
    printLeftCol("AVG SPD:", getVal(_averageSpeed), "m/s", initialColHeight + 2);
    printLeftCol("ACCEL:", getAvg(_acceleration), "m/s*s", initialColHeight + 3);

    printLeftCol("INCLINE:", getAvg(_inclineAngle), "deg", initialColHeight + 5);
    printLeftCol("ELEVAT:", getAvg(_altitude), "m", initialColHeight + 6);
    printLeftCol("AVG EFF:", getVal(_distance)* 1000 / (getVal(_calories)* 4184.0), "m/J", initialColHeight + 7);

    //Right Col
    printRightCol("SPEED:", getAvg(_speed)*3.6, "km/h", initialColHeight);
    printRightCol("POWER:", getAvg(_power), "watt", initialColHeight + 1);
    printRightCol("CALORIES:", getVal(_calories), "kC", initialColHeight + 2);
    printRightCol("GEAR RAT:", getAvg(_gearRatio), " ", initialColHeight + 3);
    printRightCol("CADENCE:", getAvg(_cadence), "p/m", initialColHeight + 4);

    //printRightCol("WIND SPD:", zero, "m/s", initialColHeight + 5);
    //printRightCol("WIND DIR:", zero, "deg", initialColHeight + 6);
    //printRightCol("WIND DIR:", zero, "deg", initialColHeight + 6);
    printRightCol("TORQUE:", getAvg(_torque), "NM", initialColHeight + 6);
    printRightCol("CUR EFF:", getAvg(_speed)/getAvg(_power), "m/J", initialColHeight + 7);

    display.display();
#ifdef EPD_ENABLE
    digitalWrite(EPD_ENABLE, LOW); //disable breakout.
#endif

#ifdef CST_DEBUG
    Serial.println("Display Updated..");
#endif
}

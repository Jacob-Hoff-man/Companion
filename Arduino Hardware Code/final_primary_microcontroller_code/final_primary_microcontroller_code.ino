#if 1
#include <vector>
#include <ArduinoBLE.h>

//CST Libraries
#include "CSTDefinitions.h"
#include "updateEvent.h"
#include "Stat.h"
#include "CSTExterns.h"
#include "display.h"
#include "PrimaryBLEHandling.h"

void updateSensors();


extern unsigned long tripStartTime = 0;
extern Stat** statArray = 0;
extern Adafruit_GPS* GPSptr = 0;

// PUT THIS IN PrimaryBLEHandling.h
extern BLEService CSTService;// = BLEService(UUID_CST_SERVICE);
extern BLEService GPSService;// = GPSService(UUID_GPS_SERVICE);
extern BLEService PedalService;// = PedalService(UUID_PEDAL_SERVICE);

std::vector<UpdateEvent*> quickEventList;
std::vector<UpdateEvent*> slowEventList;

unsigned long loopTime, quickUpdateTime, slowUpdateTime, displayUpdateTime;


void setup() {
#ifdef VALUE_PRINT || CST_DEBUG || STAT_DEBUG || GPS_DEBUG || BLE_DEBUG
    Serial.begin(9600);
    //while (!Serial)
    Serial.println("Starting CST primary microcontroller..");
#endif

    pinMode(2, OUTPUT);
    digitalWrite(2, HIGH);
    delay(1000);
    digitalWrite(2,LOW);


    BLE.begin();
    

    displaySetup(); //see display.h

    //Quick Events
    quickEventList.push_back(new IRTireEvent(CSTService, ROLLING_AVERAGE_POINTS));

    //need to fetch array from a stat
    statArray = quickEventList.at(0)->stat->statArray;
    //could this be done elsewhere??

    //Slow Events
    slowEventList.push_back(new GPSEvent(GPSService));
    slowEventList.push_back(new IMUEvent(CSTService, ROLLING_AVERAGE_POINTS));

    //need to fetch GPS for display from the GPSEvent..
    GPSptr = &(((GPSEvent*)slowEventList.at(0))->GPS); //get gpsevent (first in vec) and then get GPS* from it.
    //should move to updateEvent.cpp. no need for main to have it.


    BLESetup();     //see PrimaryBLEHandling.h

    tripStartTime = quickUpdateTime = slowUpdateTime = displayUpdateTime = 0;

    updateDisplay();
}

int LEDSTATE = HIGH;
unsigned long timer = 0;
void loop() {

    if(millis() - timer > 1000){
      LEDSTATE = (LEDSTATE == HIGH) ? LOW : HIGH;
      digitalWrite(2, LEDSTATE);
      timer = millis();
    }
  
    updateSensors(); //calls sensor events if they haven't been fired in a while
    BLE.poll(); //checks for BLE Events.
    //delay(1); //polling too quickly causes crashes.
}

void updateSensors() {

    loopTime = millis();
    unsigned long timeSinceQuickUpdate = loopTime - quickUpdateTime;
    unsigned long timeSinceSlowUpdate = loopTime - slowUpdateTime;
    unsigned long timeSinceDisplayUpdate = loopTime - displayUpdateTime;

    if (timeSinceQuickUpdate > QUICK_STAT_UPDATE_PERIOD) {
        for (UpdateEvent* event : quickEventList)
            event->update();
        quickUpdateTime = millis();
    }
    //could add timer to updateEvent.
    if (timeSinceSlowUpdate > SLOW_STAT_UPDATE_PERIOD) {
        for (UpdateEvent* event : slowEventList)
            event->update();
        slowUpdateTime = millis();
    }
    //add to update display
    if (timeSinceDisplayUpdate > DISPLAY_UPDATE_PERIOD) {
        updateDisplay();
        displayUpdateTime = millis();
    }

}

#endif

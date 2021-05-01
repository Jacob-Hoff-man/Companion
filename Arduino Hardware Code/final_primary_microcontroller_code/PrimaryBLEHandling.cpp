#include <Arduino.h>
#include <ArduinoBLE.h>
#include "CSTDefinitions.h"
#include "CSTExterns.h"
#include "Stat.h"

extern BLEService CSTService = BLEService(UUID_CST_SERVICE);
extern BLEService GPSService = BLEService(UUID_GPS_SERVICE);
extern BLEService PedalService = BLEService(UUID_PEDAL_SERVICE);

extern Stat** statArray;
extern float tireDiameter;

Stat calories(UUID_CALORIES, PedalService, _calories, 25);
Stat cadence(UUID_CADENCE, PedalService, _cadence, 25);
Stat power(UUID_POWER, PedalService, _power, 25);
Stat torque(UUID_TORQUE, PedalService, _torque, 25);
Stat gearRatio(UUID_GEAR_RATIO, PedalService, _gearRatio, 25);


void blePeripheralConnectHandler(BLEDevice deviceCalled);
void blePeripheralDisconnectHandler(BLEDevice deviceCalled);

void bleCentralDiscoverHandler(BLEDevice peripheral);

void caloriesWrittenHandler(BLEDevice central, BLECharacteristic characteristic);
void cadenceWrittenHandler(BLEDevice central, BLECharacteristic characteristic);
void powerWrittenHandler(BLEDevice central, BLECharacteristic characteristic);
void torqueWrittenHandler(BLEDevice central, BLECharacteristic characteristic);



void BLESetup() {
    //BLE.begin();
    BLE.setLocalName("CST : Primary");
    BLE.setAdvertisedService(CSTService);
    //BLE.setAdvertisedService(GPSService);
    //BLE.setAdvertisedService(PedalService);

#ifdef BLE_DEBUG
    Serial.print("\tPrimary BLE Address:\t");
    Serial.println(BLE.address());
#endif

    BLE.addService(CSTService);
    BLE.addService(PedalService);
    BLE.addService(GPSService);
    


    /////////CHARACTERISTIC EVENT HANDLERS///////////
    //BLECharacteristic* powerChar = power._characteristic;
    calories._characteristic->setEventHandler(BLEWritten, caloriesWrittenHandler);
    cadence._characteristic->setEventHandler(BLEWritten, cadenceWrittenHandler);
    power._characteristic->setEventHandler(BLEWritten, powerWrittenHandler);
    torque._characteristic->setEventHandler(BLEWritten, torqueWrittenHandler);


    //UBIQUITOUS EVENT HANDLERS
    //These events are triggered by the phone and secondary pedal.
    BLE.setEventHandler(BLEConnected, blePeripheralConnectHandler);
    BLE.setEventHandler(BLEDisconnected, blePeripheralDisconnectHandler);

    //Triggered only by finding a secondary device
    BLE.setEventHandler(BLEDiscovered, bleCentralDiscoverHandler);


    BLE.advertise(); //this advertises for the phone.

    //BLE.scan(true);
    BLE.scanForAddress(PEDAL_ADDRESS, true); //looks for pedal
#ifdef BLE_DEBUG
    Serial.print("Set up BLE. Advertising on  ");
    Serial.println(BLE.address());
#endif
}



///////CONNECTION EVENT HANDLERS/////


void blePeripheralConnectHandler(BLEDevice deviceCalled) {
#ifdef BLE_DEBUG
    Serial.print("\n\t\tDevice connected:\t");
    Serial.println(deviceCalled.address());
    Serial.println();
#endif

}
void blePeripheralDisconnectHandler(BLEDevice deviceCalled) {
#ifdef BLE_DEBUG
    Serial.print("\n\t\tDevice disconnected:\t");
    Serial.println(deviceCalled.address());
    Serial.println();
#endif

    if (deviceCalled.address() == PEDAL_ADDRESS) {
#ifdef BLE_DEBUG
        Serial.println("Secondary CST disconnected. Scanning Again.");
#endif
        BLE.scanForAddress(PEDAL_ADDRESS, true);
    }
}

//Triggered by finding a secondary device.
void bleCentralDiscoverHandler(BLEDevice peripheral) {
#ifdef BLE_DEBUG
    Serial.println("Found Secondary CST: ");
    Serial.println(peripheral.address());
#endif

    BLE.stopScan(); //connect will fail otherwise

    if (peripheral.connect()) {
#ifdef BLE_DEBUG
        Serial.println("Connected");
#endif
    }
    else {
#ifdef BLE_DEBUG
        Serial.println("Not connected. Scanning again.");
#endif
        BLE.scanForAddress(PEDAL_ADDRESS, true);
    }
}



 ///////////////////// CHARACTERISTIC HANDLERS /////////////////////



void caloriesWrittenHandler(BLEDevice central, BLECharacteristic characteristic) {
    float value = calories._characteristic->value();
    calories.updateAvg(value);
    

#ifdef BLE_DEBUG || VALUE_PRINT
    delayMicroseconds(5);
    Stat* statToPrint = &calories;
    Serial.print("\tCalories:\t");
    Serial.print(statToPrint->value, 4);
    //Serial.print("\t\tAvg:\t");
    //Serial.print(statToPrint->rollingAvg(), 4);
    Serial.println();
    
#endif
}

void cadenceWrittenHandler(BLEDevice central, BLECharacteristic characteristic) {
    float value = cadence._characteristic->value();
    cadence.updateAvg(value);

    float torqueValue = statArray[_torque]->value;
    float inclineValue = statArray[_inclineAngle]->value;
    if (torqueValue > TORQUE_MIN && inclineValue > INCLINE_MIN) {
        float speedValue = statArray[_speed]->rollingAvg();
        float tireRPM = speedValue * 60 / (tireDiameter * PI * 2 / 39.3700787); //39.3700787 inches in a meter
        statArray[_gearRatio]->updateStat(value/tireRPM);
    }



#ifdef BLE_DEBUG || VALUE_PRINT
    delayMicroseconds(5);
    Stat* statToPrint = &cadence;
    Serial.print("\tCadence:\t");
    Serial.print(statToPrint->value, 4);
    //Serial.print("\t\tAvg:\t");
    //Serial.print(statToPrint->rollingAvg(), 4);
    Serial.println();
#endif
}


void powerWrittenHandler(BLEDevice central, BLECharacteristic characteristic) {
    float value = power._characteristic->value();
    power.updateAvg(value);


#ifdef BLE_DEBUG || VALUE_PRINT
    delayMicroseconds(5);
    Stat* statToPrint = &power;
    Serial.print("\tPower:\t");
    Serial.print(statToPrint->value, 4);
    //Serial.print("\t\tAvg:\t");
    //Serial.print(statToPrint->rollingAvg(), 4);
    Serial.println();

#endif
}
void torqueWrittenHandler(BLEDevice central, BLECharacteristic characteristic) {
    float value = torque._characteristic->value();
    torque.updateAvg(value);

#ifdef BLE_DEBUG || VALUE_PRINT
    delayMicroseconds(5);
    Stat* statToPrint = &torque;
    Serial.print("\tTorque:\t");
    float torq = statToPrint->value;
    Serial.print(torq, 4);
    Serial.print("\t\tMass:\t");
    Serial.print(torq/.175, 4);
    Serial.println();

#endif
}


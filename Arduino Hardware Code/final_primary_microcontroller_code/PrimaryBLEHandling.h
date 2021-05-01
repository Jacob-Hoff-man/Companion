#include <ArduinoBLE.h>

void BLESetup();

void blePeripheralConnectHandler(BLEDevice deviceCalled);
void blePeripheralDisconnectHandler(BLEDevice deviceCalled);
void bleCentralDiscoverHandler(BLEDevice peripheral);

void caloriesWrittenHandler(BLEDevice central, BLECharacteristic characteristic);
void cadenceWrittenHandler(BLEDevice central, BLECharacteristic characteristic);
void powerWrittenHandler(BLEDevice central, BLECharacteristic characteristic);
void torqueWrittenHandler(BLEDevice central, BLECharacteristic characteristic);